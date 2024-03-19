#include <iostream>
#include <boost/asio/io_service.hpp>
#include <time.h>

#include "include/EpollSocket.hpp"
#include "include/Setting.hpp"

std::string CurrentTime()
{
    time_t rawtime;
    struct tm * timeinfo;

    time ( &rawtime );
    timeinfo = localtime ( &rawtime );
   // printf ( "Current local time and date: %s", asctime (timeinfo) );

    return std::string(asctime (timeinfo));
}

int UDPConnect()
{
    int sockfd;
    struct sockaddr_in servaddr;

    // Creating socket file descriptor
    if ( (sockfd = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) < 0 )
    {
        perror("socket creation failed");
        exit(EXIT_FAILURE);
    }

    memset(&servaddr, 0, sizeof(servaddr));

    std::cout << "UDP receive on " << Setting::getInstance()->getValue<std::string>("BROADCAST_RECEIVE.IPADDRESS") << ":"
    << Setting::getInstance()->getValue<std::string>("BROADCAST_RECEIVE.PORT") << std::endl;

    // Filling server information
    servaddr.sin_family    = AF_INET; // IPv4
    servaddr.sin_addr.s_addr = inet_addr(Setting::getInstance()->getValue<std::string>("BROADCAST_RECEIVE.IPADDRESS").c_str());
    servaddr.sin_port = htons(Setting::getInstance()->getValue<short>("BROADCAST_RECEIVE.PORT"));


    int a=1;
    if(setsockopt(sockfd, SOL_SOCKET, SO_REUSEADDR, (char*)&a, sizeof(a)) < 0)
    {
        perror("setsockopt failed");
        exit(EXIT_FAILURE);
    }

    // Bind the socket with the server address
    if ( bind(sockfd, (const struct sockaddr *)&servaddr,
            sizeof(servaddr)) < 0 )
    {
        perror("bind failed");
        exit(EXIT_FAILURE);
    }

    return sockfd;
}

int main()
{


    std::cout << "Program Started at " << CurrentTime() << std::endl;

    boost::asio::io_service io;

    EpollSocket *EpollSocket_ = new EpollSocket(io);
/**
    std::thread t([EpollSocket_]()
    {
        char recvString[10000];
        int recvStringlen;

        int sock = UDPConnect();

        uint64_t a = 1;

        while(1)
        {
            if((recvStringlen = recvfrom(sock, recvString, 10000, 0, NULL, 0)) < 0)
            {
                std::cout << "recvfrom() failed " << a++ << std::endl;
                continue;
            }

            if(Setting::getInstance()->getValue<int>("LOGS.ENABLE"))
                EpollSocket_->ProcessIMLData(recvString, recvStringlen);
                ///EpollSocket_->ProcessDataWithLogs(recvString, recvStringlen);
            else
                EpollSocket_->ProcessData(recvString, recvStringlen);
        }
    });
**/
    EpollSocket_->BindIncreamental();

    ///EpollSocket_->FakeData();

    io.run();

    ///t.join();



    return 0;
}
