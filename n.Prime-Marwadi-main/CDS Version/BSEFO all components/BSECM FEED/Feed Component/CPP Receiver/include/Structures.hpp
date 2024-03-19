#ifndef STRUCTURES_HPP
#define STRUCTURES_HPP

#define MARKET_PICTURE  31038

#pragma pack(push, 1)

struct MessageHeaderT
{
    int ExchangeTimeStamp;
    short MsgCode;
    short Reserved1;
    int Reserved2;
    short MsgSize;
    short Reserved3;
};

struct OrderByPriceT
{
    int Qty;
    int Price;
    int Orders;
    short Reserved;
};

struct OpenInterestDetailsT
{
    int CurrentOpenInterest;
};

struct MarketPictureT
{
    MessageHeaderT MessageHeader;
    char IndicativeOpenPriceVolume;
    int InstrumentIdetifier;
    char Reserved1;
    char IndexFlag;
    int TotalTradedQty;
    int LTP;
    int LTQ;
    int LTT;
    int AvgTradedPrice;
    OrderByPriceT Buy[5];
    OrderByPriceT Sell[5];
    double TotalBuyQty;
    double TotalSellQty;
    char Reserved2[2];
    int Close;
    int Open;
    int High;
    int Low;
    short Reserved3;
    OpenInterestDetailsT OpenInterestDetails;
    int TotalTrades;
    int HighestPriceEver;
    int LowestPriceEver;
    double TotalTradedValue;

};

#pragma pack(pop)

#pragma pack(push, 1)

typedef struct
{
    int Price;
    int Qty;
    int Orders;
}DepthT;

typedef struct
{
    int Token;
    int Open;
    int High;
    int Low;
    int Close;
    int Previous_Close;
    int LTP;
    int LTQ;
    char LTP_Hour;
    char LTP_Minute;
    char LTP_Second;
    char LTP_MilliSecond[3];
    char Trend;
    int Volume;
    int Value;
    char TradedValueFlag;
    long TimeStamp;
    int TotalBidQty;
    int TotalAskQty;
    int LowDPR;
    int HighDPR;
    int AvgPrice;

    DepthT Bid[5];
    DepthT Ask[5];

}DataPacketT;

#pragma pack(pop)

#endif // STRUCTURES_HPP
