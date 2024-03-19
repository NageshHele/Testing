#ifndef PROCESSFEED_HPP
#define PROCESSFEED_HPP

#include <boost/asio/io_service.hpp>
//#include "io_service.hpp"
#include "Structures.hpp"
#include "UdpSokcet.hpp"
#include "../xcd/zlib.h"

#include <map>
#include <set>
#include <vector>
#include <unordered_map>
#include <boost/function.hpp>

#define MARKETBROADCAST 2020
#define OPENINTEREST    2015

typedef std::set<int> TokenSetT;
typedef std::map<int64_t, int64_t> OpenInterestT;
typedef std::vector<int> foSymbolListT;

typedef std::unordered_map<int/** file discriptor */, float/**Multiplier**/> MultiplierHolderT;

typedef struct
{
        int VolumeTradedToday;
        char Time[30];
}TimeT;

typedef std::map<uint64_t, TimeT> TotalVolumeMapT;

class ProcessFeed
{
    public:
        ProcessFeed(boost::asio::io_service& io);
        virtual ~ProcessFeed();

        void ProcessData(char *buffer, int len);
        void ProcessDataWithLogs(char *buffer, int len);

        void FakeData();

    protected:

        OpenInterestT OpenInterest;

    private:

        UdpSokcet *udp;

        bool GetCompressedPrice(const char *buffer, const int &BasePrice, int &GetValue, int &increaseSize);
        void GetCompressedQtyAndOrders(const char *buffer, const int BaseQty, int &GetValue, int &increaseSize);
        void GetDepth(const char *buffer, DataPacketT &DATA, int &increaseSize);
        int RoundOffGap(const int &val, const int &tick);
};

#endif // PROCESSFEED_HPP
