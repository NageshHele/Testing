#include "../include/ProcessFeed.hpp"
#include "../include/Setting.hpp"

#include <iostream>

ProcessFeed::ProcessFeed(boost::asio::io_service& io)
{
    udp = new UdpSokcet(io, Setting::getInstance()->getValue<std::string>("BROADCAST.IPADDRESS"), Setting::getInstance()->getValue<int>("BROADCAST.PORT"));
}

ProcessFeed::~ProcessFeed()
{
    //dtor
}

template<typename T>
void GetCompressedValue(const char *buffer, const int &BaseValue, T &GetValue, int &increaseSize)
{
    short LocalValue = 0;

    memcpy(&LocalValue, buffer+increaseSize, 2);
    LocalValue = ntohs(LocalValue);
    increaseSize +=2;

    if(LocalValue == 32767)
    {
        memcpy(&GetValue, buffer+increaseSize, 2);
        GetValue = ntohl(GetValue);
        increaseSize +=4;
    }
    else
    {
        GetValue = LocalValue+BaseValue;
    }
}

bool ProcessFeed::GetCompressedPrice(const char *buffer, const int &BasePrice, int &GetValue, int &increaseSize)
{
    short LocalValue = 0;
    GetValue = 0;

    memcpy(&LocalValue, buffer+increaseSize, 2);
    LocalValue = ntohs(LocalValue);
    increaseSize += 2;

    if(LocalValue == 32767)
    {
        memcpy(&GetValue, buffer+increaseSize, 2);
        GetValue  = ntohl(GetValue);
        increaseSize += 4;
    }
    else if((LocalValue == 32766) || (LocalValue == -32766))
    {
        return false;
    }
    else
    {
        GetValue = LocalValue+BasePrice;
    }

    return true;
}

void ProcessFeed::GetCompressedQtyAndOrders(const char *buffer, const int BaseQty, int &GetValue, int &increaseSize)
{
    short LocalValue = 0;
    GetValue = 0;

    memcpy(&LocalValue, buffer+increaseSize, 2);
    LocalValue = ntohs(LocalValue);
    increaseSize += 2;

    if(LocalValue == 32767)
    {
        memcpy(&GetValue, buffer+increaseSize, 4);
        GetValue = ntohl(GetValue);
        increaseSize += 4;
    }
    else
    {
        GetValue = LocalValue+BaseQty;
    }
}

void ProcessFeed::GetDepth(const char *buffer, DataPacketT &DATA, int &increaseSize)
{
    for(int i = 0; i < 5; i++)
    {
        if(i == 0)
        {
            if(GetCompressedPrice(buffer, DATA.LTP, DATA.Bid[i].Price, increaseSize))
            {
                GetCompressedQtyAndOrders(buffer, DATA.LTQ, DATA.Bid[i].Qty, increaseSize);
                GetCompressedQtyAndOrders(buffer, DATA.LTQ, DATA.Bid[i].Orders, increaseSize);
                increaseSize += 2;
            }
            else
                break;
        }
        else
        {
            if(GetCompressedPrice(buffer, DATA.LTP, DATA.Bid[i-1].Price, increaseSize))
            {
                GetCompressedQtyAndOrders(buffer, DATA.LTQ, DATA.Bid[i-1].Qty, increaseSize);
                GetCompressedQtyAndOrders(buffer, DATA.LTQ, DATA.Bid[i-1].Orders, increaseSize);
                increaseSize += 2;
            }
            else
                break;
        }
    }

    for(int i = 0; i < 5; i++)
    {
        if(i == 0)
        {
            if(GetCompressedPrice(buffer, DATA.LTP, DATA.Ask[i].Price, increaseSize))
            {
                GetCompressedQtyAndOrders(buffer, DATA.LTQ, DATA.Ask[i].Qty, increaseSize);
                GetCompressedQtyAndOrders(buffer, DATA.LTQ, DATA.Ask[i].Orders, increaseSize);
                increaseSize += 2;
            }
            else
                break;
        }
        else
        {
            if(GetCompressedPrice(buffer, DATA.LTP, DATA.Ask[i-1].Price, increaseSize))
            {
                GetCompressedQtyAndOrders(buffer, DATA.LTQ, DATA.Ask[i-1].Qty, increaseSize);
                GetCompressedQtyAndOrders(buffer, DATA.LTQ, DATA.Ask[i-1].Orders, increaseSize);
                increaseSize += 2;
            }
            else
                break;
        }
    }
}

