#include "../include/ProcessFeed.hpp"
#include "../include/Setting.hpp"
#include "../include/XceedZipAPI.h"

#include <iostream>

ProcessFeed::ProcessFeed(boost::asio::io_service& io)
{
    udp = new UdpSokcet(io, Setting::getInstance()->getValue<std::string>("BROADCAST.IPADDRESS"), Setting::getInstance()->getValue<int>("BROADCAST.PORT"));
}

ProcessFeed::~ProcessFeed()
{
    //dtor
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
    std::string CompressedLengthStr = "";
    memcpy(CompressedLengthStr.c_str(), buffer, 5);

    int CompressedLength = atoi(CompressedLengthStr.c_str());
    char UncompressedData[16538];
    int UnCompressedDataLength = 0;
    memset(UncompressedData, 0, 16538);

    int Result = 0;
    Result = uncompress(UncompressedData, UnCompressedDataLength, buffer+5, CompressedLength);
    //std::cout << "Result " << Result << " CompressedLength " << CompressedLength << " UnCompressedLength " << UnCompressedDataLength << std::endl;
    /**

    Routine for Uncompressing data...
    Uncompress(UncompressedData, buffer+5, UncompressedDataLength);

    **/
    int increaseSize = 0;
    for(int i=0; increaseSize < UnCompressedDataLength; i++)
    {
        CompressedLengthStr = "";
        memcpy(CompressedLengthStr.c_str(), UncompressedData+increaseSize, 5);
        increaseSize += 5;
        int MessageLength = atoi(CompressedLengthStr.c_str());

        char MsgArray[MessageLength];
        memcpy(MsgArray, UncompressedData+increaseSize, MessageLength);
        increaseSize += MessageLength;

        MessageHeaderT MessageHeader;
        memset(&MessageHeader, 0, sizeof(MessageHeaderT));
        memcpy(&MessageHeader, MsgArray, sizeof(MessageHeaderT));

        //std::cout << "MessageLength " << MessageLength << std::endl
        //<< "MsgCode " << MessageHeader.MsgCode << " " << ntohl(MessageHeader.MsgCode) << std::endl;

        switch(MessageHeader.MsgCode)
        {
        case MARKET_PICTURE:
        {
            std::ostringstream values;

            MarketPictureT MarketPicture;
            memset(&MarketPicture, 0, sizeof(MarketPictureT));
            memcpy(&MarketPicture, MsgArray+sizeof(MessageHeaderT), sizeof(MarketPictureT));

            values << MarketPicture.InstrumentIdetifier << "|";
            values << MarketPicture.Open << "|";
            values << MarketPicture.High << "|";
            values << MarketPicture.Low << "|";
            values << MarketPicture.Close << "|";
            values << MarketPicture.LTP << "|";
            values << MarketPicture.LTQ << "|";
            values << MarketPicture.LTT +"|";
            values << "t|"; ///trend
            values << "0" << "|";  ///Volume
            values << "0" << "|";   ///Value
            values << MarketPicture.TotalBuyQty << "|";
            values << MarketPicture.TotalSellQty << "|";
            values << MarketPicture.LowestPriceEver << "|";
            values << MarketPicture.HighestPriceEver << "|";
            values << MarketPicture.AvgTradedPrice << "|";
            values << MarketPicture.TotalTradedQty << "|";
            values << MarketPicture.IndexFlag << "|";
            values << MarketPicture.OpenInterestDetails.CurrentOpenInterest << "|";

            ///for ( int m = 0; m < No_of_Price_points; ++m )
            for ( int m = 0; m < 5; ++m )
            {
                values << MarketPicture.Buy[m].Price << "|";
                values << MarketPicture.Buy[m].Qty << "|";
                values << MarketPicture.Buy[m].Orders << "|";
            }

            ///for ( int m = 0; m < No_of_Price_points; ++m )
            for ( int m = 0; m < 5; ++m )
            {
                values << MarketPicture.Sell[m].Price << "|";
                values << MarketPicture.Sell[m].Qty << "|";
                values << MarketPicture.Sell[m].Orders << "|";
            }

            values << "*";

            //std::cout << values.str() << std::endl;

            udp->Send(values.str().c_str(), values.str().length());

            break;
        }
        }
    }
}

