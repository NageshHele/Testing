#include "../include/EpollSocket.hpp"
#include "../include/Setting.hpp"

#include <winsock2.h>
#include "../include/wepoll.h"
//#include <netinet/in.h>
//#include <arpa/inet.h>
#include <unistd.h>
#include <iostream>
//#include <sys/epoll.h>
#include <cstring>

#define MAX_EVENTS 64
#define BufferSize 20480

EpollSocket::EpollSocket(boost::asio::io_service& io) : ProcessFeed(io)
{
    //ctor
}

EpollSocket::~EpollSocket()
{
    //dtor
}

int EpollSocket::prepare_mc_socket(int StreamId, std::string LanIp, std::string McastIp, int Port)
{
    struct ip_mreq group;

    int fd = socket(AF_INET, SOCK_DGRAM, 0);
    if(fd < 0)
    {
        std::cout << "Unable to open socket with id " << StreamId << " errono " << errno << std::endl;
        perror("socket");
    }

    //std::cout << "Socket opened successfully with id " << StreamId << std::endl;

    int reuse = 1;

    if(setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, (char*)&reuse, sizeof(reuse)) < 0)
    {
        std::cout << "Setting SO_REUSEADDR failed with id " << StreamId << " errno " << errno << std::endl;
        perror("setsockopt : SO_REUSEADDR");
        close(fd);
        exit(EXIT_FAILURE);
    }
    //std::cout << "Setting SO_REUSEADDR done with id " << StreamId << std::endl;

    struct sockaddr_in localSock;
    ///memset((char*)&localSock, 0, sizeof(localSock));
    localSock.sin_family = AF_INET;
    localSock.sin_addr.s_addr = INADDR_ANY;///inet_addr(McastIp.c_str());
    localSock.sin_port = ntohs(Port);

    std::cout << "socket fd " << fd << std::endl;

    if(int retBind = bind(fd, (struct sockaddr*)&localSock, sizeof(struct sockaddr)) != 0)
    {
        std::cout << "Binding socket failed with id " << StreamId << " errno " << errno << " retVal " << retBind << std::endl;
        perror("bind");
        close(fd);
        exit(EXIT_FAILURE);
    }
    //std::cout << "Binding successful with id " << StreamId << std::endl;

    group.imr_multiaddr.s_addr = inet_addr(McastIp.c_str());
    ///group.imr_interface.s_addr = inet_addr(LanIp.c_str());
    group.imr_interface.s_addr = ntohl(INADDR_ANY);
    if(setsockopt(fd, IPPROTO_IP, IP_ADD_MEMBERSHIP, (char*)&group, sizeof(group)) < 0)
    {
        std::cout << "Adding MCAST group for socket failed with id " << StreamId << " errno " << errno << std::endl;
        perror("setsockopt : IP_ADD_MEMBERSHIP");
        close(fd);
        exit(EXIT_FAILURE);
    }
    //std::cout << "Adding MC group for socket done with id " << StreamId << std::endl;

    return fd;
}