int ProcessFeed::RoundOffGap(const int &val, const int &tick)
{
    int RetVal = 0;
    int ExtraVal = val % tick;
    if(ExtraVal >= tick / 2)
    {
        RetVal = val - ExtraVal + tick;
    }
    else
    {
        RetVal = val - ExtraVal;
    }
    return RetVal;
}

void ProcessFeed::FakeData()
{
    DataPacketT DataPacket;

    memset(&DataPacket, 0, sizeof(DataPacketT));

    std::string str = "";

    for(int i = 1; i < 50000; i++)
    {
        DataPacket.Token = (i);
        DataPacket.Volume = (i+10);
        DataPacket.Value = (i+200);
        DataPacket.Trend = 'a';
        DataPacket.TradedValueFlag = 'b';
        DataPacket.LTP = (i+201);
        DataPacket.LTQ = (i+11);
        DataPacket.LTP_Hour = 'c';
        DataPacket.LTP_Minute = 'd';
        DataPacket.LTP_Second = 'e';
        DataPacket.Close = (i+202);

        memcpy(DataPacket.LTP_MilliSecond, "fgh", 3);

        ///DataPacket.TimeStamp =

        DataPacket.Open = (i+203);
        DataPacket.Previous_Close = (i+204);
        DataPacket.High = (i+210);
        DataPacket.Low = (i+199);


        DataPacket.TotalBidQty = (i+300);
        DataPacket.TotalAskQty = (i+301);
        DataPacket.LowDPR = (i+190);
        DataPacket.HighDPR = (i+215);
        DataPacket.AvgPrice = (i+205);

        for(int j=0; j<5; j++)
        {
            DataPacket.Bid[j].Price = (i+201+j);
            DataPacket.Bid[j].Qty = (i+11+j);
            DataPacket.Bid[j].Orders = (j+20);

            DataPacket.Ask[j].Price = (i+210+j);
            DataPacket.Ask[j].Qty = (i+12+j);
            DataPacket.Ask[j].Orders = (j+19);
        }


        str += std::to_string(i) + "|";
        str += std::to_string(i+203) + "|";
        str += std::to_string(i+210) + "|";
        str += std::to_string(i+199) + "|";
        str += std::to_string(i+202) + "|";
        str += std::to_string(i+204) + "|";
        str += std::to_string(i+201) + "|";
        str += std::to_string(i+11) + "|";
        str += "c|";
        str += "d|";
        str += "e|";
        str += "fgh|";
        str += "a|";
        str += std::to_string(i+10) + "|";
        str += std::to_string(i+200) + "|";
        str += "b|";
        str += std::to_string(i+800) + "|";
        str += std::to_string(i+300) + "|";
        str += std::to_string(i+301) + "|";
        str += std::to_string(i+190) + "|";
        str += std::to_string(i+215) + "|";
        str += std::to_string(i+205) + "|";

        for(int j=0; j<5; j++)
        {
            str += std::to_string(i+201+j) + "|";
            str += std::to_string(i+11+j) + "|";
            str += std::to_string(20+j) + "|";
        }

        for(int j=0; j<5; j++)
        {
            str += std::to_string(i+210+j) + "|";
            str += std::to_string(i+12+j) + "|";
            str += std::to_string(19+j) + "|";
        }

        str += "*";

        ///std::cout << str;

        ///DataPacket.TimeStamp =

        if(1)
        {

            std::cout
                    << "Token " << DataPacket.Token << std::endl
                    << "LTP " << DataPacket.LTP << std::endl
                    << "LTQ " << DataPacket.LTQ << std::endl
                    << "Volume " << DataPacket.Volume << std::endl
                    << "Value " << DataPacket.Value << std::endl
                    << "TotalBidQty " << DataPacket.TotalBidQty << std::endl
                    << "TotalAskQty " << DataPacket.TotalAskQty << std::endl
                    << "HighDPR " << DataPacket.HighDPR << std::endl
                    << "LowDPR " << DataPacket.LowDPR << std::endl
                    << "WeightedAveragePrice " << DataPacket.AvgPrice << std::endl
                    << "Open " << DataPacket.Open << std::endl
                    << "High " << DataPacket.High << std::endl
                    << "Low " << DataPacket.Low << std::endl
                    << "Close " << DataPacket.Close << std::endl
                    << "PreviousClose " << DataPacket.Previous_Close << std::endl
                    << "TimeStamp " << DataPacket.TimeStamp << std::endl
                    << std::endl;

            std::cout
                    << " ---- Best 5 Depth ----" << std::endl
                    << "Bid.Orders  Bid.Quantity  Bid.Price  Ask.Price  Ask.Quantity  Ask.Orders"
                    << std::endl;

            for(int j = 0; j < 5; j++)
            {
                std::cout
                        << DataPacket.Bid[j].Orders << "        "
                        << DataPacket.Bid[j].Qty << "        "
                        << DataPacket.Bid[j].Price << "          "
                        << DataPacket.Ask[j].Price << "         "
                        << DataPacket.Ask[j].Qty << "        "
                        << DataPacket.Ask[j].Orders << "         "
                        << std::endl << std::endl;
            }
        }

        ///udp->Send((char*)&DataPacket, sizeof(DataPacketT));
        udp->Send(str.c_str(), str.length());

        str = "";

        sleep(1);
    }


    FakeData();
}