void ProcessFeed::ProcessDataWithLogs(char *buffer, int len)
{
    std::cout << "Len " << len << " msg :: " << buffer << std::endl;
    std::string CompressedLengthStr = "";
    memcpy(CompressedLengthStr.c_str(), buffer, 5);

    int CompressedLength = atoi(CompressedLengthStr.c_str());
    char UncompressedData[16538];
    int UnCompressedDataLength = 0;
    memset(UncompressedData, 0, 16538);

    std::cout << "Compressed length " << CompressedLengthStr << " " << CompressedLength << std::endl;
/**
    int Result = 0;

    Result = uncompress(UncompressedData, UnCompressedDataLength, buffer+5, CompressedLength);
    std::cout << "Result " << Result << " CompressedLength " << CompressedLength << " UnCompressedLength " << UnCompressedDataLength << std::endl;
    std::cout << "Data " << UncompressedData << std::endl;
**/

    HXCEEDCMP m_hXceedCmp = NULL;
    m_hXceedCmp = XcCreateXceedCompressionA("fgbjdsfjbdfjb");
    if(m_hXceedCmp == NULL)
    {
        std::cout << "Error initializing compression routine" << std::endl;
        return;
    }

    BYTE* m_pCompDecompBuffer = XzAlloc(10000);
    if(m_pCompDecompBuffer == NULL)
    {
        std::cout << "Error allocating size to comp decomp buffer" << std::endl;
        return;
    }

    BYTE* m_pTempBuffer = XzAlloc(10000);
    if(m_pTempBuffer == NULL)
    {
        std::cout << "Error allocating size to temp buffer" << std::endl;
        return;
    }

    memcpy(m_pCompDecompBuffer, buffer + 5, CompressedLength);

    int InStatus = XcUncompress(m_hXceedCmp, m_pCompDecompBuffer, CompressedLength, (BYTE**)&m_pTempBuffer, UnCompressedDataLength, TRUE);
    if(InStatus != 0)
    {
        std::cout << "Error in decompression" << std::endl;
        char m_cErrorBuffer[10000];
        XcGetErrorDescription(m_hXceedCmp, InStatus, m_cErrorBuffer,10000);
        return;
    }

    memcpy(UncompressedData, m_pTempBuffer, UnCompressedDataLength);

    /**

    Routine for Uncompressing data...
    Uncompress(UncompressedData, buffer+5, UncompressedDataLength);

    **/
    int increaseSize = 0;
    for(int i=0; increaseSize < UnCompressedDataLength; i++)
    {
        CompressedLengthStr = "";
        memcpy(CompressedLengthStr.c_str(), UncompressedData+increaseSize, 5);
        increaseSize += 5;
        int MessageLength = atoi(CompressedLengthStr.c_str());

        char MsgArray[MessageLength];
        memcpy(MsgArray, UncompressedData+increaseSize, MessageLength);
        increaseSize += MessageLength;

        MessageHeaderT MessageHeader;
        memset(&MessageHeader, 0, sizeof(MessageHeaderT));
        memcpy(&MessageHeader, MsgArray, sizeof(MessageHeaderT));

        std::cout << "MessageLength " << MessageLength << std::endl
        << "MsgCode " << MessageHeader.MsgCode << " " << ntohl(MessageHeader.MsgCode) << std::endl;

        switch(MessageHeader.MsgCode)
        {
        case MARKET_PICTURE:
        {
            std::ostringstream values;

            MarketPictureT MarketPicture;
            memset(&MarketPicture, 0, sizeof(MarketPictureT));
            memcpy(&MarketPicture, MsgArray+sizeof(MessageHeaderT), sizeof(MarketPictureT));

            values << MarketPicture.InstrumentIdetifier << "|";
            values << MarketPicture.Open << "|";
            values << MarketPicture.High << "|";
            values << MarketPicture.Low << "|";
            values << MarketPicture.Close << "|";
            values << MarketPicture.LTP << "|";
            values << MarketPicture.LTQ << "|";
            values << MarketPicture.LTT +"|";
            values << "t|"; ///trend
            values << "0" << "|";  ///Volume
            values << "0" << "|";   ///Value
            values << MarketPicture.TotalBuyQty << "|";
            values << MarketPicture.TotalSellQty << "|";
            values << MarketPicture.LowestPriceEver << "|";
            values << MarketPicture.HighestPriceEver << "|";
            values << MarketPicture.AvgTradedPrice << "|";
            values << MarketPicture.TotalTradedQty << "|";
            values << MarketPicture.IndexFlag << "|";
            values << MarketPicture.OpenInterestDetails.CurrentOpenInterest << "|";

            ///for ( int m = 0; m < No_of_Price_points; ++m )
            for ( int m = 0; m < 5; ++m )
            {
                values << MarketPicture.Buy[m].Price << "|";
                values << MarketPicture.Buy[m].Qty << "|";
                values << MarketPicture.Buy[m].Orders << "|";
            }

            ///for ( int m = 0; m < No_of_Price_points; ++m )
            for ( int m = 0; m < 5; ++m )
            {
                values << MarketPicture.Sell[m].Price << "|";
                values << MarketPicture.Sell[m].Qty << "|";
                values << MarketPicture.Sell[m].Orders << "|";
            }

            values << "*";

            std::cout << values.str() << std::endl;

            udp->Send(values.str().c_str(), values.str().length());

            break;
        }
        }
    }
}

/**
void ProcessFeed::ProcessDataWithLogs(char *buffer, int len)
{
    std::cout << __LINE__ << " " << __PRETTY_FUNCTION__ << " len " << len << std::endl;


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
**/