void EpollSocket::ConstructBinding(HANDLE epollfd, int StreamId, std::string LanIp, std::string McastIp, int Port)
{
    struct epoll_event eva;
    int sfd_replay_a = 0;

    sfd_replay_a = prepare_mc_socket(StreamId, LanIp, McastIp, Port);

    if(sfd_replay_a == -1)
    {
        perror("prepare_mc_socket");
        exit(EXIT_FAILURE);
    }

    eva.events = EPOLLIN;
    eva.data.fd = sfd_replay_a;

    if(epoll_ctl(epollfd, EPOLL_CTL_ADD, sfd_replay_a, &eva) == -1)
    {
        perror("epoll_ctl");
        exit(EXIT_FAILURE);
    }

    std::cout << "StreamId " << StreamId << " connected successfully" << std::endl;
}
#define _WIN32
void EpollSocket::BindIncreamental()
{
#ifdef _WIN32
    //
    // Initialize Windows Socket API with given VERSION.
    //
    WSADATA wsaData;
    if (WSAStartup(0x0101, &wsaData)) {
        perror("WSAStartup");
        return 1;
    }
#endif

    // create what looks like an ordinary UDP socket
    //
    int fd = socket(AF_INET, SOCK_DGRAM, 0);
    if (fd < 0) {
        perror("socket");
        return 1;
    }

    // allow multiple sockets to use the same PORT number
    //
    u_int yes = 1;
    if (
        setsockopt(
            fd, SOL_SOCKET, SO_REUSEADDR, (char*) &yes, sizeof(yes)
        ) < 0
    ){
       perror("Reusing ADDR failed");
       return 1;
    }

        // set up destination address
    //
    struct sockaddr_in addr;
    memset(&addr, 0, sizeof(addr));
    addr.sin_family = AF_INET;
    addr.sin_addr.s_addr = htonl(INADDR_ANY); // differs from sender
    addr.sin_port = htons(Setting::getInstance()->getValue<short>("FEED.PORT0"));

    // bind to receive address
    //
    if (bind(fd, (struct sockaddr*) &addr, sizeof(addr)) < 0) {
        perror("bind");
        return 1;
    }

    // use setsockopt() to request that the kernel join a multicast group
    //
    struct ip_mreq mreq;
    mreq.imr_multiaddr.s_addr = inet_addr(Setting::getInstance()->getValue<std::string>("FEED.MCASTIP0").c_str());
    mreq.imr_interface.s_addr = htonl(INADDR_ANY);
    if (
        setsockopt(
            fd, IPPROTO_IP, IP_ADD_MEMBERSHIP, (char*) &mreq, sizeof(mreq)
        ) < 0
    ){
        perror("setsockopt");
        return 1;
    }

    // now just enter a read-print loop
    std::cout << "Before read" << std::endl;
    while (1) {
        char msgbuf[BufferSize];
        //memset(msgbuf, 0, MSGBUFSIZE);
        int addrlen = sizeof(addr);
        int nbytes = recvfrom(
            fd,
            msgbuf,
            BufferSize,
            0,
            (struct sockaddr *) &addr,
            &addrlen
        );
        if (nbytes < 0) {
            perror("recvfrom");
            return 1;
        }
        //msgbuf[nbytes] = '\0';
        std::cout << nbytes << " msg :: " << msgbuf << std::endl;

        ProcessDataWithLogs(msgbuf, nbytes);
     }

    return;

    struct epoll_event events[MAX_EVENTS];
    //int epollfd = epoll_creatl(0);
    HANDLE epollfd = epoll_create1(0);
    if(epollfd == -1)
    {
        perror("epoll_creatl");
        exit(EXIT_FAILURE);
    }

    const std::string interfaceAddress (Setting::getInstance()->getValue<std::string>("FEED.LANIP"));
    const int count_ (Setting::getInstance()->getValue<int>("FEED.COUNT"));

    std::cout << std::endl << "LANIP " << interfaceAddress << std::endl << std::endl;

    for(int i = 0; i < 1; i++)
    {
        std::string address = "FEED.MCASTIP" + std::to_string(i);
        std::string port = "FEED.MCASTPORT" + std::to_string(i);

        std::cout
                << address << "   : " << Setting::getInstance()->getValue<std::string>(address) << std::endl
                << port << " : " << Setting::getInstance()->getValue<int>(port)
                << std::endl;

/**
        int fd = prepare_mc_socket(i, interfaceAddress, Setting::getInstance()->getValue<std::string>(address), Setting::getInstance()->getValue<int>(port));
        char buff[BufferSize] = {0};

        while(1)
        {
            int retLen = read(fd, buff, BufferSize);
            if(retLen == -1)
            {
                perror("read");
                close(fd);
                exit(1);
            }
            else
                ProcessDataWithLogs(buff, retLen);
        }

        return;
**/
        this->ConstructBinding(epollfd, i, interfaceAddress, Setting::getInstance()->getValue<std::string>(address), Setting::getInstance()->getValue<int>(port));

        std::cout << std::endl;
    }

    std::cout << std::endl;

    char buffer[BufferSize] = {0};

    std::cout << __LINE__ << " About to read" << std::endl;

    if(Setting::getInstance()->getValue<int>("LOGS.ENABLE"))
    {
        while(true)
        {
            std::cout << __LINE__ << " Before epoll_wait" << std::endl;
            int nfds = epoll_wait(epollfd, events, MAX_EVENTS, -1);
            if(nfds == -1)
            {
                std::cout << __LINE__ << " Something is wrong in epoll_wait" << std::endl;
                continue;
            }

            std::cout << __LINE__ << " Event recieved" << std::endl;

            for(int n = 0; n < nfds; n++)
            {
                if((events[n].events & EPOLLERR) || (events[n].events & EPOLLHUP) || (!(events[n].events & EPOLLIN)))
                {
                    fprintf(stderr, "epoll_error\n");
                    close(events[n].data.fd);
                    continue;
                }

                if(events[n].events & EPOLLIN)
                {
                    std::cout << __LINE__ << " About to read" << std::endl;
                    int len = recv(events[n].data.fd, buffer, BufferSize, 0);
                    if(len < 0)
                    {
                        std::cout << __LINE__ << " Data recv len " << len << std::endl;
                        close(events[n].data.fd);
                    }
                    else
                    {
                        ProcessDataWithLogs(buffer, len);
                    }
                }
            }
        }
    }
    else
    {
        while(true)
        {
            std::cout << __LINE__ << " Before epoll_wait" << std::endl;
            int nfds = epoll_wait(epollfd, events, MAX_EVENTS, -1);
            if(nfds == -1)
            {
                std::cout << __LINE__ << " Something is wrong in epoll_wait" << std::endl;
                continue;
            }

            std::cout << __LINE__ << " Event recieved" << std::endl;

            for(int n = 0; n < nfds; n++)
            {
                if((events[n].events & EPOLLERR) || (events[n].events & EPOLLHUP) || (!(events[n].events & EPOLLIN)))
                {
                    fprintf(stderr, "epoll_error\n");
                    close(events[n].data.fd);
                    continue;
                }

                if(events[n].events & EPOLLIN)
                {
                    std::cout << __LINE__ << " About to read" << std::endl;
                    int len = recv(events[n].data.fd, buffer, BufferSize, 0);
                    if(len < 0)
                    {
                        std::cout << __LINE__ << " Data recv len " << len << std::endl;
                        close(events[n].data.fd);
                    }
                    else
                    {
                        ProcessData(buffer, len);
                    }
                }
            }
        }
    }
    BindIncreamental();
}