void ProcessFeed::ProcessData(char *buffer, int len)
{
    int increaseSize = 0;

    MARKET_PICTURE1 Mkt_Picture1;
    memset(&Mkt_Picture1, 0, sizeof(MARKET_PICTURE1));
    memcpy(&Mkt_Picture1, buffer, sizeof(MARKET_PICTURE1));

    increaseSize += sizeof(MARKET_PICTURE1);

    switch(ntohl(Mkt_Picture1.MsgType))
    {
    case MARKETBROADCAST:
    {
        SkipDataT SkipData;
        memset(&SkipData, 0, sizeof(SkipDataT));

        MARKET_PICTURE2 Mkt_Picture2;
        DataPacketT DataPacket;

        for(int i = 0; ntohs(Mkt_Picture1.NoOfRecords); i++)
        {
            memset(&DataPacket, 0, sizeof(DataPacketT));
            memset(&Mkt_Picture2, 0, sizeof(MARKET_PICTURE2));
            memcpy(&Mkt_Picture2, buffer+increaseSize, sizeof(MARKET_PICTURE2));
            increaseSize += sizeof(MARKET_PICTURE1);

            DataPacket.Token = ntohl(Mkt_Picture2.InstCode);
            DataPacket.Volume = ntohl(Mkt_Picture2.TotalTradedQty);
            DataPacket.Value = ntohl(Mkt_Picture2.TotalTradedValue);
            DataPacket.Trend = Mkt_Picture2.Trend;
            DataPacket.TradedValueFlag = Mkt_Picture2.TradedValueFlag;
            DataPacket.LTP = ntohl(Mkt_Picture2.LTP);
            DataPacket.LTQ = ntohl(Mkt_Picture2.LTQ);
            DataPacket.LTP_Hour = Mkt_Picture2.LTP_Hour;
            DataPacket.LTP_Minute = Mkt_Picture2.LTP_Minute;
            DataPacket.LTP_Second = Mkt_Picture2.LTP_Second;
            DataPacket.Close = ntohl(Mkt_Picture2.CloseRate);

            memcpy(DataPacket.LTP_MilliSecond, Mkt_Picture2.LTP_MilliSecond, sizeof(Mkt_Picture2.LTP_MilliSecond));

            ///DataPacket.TimeStamp =

            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.Open, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.Previous_Close, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.High, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.Low, increaseSize);

            GetCompressedValue(buffer, DataPacket.LTP, SkipData.Reserved, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, SkipData.EP, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTQ, SkipData.EQ, increaseSize);

            GetCompressedValue(buffer, DataPacket.LTQ, DataPacket.TotalBidQty, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTQ, DataPacket.TotalAskQty, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.LowDPR, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.HighDPR, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.AvgPrice, increaseSize);

            GetDepth(buffer, DataPacket, increaseSize);

            if((DataPacket.Bid[0].Price < 0)
                    || (DataPacket.Ask[0].Price < 0)
                    || (DataPacket.LowDPR < 0)
                    || (DataPacket.HighDPR < 0)
                    || (DataPacket.LTP < 0)
                    || (DataPacket.Bid[0].Price < DataPacket.LowDPR)
                    || (DataPacket.Bid[0].Price > DataPacket.HighDPR)
                    || (DataPacket.Ask[0].Price < DataPacket.LowDPR)
                    || (DataPacket.Ask[0].Price > DataPacket.HighDPR))
                continue;

            udp->Send((char*)&DataPacket, sizeof(DataPacketT));
        }

        break;
    }
    }
}

