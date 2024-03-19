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
    struct sockaddr_in localSock;
    struct ip_mreq group;

    int fd = socket(AF_INET, SOCK_DGRAM, 0);
    if(fd < 0)
    {
        std::cout << "Unable to open socket with id " << StreamId << std::endl;
    }

    //std::cout << "Socket opened successfully with id " << StreamId << std::endl;

    int reuse = 1;

    if(setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, (char*)&reuse, sizeof(reuse)) < 0)
    {
        std::cout << "Setting SO_REUSEADDR failed with id " << StreamId << std::endl;
        close(fd);
        exit(EXIT_FAILURE);
    }
    //std::cout << "Setting SO_REUSEADDR done with id " << StreamId << std::endl;

    memset((char*)&localSock, 0, sizeof(localSock));
    localSock.sin_family = AF_INET;
    localSock.sin_addr.s_addr = inet_addr(McastIp.c_str());
    localSock.sin_port = htons(Port);

    if(bind(fd, (struct sockaddr*)&localSock, sizeof(struct sockaddr)) == -1)
    {
        std::cout << "Binding socket failed with id " << StreamId << std::endl;
        close(fd);
        exit(EXIT_FAILURE);
    }
    //std::cout << "Binding successful with id " << StreamId << std::endl;

    group.imr_multiaddr.s_addr = inet_addr(McastIp.c_str());
    group.imr_interface.s_addr = inet_addr(LanIp.c_str());
    if(setsockopt(fd, IPPROTO_IP, IP_ADD_MEMBERSHIP, (char*)&group, sizeof(group)) < 0)
    {
        std::cout << "Adding MC group for socket failed with id " << StreamId << std::endl;
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

    if(Setting::getInstance()->getValue<int>("LOGS.ENABLE"))
    {
        while(true)
        {
            int nfds = epoll_wait(epollfd, events, MAX_EVENTS, -1);
            if(nfds == -1)
            {
                continue;
            }

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
            int nfds = epoll_wait(epollfd, events, MAX_EVENTS, -1);
            if(nfds == -1)
            {
                continue;
            }

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