/**
void EpollSocket::BindIncreamental()
{
    struct epoll_event events[MAX_EVENTS];
    //int epollfd = epoll_creatl(0);
    HANDLE epollfd = epoll_create1(0);
    if(epollfd == -1)
    {
        perror("epoll_creatl");
        exit(EXIT_FAILURE);
    }

    const std::string interfaceAddress (Setting::getInstance()->getValue<std::string>("FEED.LANIP"));
    const int count_ (Setting::getInstance()->getValue<int>("FEED.COUNT"));

    std::cout << std::endl << "LANIP " << interfaceAddress << std::endl << std::endl;

    for(int i = 0; i < count_; i++)
    {
        std::string address = "FEED.MCASTIP" + std::to_string(i);
        std::string port = "FEED.MCASTPORT" + std::to_string(i);

        std::cout
                << address << "   : " << Setting::getInstance()->getValue<std::string>(address) << std::endl
                << port << " : " << Setting::getInstance()->getValue<int>(port)
                << std::endl;

        this->ConstructBinding(epollfd, i, interfaceAddress, Setting::getInstance()->getValue<std::string>(address), Setting::getInstance()->getValue<int>(port));

        std::cout << std::endl;
    }

    std::cout << std::endl;

    char buffer[BufferSize] = {0};

    std::cout << __LINE__ << " About to read" << std::endl;

    if(Setting::getInstance()->getValue<int>("LOGS.ENABLE"))
    {
        while(true)
        {
            std::cout << __LINE__ << " Before epoll_wait" << std::endl;
            int nfds = epoll_wait(epollfd, events, MAX_EVENTS, -1);
            if(nfds == -1)
            {
                std::cout << __LINE__ << " Something is wrong in epoll_wait" << std::endl;
                continue;
            }

            std::cout << __LINE__ << " Event recieved" << std::endl;

            for(int n = 0; n < nfds; n++)
            {
                if((events[n].events & EPOLLERR) || (events[n].events & EPOLLHUP) || (!(events[n].events & EPOLLIN)))
                {
                    fprintf(stderr, "epoll_error\n");
                    close(events[n].data.fd);
                    continue;
                }

                if(events[n].events & EPOLLIN)
                {
                    std::cout << __LINE__ << " About to read" << std::endl;
                    int len = recv(events[n].data.fd, buffer, BufferSize, 0);
                    if(len < 0)
                    {
                        std::cout << __LINE__ << " Data recv len " << len << std::endl;
                        close(events[n].data.fd);
                    }
                    else
                    {
                        ProcessDataWithLogs(buffer, len);
                    }
                }
            }
        }
    }
    else
    {
        while(true)
        {
            std::cout << __LINE__ << " Before epoll_wait" << std::endl;
            int nfds = epoll_wait(epollfd, events, MAX_EVENTS, -1);
            if(nfds == -1)
            {
                std::cout << __LINE__ << " Something is wrong in epoll_wait" << std::endl;
                continue;
            }

            std::cout << __LINE__ << " Event recieved" << std::endl;

            for(int n = 0; n < nfds; n++)
            {
                if((events[n].events & EPOLLERR) || (events[n].events & EPOLLHUP) || (!(events[n].events & EPOLLIN)))
                {
                    fprintf(stderr, "epoll_error\n");
                    close(events[n].data.fd);
                    continue;
                }

                if(events[n].events & EPOLLIN)
                {
                    std::cout << __LINE__ << " About to read" << std::endl;
                    int len = recv(events[n].data.fd, buffer, BufferSize, 0);
                    if(len < 0)
                    {
                        std::cout << __LINE__ << " Data recv len " << len << std::endl;
                        close(events[n].data.fd);
                    }
                    else
                    {
                        ProcessData(buffer, len);
                    }
                }
            }
        }
    }
    BindIncreamental();
}
**/