void ProcessFeed::ProcessDataWithLogs(char *buffer, int len)
{
    std::cout << __LINE__ << " " << __PRETTY_FUNCTION__ << " len " << len << std::endl;

    int increaseSize = 0;

    MARKET_PICTURE1 Mkt_Picture1;
    memset(&Mkt_Picture1, 0, sizeof(MARKET_PICTURE1));
    memcpy(&Mkt_Picture1, buffer, sizeof(MARKET_PICTURE1));

    increaseSize += sizeof(MARKET_PICTURE1);

    std::cout << "MessageType " << ntohl(Mkt_Picture1.MsgType) << std::endl;

    switch(ntohl(Mkt_Picture1.MsgType))
    {
    case MARKETBROADCAST:
    {
        SkipDataT SkipData;
        memset(&SkipData, 0, sizeof(SkipDataT));

        MARKET_PICTURE2 Mkt_Picture2;
        DataPacketT DataPacket;

        for(int i = 0; ntohs(Mkt_Picture1.NoOfRecords); i++)
        {
            memset(&DataPacket, 0, sizeof(DataPacketT));
            memset(&Mkt_Picture2, 0, sizeof(MARKET_PICTURE2));
            memcpy(&Mkt_Picture2, buffer+increaseSize, sizeof(MARKET_PICTURE2));
            increaseSize += sizeof(MARKET_PICTURE2);

            DataPacket.Token = ntohl(Mkt_Picture2.InstCode);
            DataPacket.Volume = ntohl(Mkt_Picture2.TotalTradedQty);
            DataPacket.Value = ntohl(Mkt_Picture2.TotalTradedValue);
            DataPacket.Trend = Mkt_Picture2.Trend;
            DataPacket.TradedValueFlag = Mkt_Picture2.TradedValueFlag;
            DataPacket.LTP = ntohl(Mkt_Picture2.LTP);
            DataPacket.LTQ = ntohl(Mkt_Picture2.LTQ);
            DataPacket.LTP_Hour = Mkt_Picture2.LTP_Hour;
            DataPacket.LTP_Minute = Mkt_Picture2.LTP_Minute;
            DataPacket.LTP_Second = Mkt_Picture2.LTP_Second;
            DataPacket.Close = ntohl(Mkt_Picture2.CloseRate);

            memcpy(DataPacket.LTP_MilliSecond, Mkt_Picture2.LTP_MilliSecond, sizeof(Mkt_Picture2.LTP_MilliSecond));

            ///DataPacket.TimeStamp =

            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.Open, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.Previous_Close, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.High, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.Low, increaseSize);

            GetCompressedValue(buffer, DataPacket.LTP, SkipData.Reserved, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, SkipData.EP, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTQ, SkipData.EQ, increaseSize);

            GetCompressedValue(buffer, DataPacket.LTQ, DataPacket.TotalBidQty, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTQ, DataPacket.TotalAskQty, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.LowDPR, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.HighDPR, increaseSize);
            GetCompressedValue(buffer, DataPacket.LTP, DataPacket.AvgPrice, increaseSize);

            GetDepth(buffer, DataPacket, increaseSize);

            if((DataPacket.Bid[0].Price < 0)
                    || (DataPacket.Ask[0].Price < 0)
                    || (DataPacket.LowDPR < 0)
                    || (DataPacket.HighDPR < 0)
                    || (DataPacket.LTP < 0)
                    || (DataPacket.Bid[0].Price < DataPacket.LowDPR)
                    || (DataPacket.Bid[0].Price > DataPacket.HighDPR)
                    || (DataPacket.Ask[0].Price < DataPacket.LowDPR)
                    || (DataPacket.Ask[0].Price > DataPacket.HighDPR))
                continue;

            udp->Send((char*)&DataPacket, sizeof(DataPacketT));

            std::cout
                    << "Token " << DataPacket.Token << std::endl
                    << "LTP " << DataPacket.LTP << std::endl
                    << "LTQ " << DataPacket.LTQ << std::endl
                    << "Volume " << DataPacket.Volume << std::endl
                    << "Value " << DataPacket.Value << std::endl
                    << "TotalBidQty " << DataPacket.TotalBidQty << std::endl
                    << "TotalAskQty " << DataPacket.TotalAskQty << std::endl
                    << "HighDPR " << DataPacket.HighDPR << std::endl
                    << "LowDPR " << DataPacket.LowDPR << std::endl
                    << "WeightedAveragePrice " << DataPacket.AvgPrice << std::endl
                    << "Open " << DataPacket.Open << std::endl
                    << "High " << DataPacket.High << std::endl
                    << "Low " << DataPacket.Low << std::endl
                    << "Close " << DataPacket.Close << std::endl
                    << "PreviousClose " << DataPacket.Previous_Close << std::endl
                    << "TimeStamp " << DataPacket.TimeStamp << std::endl
                    << std::endl;

            std::cout
                    << " ---- Best 5 Depth ----" << std::endl
                    << "Bid.Orders     Bid.Quantity     Bid.Price     Ask.Price     Ask.Quantity     Ask.Orders"
                    << std::endl;

            for(int i = 0; i < 5; i++)
            {
                std::cout
                        << DataPacket.Bid[i].Orders << "  "
                        << DataPacket.Bid[i].Qty << "  "
                        << DataPacket.Bid[i].Price << "  "
                        << DataPacket.Ask[i].Price << "  "
                        << DataPacket.Ask[i].Qty << "  "
                        << DataPacket.Ask[i].Orders << "  "
                        << std::endl << std::endl;
            }
        }

        break;
    }
    default:
        {
            std::cout << __LINE__ << " " << __PRETTY_FUNCTION__ << " Unhandled MessageType " << ntohl(Mkt_Picture1.MsgType) << std::endl;
            break;
        }
    }
}

