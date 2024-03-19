#include "../include/UdpSokcet.hpp"
#include <iostream>

UdpSokcet::UdpSokcet(boost::asio::io_service& io, std::string IPADDRESS, int UDP_PORT) : socket__(io)
{
    remote_endpoint = udp::endpoint(boost::asio::ip::address::from_string(IPADDRESS), UDP_PORT);
    socket__.open(udp::v4());
    socket__.set_option(boost::asio::ip::udp::socket::reuse_address(true));
    socket__.set_option(boost::asio::socket_base::broadcast(true));

    std::cout << "Data will be Broadcasting on " << IPADDRESS << "@" << UDP_PORT << std::endl;
}

UdpSokcet::~UdpSokcet()
{
    socket__.close();
}

void UdpSokcet::Send(char *buffer, size_t size_)
{
    boost::system::error_code err;
    int sent = socket__.send_to(boost::asio::buffer(buffer, size_), remote_endpoint);

    ///std::cout << "Sent size " << sent << std::endl;
}
