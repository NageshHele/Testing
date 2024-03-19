#ifndef UDPSOKCET_HPP
#define UDPSOKCET_HPP

#include <sys/types.h>
//#include <sys/socket.h>
//#include <arpa/inet.h>
//#include <netinet/in.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <string>

#include <boost/asio.hpp>

using boost::asio::ip::udp;
using boost::asio::ip::address;

class UdpSokcet
{
    public:
        UdpSokcet(boost::asio::io_service& io, std::string IPADDRESS, int UDP_PORT);
        virtual ~UdpSokcet();

        void Send(char *buffer, size_t size_);

    protected:

    private:

        udp::socket socket__;
        udp::endpoint remote_endpoint;
};

#endif // UDPSOKCET_HPP