void ProcessFeed::ProcessIMLData(char* buffer, std::size_t len)
{
   // std::string str = "";



    char part[12];
    memcpy ( part, buffer, 12 );

    IML::HEARTBEAT *Header;
    Header = ( IML::HEARTBEAT * ) ( part );

    char subpart[Header->Msg_Len];
    memcpy ( subpart, ( buffer + 8 ), Header->Msg_Len );

    std::cout << __LINE__ << " " << __PRETTY_FUNCTION__ << " len " << Header->Msg_Len << " Msgtype " << Header->Mst_Type << " " << ntohl ( Header->Mst_Type ) << std::endl;

    switch ( Header->Mst_Type )
    {
    case MARKETBROADCAST:
    {
        IML::MARKET_PICTURE_BROADCAST *MPB = new IML::MARKET_PICTURE_BROADCAST;

        MPB = ( IML::MARKET_PICTURE_BROADCAST * ) ( subpart );

        for ( int i = 0; i < MPB->Num_Records; ++i )
        {
            std::ostringstream values;

            values << MPB->PIC[i].Instrument_Code << "|";
            values << MPB->PIC[i].Open_Rate << "|";
            values << MPB->PIC[i].High_Rate << "|";
            values << MPB->PIC[i].Low_Rate << "|";
            values << MPB->PIC[i].Close_Rate << "|";
            values << MPB->PIC[i].Previous_Close_Rate << "|";
            values << MPB->PIC[i].LTP << "|";
            values << MPB->PIC[i].Last_Trade_Qty << "|";
            values << MPB->PIC[i].LTP_Hour +"|";
            values << MPB->PIC[i].LTP_Minute +"|";
            values << MPB->PIC[i].LTP_Second + "|";
            values << MPB->PIC[i].LTP_Millisecond << "|";
            values << "t|"; ///trend
            values << MPB->PIC[i].Volume << "|";
            values << MPB->PIC[i].Value << "|";
            values << MPB->PIC[i].Trade_Value_Flag + "|";
            values << MPB->PIC[i].Timestamp << "|";
            values << MPB->PIC[i].Total_Bid_Qty << "|";
            values << MPB->PIC[i].Total_Offer_Qty << "|";
            values << MPB->PIC[i].Lower_Circuit_Limit << "|";
            values << MPB->PIC[i].Upper_Circuit_Limit << "|";
            values << MPB->PIC[i].Weighted_Average << "|";

            ///for ( int m = 0; m < MPB->PIC[i].No_of_Price_points; ++m )
            for ( int m = 0; m < 5; ++m )
            {
                values << MPB->PIC[i].Bid[m].Best_Bid_Rate << "|";
                values << MPB->PIC[i].Bid[m].Total_Bid_Qty << "|";
                values << MPB->PIC[i].Bid[m].No_of_Bid_attheprice_points << "|";
            }

            ///for ( int m = 0; m < MPB->PIC[i].No_of_Price_points; ++m )
            for ( int m = 0; m < 5; ++m )
            {
                values << MPB->PIC[i].Bid[m].Best_Offer_Rate << "|";
                values << MPB->PIC[i].Bid[m].Total_Offer_Qty << "|";
                values << MPB->PIC[i].Bid[m].No_of_Ask_attheprice_point << "|";
            }

            values << "*";

            udp->Send(values.str().c_str(), values.str().length());



            /**
            str += std::to_string(MPB->PIC[i].Instrument_Code) + "|";
            str += std::to_string(MPB->PIC[i].Open_Rate) + "|";
            str += std::to_string(MPB->PIC[i].High_Rate) + "|";
            str += std::to_string(MPB->PIC[i].Low_Rate) + "|";
            str += std::to_string(MPB->PIC[i].Close_Rate) + "|";
            str += std::to_string(MPB->PIC[i].Previous_Close_Rate) + "|";
            str += std::to_string(MPB->PIC[i].LTP) + "|";
            str += std::to_string(MPB->PIC[i].Last_Trade_Qty) + "|";
            str += MPB->PIC[i].LTP_Hour +"|";
            str += MPB->PIC[i].LTP_Minute +"|";
            str += MPB->PIC[i].LTP_Second + "|";
            str += std::string(MPB->PIC[i].LTP_Millisecond) + "|";
            str += "t|"; ///trend
            str += std::to_string(MPB->PIC[i].Volume) + "|";
            str += std::to_string(MPB->PIC[i].Value) + "|";
            str += MPB->PIC[i].Trade_Value_Flag + "|";
            str += std::to_string(MPB->PIC[i].Timestamp) + "|";
            str += std::to_string(MPB->PIC[i].Total_Bid_Qty) + "|";
            str += std::to_string(MPB->PIC[i].Total_Offer_Qty) + "|";
            str += std::to_string(MPB->PIC[i].Lower_Circuit_Limit) + "|";
            str += std::to_string(MPB->PIC[i].Upper_Circuit_Limit) + "|";
            str += std::to_string(MPB->PIC[i].Weighted_Average) + "|";

            for ( int m = 0; m < MPB->PIC[i].No_of_Price_points; ++m )
            {
                str += std::to_string(MPB->PIC[i].Bid[m].Best_Bid_Rate) + "|";
                str += std::to_string(MPB->PIC[i].Bid[m].Total_Bid_Qty) + "|";
                str += std::to_string(MPB->PIC[i].Bid[m].No_of_Bid_attheprice_points) + "|";
            }

            for ( int m = 0; m < MPB->PIC[i].No_of_Price_points; ++m )
            {
                str += std::to_string(MPB->PIC[i].Bid[m].Best_Offer_Rate) + "|";
                str += std::to_string(MPB->PIC[i].Bid[m].Total_Offer_Qty) + "|";
                str += std::to_string(MPB->PIC[i].Bid[m].No_of_Ask_attheprice_point) + "|";
            }

            str += "*";

            udp->Send(str.c_str(), str.length());

            str = "";
**/



            /**
            memset ( &DepthPacket.MIbaseStruct, 0, sizeof ( MIbaseStructT ) );
            DepthPacket.MIbaseStruct.Token = MPB->PIC[i].Instrument_Code;

            DepthPacket.MIbaseStruct.Exchange = 1;
            for ( int m = 0; m < MPB->PIC[i].No_of_Price_points; ++m )
            {
                DepthPacket.MIbaseStruct.SnapshotDetails.Bid[m].Price = MPB->PIC[i].Bid[m].Best_Bid_Rate / Multiplier;
                DepthPacket.MIbaseStruct.SnapshotDetails.Bid[m].Quantity = MPB->PIC[i].Bid[m].Total_Bid_Qty;
                DepthPacket.MIbaseStruct.SnapshotDetails.Bid[m].Orders = MPB->PIC[i].Bid[m].No_of_Bid_attheprice_points;

                DepthPacket.MIbaseStruct.SnapshotDetails.Ask[m].Price = MPB->PIC[i].Bid[m].Best_Offer_Rate / Multiplier;
                DepthPacket.MIbaseStruct.SnapshotDetails.Ask[m].Quantity = MPB->PIC[i].Bid[m].Total_Offer_Qty;
                DepthPacket.MIbaseStruct.SnapshotDetails.Ask[m].Orders = MPB->PIC[i].Bid[m].No_of_Ask_attheprice_point;



            }

            DepthPacket.MIbaseStruct.SnapshotDetails.TotalBuyQuantity = MPB->PIC[i].Total_Bid_Qty;
            DepthPacket.MIbaseStruct.SnapshotDetails.TotalSellQuantity = MPB->PIC[i].Total_Offer_Qty;

            DepthPacket.MIbaseStruct.SnapshotDetails.HighDPR = MPB->PIC[i].Upper_Circuit_Limit / Multiplier;
            DepthPacket.MIbaseStruct.SnapshotDetails.LowDPR = MPB->PIC[i].Lower_Circuit_Limit / Multiplier;

            DepthPacket.MIbaseStruct.SnapshotDetails.Open = MPB->PIC[i].Open_Rate / Multiplier;
            DepthPacket.MIbaseStruct.SnapshotDetails.High = MPB->PIC[i].High_Rate / Multiplier;
            DepthPacket.MIbaseStruct.SnapshotDetails.Low = MPB->PIC[i].Low_Rate / Multiplier;
            DepthPacket.MIbaseStruct.SnapshotDetails.Close = MPB->PIC[i].Previous_Close_Rate / Multiplier;

            DepthPacket.MIbaseStruct.SnapshotDetails.LTP = MPB->PIC[i].LTP / Multiplier;
            DepthPacket.MIbaseStruct.SnapshotDetails.LTQ = MPB->PIC[i].Last_Trade_Qty;
            DepthPacket.MIbaseStruct.SnapshotDetails.Time = std::chrono::system_clock::now().time_since_epoch().count();


            DepthPacket.MIbaseStruct.SnapshotDetails.PercentChange = ( float ) ( ( float ) ( DepthPacket.MIbaseStruct.SnapshotDetails.Close- DepthPacket.MIbaseStruct.SnapshotDetails.LTP ) * 100/ DepthPacket.MIbaseStruct.SnapshotDetails.Close );

            DepthPacket.MIbaseStruct.SnapshotDetails.VolumeTradedToday = MPB->PIC[i].Volume;


            DepthPacket.MIbaseStruct.SnapshotDetails.ATP = MPB->PIC[i].Weighted_Average / Multiplier;

            OpenInterestT::iterator iterator_ = this->OpenInterest.find ( DepthPacket.MIbaseStruct.Token );
            if ( iterator_ != OpenInterest.end() )
            {
                DepthPacket.MIbaseStruct.SnapshotDetails.OpenInterest = iterator_->second;
            }
            else
                DepthPacket.MIbaseStruct.SnapshotDetails.OpenInterest = 0;

            DepthPacket.MIbaseStruct.SnapshotDetails.LowExecBand = DepthPacket.MIbaseStruct.SnapshotDetails.LowDPR ;
            DepthPacket.MIbaseStruct.SnapshotDetails.HighExecBand = DepthPacket.MIbaseStruct.SnapshotDetails.HighDPR ;

            std::string time ( " " );

            time += std::to_string ( MPB->Hour );
            time += ":";
            time += std::to_string ( MPB->Minute );
            time += ":";
            time += std::to_string ( MPB->Second );
            time += ".";
            time += std::to_string ( MPB->Millisecond );
            memcpy ( DepthPacket.MIbaseStruct.SnapshotDetails.LTTime, time.c_str(), time.length() );

            this->Snapshot->Send ( ( char * ) &DepthPacket, sizeof ( DepthPacketT ) );
            **/
            /*
                        std::ofstream Myfile ( name, std::ios::app|std::ios::binary );
                        if ( Myfile.is_open() )
                        {
                             Myfile.write ( (char*)&DepthPacket, sizeof ( DepthPacketT ) );
                        }*/

            break;
        }
    }
    case OPENINTEREST:
    {
        IML::OPEN_INTEREST_BROADCAST *obj = ( IML::OPEN_INTEREST_BROADCAST * ) ( subpart );
        for ( int i = 0; i < obj->Num_Records; ++i )
        {
            OpenInterestT::iterator iterator_ = this->OpenInterest.find ( obj->OpenI[i].Instrument_Id );
            if ( this->OpenInterest.end() != iterator_ )
            {
                iterator_->second = obj->OpenI[i].Open_Interest_Value;
            }
            else
            {
                this->OpenInterest.insert ( OpenInterestT::value_type ( obj->OpenI[i].Instrument_Id, obj->OpenI[i].Open_Interest_Value ) );
            }
        }
        break;
    }
    default:
        break;
    }
}
