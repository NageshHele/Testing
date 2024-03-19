#ifndef EPOLLSOCKET_HPP
#define EPOLLSOCKET_HPP

#include "ProcessFeed.hpp"

class EpollSocket : public ProcessFeed
{
    public:
        EpollSocket(boost::asio::io_service& io);
        virtual ~EpollSocket();

        void BindIncreamental();

    protected:

        int prepare_mc_socket(int StreamId, std::string LanIp, std::string McastIp, int Port);

        void ConstructBinding(HANDLE epollfd, int StreamId, std::string LanIp, std::string McastIp, int Port);

    private:
};

#endif // EPOLLSOCKET_HPP
