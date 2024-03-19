#ifndef STRUCTURES_HPP
#define STRUCTURES_HPP

namespace IML {
#pragma pack(push,4)

    struct HEARTBEAT
    {
        int Slot_Number;
        int Msg_Len;
        int Mst_Type;
    };

    struct TIME_BROADCAST
    {

        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Reserved_Field6;
        char Reserved_Field7;
        char Reserved_Field8;
        char Reserved_Field9[2];

    };

    struct SESSION_CHANGE_BROADCAST
    {

        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Product_ID;
        short Reserved_Field4;
        short Filler;
        short Market_Type;
        short Session_Number;
        int Reserved_Field5;
        char Start_End_Flag;
        char Reserved_Field6;
        char Reserved_Field7[2];

    };

    struct BID_ASK
    {
        int Best_Bid_Rate;
        int Total_Bid_Qty;
        int No_of_Bid_attheprice_points;
        int Implied_Buy_Quantity;
        int Best_Offer_Rate;
        int Total_Offer_Qty;
        int No_of_Ask_attheprice_point;
        int Implied_Sell_Quantity;
    };

    struct MARKET_PICTURE
    {
        int Instrument_Code;
        int Open_Rate;
        int Previous_Close_Rate;
        int High_Rate;
        int Low_Rate;
        int No_of_Trades;
        int Volume;
        int Value;
        int Last_Trade_Qty;
        int LTP;
        int Close_Rate;
        int Block_Deal_Reference_Price;
        int Indicative_Equilibrium_Price;
        int Indicative_Equilibrium_Qty;
        long Timestamp;
        int Total_Bid_Qty;
        int Total_Offer_Qty;
        char Trade_Value_Flag;
        char Filler;
        char Reserved_Field1;
        char Reserved_Field2;
        int Lower_Circuit_Limit;
        int Upper_Circuit_Limit;
        int Weighted_Average;
        short Market_Type;
        short Session_Number;
        char LTP_Hour;
        char LTP_Minute;
        char LTP_Second;
        char LTP_Millisecond[3];
        char Reserved_Field3[2];
        short Reserved_Field4;
        short No_of_Price_points;
        BID_ASK Bid[5];

    };

    struct MARKET_PICTURE_BROADCAST
    {
        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Num_Records;
        MARKET_PICTURE PIC[6];

    };

    struct CLOSE_PRICE
    {
        int Instrument_Code;
        int Price;
        char Reserved_Field1;
        char Traded;
        char Precision_Indicator;
        char Reserved_Field2;
    };

    struct CLOSE_PRICE_BROADCAST
    {
        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Num_Records;
        CLOSE_PRICE CL[80];
    };

    struct INDEXS
    {
        int Index_Code;
        int Index_High;
        int Index_Low;
        int Index_Open;
        int Previous_Index_Close;
        int Index_Value;
        char Index_ID[7];
        char Reserved_Field1;
        char Reserved_Field2;
        char Reserved_Field3;
        char Reserved_Field4[2];
        short Reserved_Field5;
        short Reserved_Field6;
    };

    struct SENSEX_BROADCAST
    {
        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Num_Records;
        INDEXS IDX[24];
    };

    struct ALL_INDICES_BROADCAST
    {
        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Num_Records;
        INDEXS IDX[24];

    };

    struct VAR_EMLVAR
    {
        int Instrument_code;
        int VAR_IM_Percentage;
        int ELM_VAR_Percentage;
        int Reserved_Field1;
        short Reserved_Field2;
        short Reserved_Field3;
        char Reserved_Field4;
        char Identifier;
        char Reserved_Field5[2];
    };

    struct VAR_PERCENTAGE_BROADCAST
    {
        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Num_Records;
        VAR_EMLVAR _var[40];
    };

    struct OPEN_INTEREST
    {
        int Instrument_Id;
        int Open_Interest_Quantity;
        int Open_Interest_Value;
        int Open_Interest_Change;
        char Reserved_Field1[4];
        int Reserved_Field2;
        short Reserved_Field3;
        short Reserved_Field4;
        char Reserved_Field5;
        char Reserved_Field6;
        char Reserved_Field7[2];
    };

    struct OPEN_INTEREST_BROADCAST
    {
        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Num_Records;
        OPEN_INTEREST OpenI[26];

    };

    struct RBI_REFERENCE
    {
        int Underlying_Asset_ID;
        int RBI_Rate;
        short Reserved_Field1;
        short Reserved_Field2;
        char Date[11];
        char Filler[1];
    };

    struct RBI_REFERENCE_RATE
    {
        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Num_Records;
        RBI_REFERENCE RBIRef[6];
    };

    struct NEWS_HEADLINE_BROADCAST
    {
        int Message_Type;
        int Reserved_Field1;
        int Reserved_Field2;
        unsigned short Reserved_Field3;
        short Hour;
        short Minute;
        short Second;
        short Millisecond;
        short Reserved_Field4;
        short Reserved_Field5;
        short Reserved_Field6;
        short News_Category;
        int News_Id;
        char News_Headline[40];
        char Reserved_Field7;
        char Reserved_Field8;
        char Reserved_Field9[2];
    };

#pragma pack(pop)
}

#pragma pack(push, 1)

typedef struct
{
    int Reserved;
    int EP;
    int EQ;
}SkipDataT;

typedef struct
{
    int MsgType;
    int Reserved1;
    int Reserved2;
    unsigned short Reserved3;
    short Hour;
    short Minute;
    short Second;
    short MilliSecond;
    short Reserved4;
    short Reserved5;
    short NoOfRecords;
}MARKET_PICTURE1;

typedef struct
{
    int InstCode;
    int NoOfTrades;
    int TotalTradedQty;
    int TotalTradedValue;
    char TradedValueFlag;
    char Trend;
    char SisLakhFlag;
    char Reserved1;
    short MktType;
    short SessionNumber;

    char LTP_Hour;
    char LTP_Minute;
    char LTP_Second;
    char LTP_MilliSecond[3];

    char Reserved2[2];
    short Reserved3;
    short NoOfPricePoint;
    long TimeStamp;
    int CloseRate;
    int LTQ;
    int LTP;
}MARKET_PICTURE2;

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
