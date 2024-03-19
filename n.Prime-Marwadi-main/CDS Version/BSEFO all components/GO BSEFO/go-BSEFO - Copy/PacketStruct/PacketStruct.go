package PacketStruct

import (
	"encoding/binary"
	"fmt"
	_ "log"
	"net"
	"os"
	"strconv"
	"strings"
	"sync"
	_ "syscall"
	"time"
	_ "unsafe"

	"golang.org/x/net/ipv4"
)

const (
	InitialBroadcastMessageLength = 237
	FinalBroadcastMessageLength   = 261 //

	MaxByteValue         = 0x7F
	MaxUnsignedByteValue = 0xFF
	EndOfBuyBid          = 157
	EndOfSellBid         = 237
	StartOfOpenInterest  = 237
	StartOfVar           = 253
)

type FinalBroadcastStructure struct {
	MessageData [FinalBroadcastMessageLength]byte
}
type BroadcastFinalMessage struct {
	MessageData [FinalBroadcastMessageLength]byte
}

type MessageHeader struct {
	MessageType uint32
}

type Packet2015 struct {
	MessageData [24]byte
}

type Packet2015MessageData struct {
	MessageData [36]byte
}

var (
	sharedMemoryMap               = make(map[uint64]*FinalBroadcastStructure)
	shortValue                    = make([]byte, 4)
	longValue                     = make([]byte, 4)
	longlongValue                 = make([]byte, 8)
	IndexOfOpenInterest           = StartOfOpenInterest
	IndexOfNormalMessage          = 4
	IndexOfVar                    = StartOfVar
	Open                   int32  = 0
	High                   int32  = 0
	Low                    int32  = 0
	Close                  int32  = 0
	TotalBidQuantity       uint32 = 0
	TotalOfferQuantity     uint32 = 0
	UpperCircuit           int32  = 0
	LowerCircuit           int32  = 0
	WeightedAveragePrice   int32  = 0
	BidPrice1              int32  = 0
	BidQuantity1           uint32 = 0
	BidOrders1             int32  = 0
	BidImpliedQuantity1    int32  = 0
	BreakBidLoop                  = 0
	BaseBidPrice           int32  = 0
	BaseBidQuantity        uint32 = 0
	BaseBidOrders          int32  = 0
	BaseBidImpliedQuantity int32  = 0
	BroadCastPipeLine             = make(chan *FinalBroadcastStructure, 20)
	LicenseDayTested              = 0
)

func Sample1() {

	fmt.Println("Hi Welcome To Packet Structure")

}

func CheckLicenseDay(timestamp *uint64) {

	year := 2023
	month := time.September
	day := 30

	t := time.Date(year, month, day, 0, 0, 0, 0, time.UTC)

	// Convert the time object to an int64 timestamp
	LicenseTimeStamp := t.UnixNano()
	fmt.Println("Licensesed Expiry Date :", LicenseTimeStamp)
	fmt.Println("Current Time Stamp :", *timestamp)
	if LicenseTimeStamp >= int64(*timestamp) {

		fmt.Println("License Is Valid....")
	} else {

		fmt.Println("License Is Invalid.... Exiting Program....")
		os.Exit(1)
	}
	LicenseDayTested = 1

}

func Insert2015Value(FinalStrcuture *FinalBroadcastStructure,
	OpenInterestQuantity *uint32, OpenInterestValue *uint64, OpenInterestChange *uint32) {
	IndexOfOpenInterest = StartOfOpenInterest
	binary.BigEndian.PutUint32(longValue, uint32(*OpenInterestQuantity))
	copy(FinalStrcuture.MessageData[IndexOfOpenInterest:IndexOfOpenInterest+4], longValue)
	IndexOfOpenInterest += 4

	binary.BigEndian.PutUint64(longlongValue, uint64(*OpenInterestValue))
	copy(FinalStrcuture.MessageData[IndexOfOpenInterest:IndexOfOpenInterest+8], longlongValue)
	IndexOfOpenInterest += 8
	binary.BigEndian.PutUint32(longValue, uint32(*OpenInterestChange))
	copy(FinalStrcuture.MessageData[IndexOfOpenInterest:IndexOfOpenInterest+4], longValue)

}

func ProcessPacket2015(buffer *[]byte, length *int) uint16 {

	numberOfRecords := uint16(binary.BigEndian.Uint16((*buffer)[26:28]))
	Header2015End := 28
	RemainingSizeOf2015Message := 16

	for i := 0; i < int(numberOfRecords); i++ {

		InstrumentID := uint64(binary.BigEndian.Uint32((*buffer)[Header2015End : Header2015End+4]))
		Header2015End += 4
		OpenInterestQuantity := uint32(binary.BigEndian.Uint32((*buffer)[Header2015End : Header2015End+4]))
		Header2015End += 4
		OpenInterestValue := uint64(binary.BigEndian.Uint64((*buffer)[Header2015End : Header2015End+8]))
		Header2015End += 8
		OpenInterestChange := uint32(binary.BigEndian.Uint32((*buffer)[Header2015End : Header2015End+4]))
		Header2015End += 4
		Header2015End += RemainingSizeOf2015Message
		//fmt.Println("Instrment ID ", InstrumentID)
		//fmt.Println("Open Interest Quantity ", OpenInterestQuantity)
		//fmt.Println("Open Interest Value ", OpenInterestValue)
		//fmt.Println("Open Interest Change ", OpenInterestChange)

		FinalStrcuture := CheckMemeoryExistForInstrumentID(&InstrumentID)
		Insert2015Value(FinalStrcuture, &OpenInterestQuantity, &OpenInterestValue, &OpenInterestChange)
		BroadCastPipeLine <- FinalStrcuture

		/*
			if val, ok := sharedMemoryMap[uint64(InstrumentID)]; ok {
				Insert2015Value(val, &OpenInterestQuantity, &OpenInterestValue, &OpenInterestChange)
				fmt.Println("Instrument ID Exist ")
			} else {
				// Key doesn't exist, create a new value
				newValue := &FinalBroadcastStructure{}

				sharedMemoryMap[uint64(InstrumentID)] = newValue
				Insert2015Value(newValue, &OpenInterestQuantity, &OpenInterestValue, &OpenInterestChange)
				fmt.Println("Instrument ID Created ")
			}
		*/

	}

	//fmt.Println("Number Of Records ", numberOfRecords)

	return numberOfRecords

}

func ProcessPacket2016(buffer *[]byte, length *int) uint16 {

	numberOfRecords := uint16(binary.BigEndian.Uint16((*buffer)[26:28]))
	Header2016End := 28
	RemainingSizeOf2016Message := 12

	for i := 0; i < int(numberOfRecords); i++ {

		InstrumentID := uint64(binary.BigEndian.Uint32((*buffer)[Header2016End : Header2016End+4]))
		Header2016End += 4
		VarPercentage := uint32(binary.BigEndian.Uint32((*buffer)[Header2016End : Header2016End+4]))
		Header2016End += 4
		ELMVarPercentage := uint32(binary.BigEndian.Uint32((*buffer)[Header2016End : Header2016End+4]))
		Header2016End += 4
		Header2016End += RemainingSizeOf2016Message
		//fmt.Println("Instrment ID ", InstrumentID)

		FinalStrcuture := CheckMemeoryExistForInstrumentID(&InstrumentID)

		IndexOfVar = StartOfVar
		binary.BigEndian.PutUint32(longValue, uint32(VarPercentage))
		copy(FinalStrcuture.MessageData[IndexOfVar:IndexOfVar+4], longValue)
		IndexOfVar += 4
		binary.BigEndian.PutUint32(longValue, uint32(ELMVarPercentage))
		copy(FinalStrcuture.MessageData[IndexOfVar:IndexOfVar+4], longValue)
		IndexOfVar += 4
		//Insert2015Value(FinalStrcuture, &OpenInterestQuantity, &OpenInterestValue, &OpenInterestChange)
		BroadCastPipeLine <- FinalStrcuture
		//fmt.Println("Var Percentage ", VarPercentage)
		//fmt.Println("ELM Percentage ", ELMVarPercentage)

		//fmt.Println("Record Finished ", i)

	}

	//fmt.Println("Number Of Records ", numberOfRecords)

	return numberOfRecords

}

func CheckMemeoryExistForInstrumentID(InstrumentID *uint64) *FinalBroadcastStructure {

	if val, ok := sharedMemoryMap[*InstrumentID]; ok {
		return val
	} else {
		// Key doesn't exist, create a new value
		newValue := &FinalBroadcastStructure{}

		sharedMemoryMap[*InstrumentID] = newValue
		return newValue
		//fmt.Println("Instrument ID Created ")
	}
}

func Process2020MarketPicture(buffer *[]byte, length *int, messageType *uint32) uint16 {

	NumberOfRecords := uint16(binary.BigEndian.Uint16((*buffer)[26:28]))

	for i := 0; i < int(NumberOfRecords); i++ {
		start := 28
		End := 4
		IndexOfNormalMessage = 8
		if *messageType == 2021 {
			End = 8
		}
		InstrumentCode := uint64(binary.BigEndian.Uint32((*buffer)[start : start+End]))
		start += End
		FinalStrcuture := CheckMemeoryExistForInstrumentID(&InstrumentCode)
		binary.BigEndian.int32(longlongValue, uint64(InstrumentCode))

		copy(FinalStrcuture.MessageData[0:IndexOfNormalMessage], longlongValue)

		numberOfTrades := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(numberOfTrades))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		tradeVolume := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(tradeVolume))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		tradedValue := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(tradedValue))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4
		//fmt.Println("Value Of Start ", start)
		TradeValueInLacOrCR := string((*buffer)[start : start+1])

		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+1], []byte(TradeValueInLacOrCR))
		IndexOfNormalMessage += 1
		//fmt.Println(" Value In Lakhs Or Crore ", tradeValueInLacOrCR)
		start += 20
		//fmt.Println("Position Of Start Index ", start)
		Timestamp := uint64(binary.BigEndian.Uint64((*buffer)[start : start+8]))
		//fmt.Println("TimeStamp :", Timestamp)
		if LicenseDayTested == 0 {

			CheckLicenseDay(&Timestamp)
		}
		start += 8
		binary.BigEndian.PutUint64(longlongValue, uint64(Timestamp))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+8], longlongValue)
		IndexOfNormalMessage += 8
		//fmt.Println(" Index Of Normal Message upto Timestamp ", IndexOfNormalMessage)

		CloseRate := int32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(CloseRate))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		LTQ := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(LTQ))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		LTP := int32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(LTP))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4
		//fmt.Println("LTP  ", LTP)
		//fmt.Printf("Close Rate %d LTP %d   LTQ %d \n", closeRate, ltp, ltq)
		//fmt.Println(" Index Of Normal Message upto LTP ", IndexOfNormalMessage)

		////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////Decompression Logic Started/////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////Retriving Open Price////////////////////////////////
		if int((*buffer)[start+1])<<8|int((*buffer)[start]) == 32767 {
			//fmt.Println("Got Maximum Byte")
			temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			Open = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(Open))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Println("Got Minimum Byte")
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			Open = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(Open))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Retriving Previous Day Close Price////////////////////////////////

		if int((*buffer)[start+1])<<8|int((*buffer)[start]) == 32767 {
			//fmt.Println("Got Maximum Byte")
			temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			Close = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(Close))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Println("Got Minimum Byte")
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			Close = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(Close))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Retriving High Price////////////////////////////////

		if int((*buffer)[start+1])<<8|int((*buffer)[start]) == 32767 {
			//fmt.Println("Got Maximum Byte")
			temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			High = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(High))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Println("Got Minimum Byte")
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			High = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(High))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}
		////////////////////////////////////////Retriving Low Price////////////////////////////////

		if int((*buffer)[start+1])<<8|int((*buffer)[start]) == 32767 {
			//fmt.Println("Got Maximum Byte")
			temp := int32(binary.BigEndian.Uint32((*buffer)[start : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			Low = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(Low))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Println("Got Minimum Byte")
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			Low = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(Low))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		/*
			////////////////////////////////////////Retriving Reserved(Block Deal)////////////////////////////////

			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32763 {
				fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+4])
				start += 4

			} else {
				fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
				start += 2
			}

			////////////////////////////////////////Equilibrium Price////////////////////////////////

			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
				fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+4])
				start += 4

			} else {
				fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
				start += 2
			}



			////////////////////////////////////////Equilibrium Quantity////////////////////////////////
		*/

		//fmt.Println("Start Index Value ", start)
		//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+20])
		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {

			start += 6

		} else {

			start += 2

		}
		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {

			start += 6

		} else {

			start += 2

		}
		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {

			start += 6

		} else {

			start += 2

		}

		////////////////////////////////////////Retriving Bid Quantity////////////////////////////////
		//fmt.Println("Start Index Value ", start)
		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
			//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start+2:start+4+2])
			temp := uint32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			TotalBidQuantity = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(TotalBidQuantity))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
			temp := uint16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			TotalBidQuantity = LTQ + uint32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(Low))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Total Offer Quantity////////////////////////////////

		if int((*buffer)[start])<<8|int((*buffer)[start]+1) == 32767 {
			//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start+2:start+4+2])
			temp := uint32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			TotalOfferQuantity = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(TotalOfferQuantity))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
			temp := uint16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			TotalOfferQuantity = LTQ + uint32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(TotalOfferQuantity))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Lower Circuit////////////////////////////////

		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
			//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start+2:start+2+4])

			temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			LowerCircuit = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(LowerCircuit))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			LowerCircuit = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(LowerCircuit))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Upper Circuit////////////////////////////////

		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
			//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start+2:start+4+2])
			temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			UpperCircuit = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(UpperCircuit))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Printf("Hexadecimal Representation: %X\n", (*buffer)[start:start+2])
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			UpperCircuit = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(UpperCircuit))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Weighted Average Price////////////////////////////////

		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
			//fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start+2:start+4+2])
			temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
			//fmt.Println("Temp ", temp)
			start += 6
			WeightedAveragePrice = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(WeightedAveragePrice))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			//fmt.Printf("Hexadecimal Representation: %X\n", (*buffer)[start:start+2])
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			WeightedAveragePrice = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(WeightedAveragePrice))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		//fmt.Println(" Index Of Normal Message Before Bid Array ", IndexOfNormalMessage)

		//fmt.Println("Instrument ID ", InstrumentCode)
		//fmt.Println("TimeStamp    ", Timestamp)
		//fmt.Println("LTP  ", LTP)
		//fmt.Println("Open Price ", Open)
		//fmt.Println("Close Price ", Close)
		//fmt.Println("High Price ", High)
		//fmt.Println("Low Price ", Low)
		//fmt.Println("Upper Circuit ", UpperCircuit)
		//fmt.Println("Lower Circuit ", LowerCircuit)
		//fmt.Println("Weighted Average Price ", WeightedAveragePrice)
		//fmt.Println("LTQ ", LTQ)
		//fmt.Println("Total Bid Quantity ", TotalBidQuantity)
		//fmt.Println("Total Ask Quantity ", TotalOfferQuantity)

		///////////////////////////////////////////Extraction Of Bid 1/////////////////////////////////
		//fmt.Println("LTP  ", LTP)
		BaseBidPrice = LTP
		BaseBidQuantity = LTQ
		BaseBidOrders = int32(LTQ)
		BaseBidImpliedQuantity = int32(LTQ)
		BreakBidLoop = 0
		//fmt.Printf("Hexadecimal Representation: %X\n", (*buffer)[start:start+40])
		for k := 0; k < 5; k++ {

			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32766 {
				//fmt.Println("End Of Book Reached")
				BreakBidLoop = 1

			} else if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {

				temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))

				start += 6
				BidPrice1 = temp
				//fmt.Println("Bid Price Changed Maximum")
				binary.BigEndian.PutUint32(longValue, uint32(BidPrice1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			} else {
				temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
				start += 2
				BidPrice1 = BaseBidPrice + int32(temp)
				//fmt.Println("Bid Price Changed Minimum")

				binary.BigEndian.PutUint32(longValue, uint32(BidPrice1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			}
			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32766 {
				//fmt.Println("End Of Book Reached")
				BreakBidLoop = 1

			} else if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
				temp := uint32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
				//fmt.Println("Temp ", temp)
				start += 6
				BidQuantity1 = temp
				//fmt.Println("Open ", Open)
				binary.BigEndian.PutUint32(longValue, uint32(BidQuantity1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			} else {
				temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
				start += 2
				BidQuantity1 = BaseBidQuantity + uint32(temp)
				binary.BigEndian.PutUint32(longValue, uint32(BidQuantity1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			}

			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32766 {
				//fmt.Println("End Of Book Reached")
				BreakBidLoop = 1

			} else if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
				temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
				//fmt.Println("Temp ", temp)
				start += 6
				BidOrders1 = temp
				//fmt.Println("Open ", Open)
				binary.BigEndian.PutUint32(longValue, uint32(BidOrders1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			} else {
				temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
				start += 2
				BidOrders1 = BaseBidOrders + int32(temp)
				binary.BigEndian.PutUint32(longValue, uint32(BidOrders1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			}
			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32766 {
				//fmt.Println("End Of Book Reached")
				BreakBidLoop = 1

			} else if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
				temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
				//fmt.Println("Temp ", temp)
				start += 6
				BidImpliedQuantity1 = temp
				//fmt.Println("Open ", Open)
				binary.BigEndian.PutUint32(longValue, uint32(BidImpliedQuantity1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			} else {
				temp := uint16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
				start += 2
				BidImpliedQuantity1 = BaseBidImpliedQuantity + int32(temp)
				binary.BigEndian.PutUint32(longValue, uint32(BidImpliedQuantity1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			}

			//fmt.Println("Bid Price1 ", BidPrice1)
			//fmt.Println("Bid Quantity1 ", BidQuantity1)
			//fmt.Println("Bid Orders1 ", BidOrders1)
			//fmt.Println("Bid Implied Quantity ", BidImpliedQuantity1)
			BaseBidPrice = BidPrice1
			BaseBidQuantity = BidQuantity1
			BaseBidOrders = BidOrders1
			BaseBidImpliedQuantity = BidImpliedQuantity1

			if BreakBidLoop == 1 {

				//fmt.Println("Breaking Bid Loop As Order Book End Found")
				break
			}
			//break
		}
		//fmt.Println(" Index Of Normal Message After Bid Array ", IndexOfNormalMessage)
		//fmt.Println("End Of Normal Message ", IndexOfNormalMessage)
		if IndexOfNormalMessage < EndOfBuyBid {
			for i := IndexOfNormalMessage; i < EndOfBuyBid; i++ {
				FinalStrcuture.MessageData[i] = 0x00
			}
			IndexOfNormalMessage = EndOfBuyBid

		}

		BaseBidPrice = LTP
		BaseBidQuantity = LTQ
		BaseBidOrders = int32(LTQ)
		BaseBidImpliedQuantity = int32(LTQ)
		BreakBidLoop = 0

		for k := 0; k < 5; k++ {

			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == -32766 {
				//fmt.Println("End Of Book Reached")
				break

			} else if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {

				temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))

				start += 6
				BidPrice1 = temp
				//fmt.Println("Bid Price Changed Maximum")
				binary.BigEndian.PutUint32(longValue, uint32(BidPrice1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			} else {
				temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
				start += 2
				BidPrice1 = BaseBidPrice + int32(temp)
				//fmt.Println("Bid Price Changed Minimum")

				binary.BigEndian.PutUint32(longValue, uint32(BidPrice1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			}
			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == -32766 {
				//fmt.Println("End Of Book Reached")
				break

			} else if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
				temp := uint32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
				//fmt.Println("Temp ", temp)
				start += 6
				BidQuantity1 = temp
				//fmt.Println("Open ", Open)
				binary.BigEndian.PutUint32(longValue, uint32(BidQuantity1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			} else {
				temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
				start += 2
				BidQuantity1 = BaseBidQuantity + uint32(temp)
				binary.BigEndian.PutUint32(longValue, uint32(BidQuantity1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			}

			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == -32766 {
				//fmt.Println("End Of Book Reached")
				break

			} else if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
				temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
				//fmt.Println("Temp ", temp)
				start += 6
				BidOrders1 = temp
				//fmt.Println("Open ", Open)
				binary.BigEndian.PutUint32(longValue, uint32(BidOrders1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			} else {
				temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
				start += 2
				BidOrders1 = BaseBidOrders + int32(temp)
				binary.BigEndian.PutUint32(longValue, uint32(BidOrders1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			}
			if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32766 {
				//fmt.Println("End Of Book Reached")
				break

			} else if int((*buffer)[start])<<8|int((*buffer)[start+1]) == -32767 {
				temp := int32(binary.BigEndian.Uint32((*buffer)[start+2 : start+4+2]))
				//fmt.Println("Temp ", temp)
				start += 6
				BidImpliedQuantity1 = temp
				//fmt.Println("Open ", Open)
				binary.BigEndian.PutUint32(longValue, uint32(BidImpliedQuantity1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			} else {
				temp := uint16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
				start += 2
				BidImpliedQuantity1 = BaseBidImpliedQuantity + int32(temp)
				binary.BigEndian.PutUint32(longValue, uint32(BidImpliedQuantity1))
				copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
				IndexOfNormalMessage += 4

			}

			//fmt.Println("Offer Price1 ", BidPrice1)
			//fmt.Println("Offer Quantity1 ", BidQuantity1)
			//fmt.Println("Offer Orders1 ", BidOrders1)
			//fmt.Println("Offer Implied Quantity ", BidImpliedQuantity1)
			//fmt.Println("Index Of Normal Message ", IndexOfNormalMessage)
			BaseBidPrice = BidPrice1
			BaseBidQuantity = BidQuantity1
			BaseBidOrders = BidOrders1
			BaseBidImpliedQuantity = BidImpliedQuantity1

			if BreakBidLoop == 1 {

				//fmt.Println("Breaking Bid Loop As Order Book End Found")
				break
			}
			//break
		}
		if IndexOfNormalMessage < EndOfSellBid {
			for i := IndexOfNormalMessage; i < EndOfSellBid; i++ {
				FinalStrcuture.MessageData[i] = 0x00
			}
			IndexOfNormalMessage = EndOfBuyBid

		}

		//fmt.Println("End Of Normal Message ", IndexOfNormalMessage)
		BroadCastPipeLine <- FinalStrcuture

	}

	//fmt.Println(" Number Of Record ", NumberOfRecords)
	return NumberOfRecords

}

func ProcessIndexMarketPicture(buffer *[]byte, length *int, messageType *uint32) uint16 {

	NumberOfRecords := uint16(binary.BigEndian.Uint16((*buffer)[26:28]))

	for i := 0; i < int(NumberOfRecords); i++ {
		start := 28
		End := 4
		IndexOfNormalMessage = 8
		if *messageType == 2021 {
			End = 8
		}
		InstrumentCode := uint64(binary.BigEndian.Uint32((*buffer)[start : start+End]))
		start += End
		InstrumentCodeNegative := int64(InstrumentCode) * -1
		FinalStrcuture := CheckMemeoryExistForInstrumentID(&InstrumentCode)
		binary.BigEndian.PutUint64(longlongValue, uint64(InstrumentCodeNegative))

		copy(FinalStrcuture.MessageData[0:IndexOfNormalMessage], longlongValue)

		highPrice := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(highPrice))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		Low := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(Low))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		OpenPrice := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(OpenPrice))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		ClosePrice := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(ClosePrice))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		IndexValue := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		binary.BigEndian.PutUint32(longValue, uint32(IndexValue))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
		IndexOfNormalMessage += 4

		//fmt.Println("End Of Normal Message ", IndexOfNormalMessage)
		BroadCastPipeLine <- FinalStrcuture

	}

	//fmt.Println(" Number Of Record ", NumberOfRecords)
	return NumberOfRecords

}

func ProcessPacket(buffer *[]byte, length *int) {

	transactionCode := uint32(binary.BigEndian.Uint32((*buffer)[:4]))
	//fmt.Println("Transaction Code ", transactionCode)
	if transactionCode == 2015 {

		//time.Sleep(5)
		ProcessPacket2015(buffer, length)
		//time.Sleep(5)

	} else if transactionCode == 2020 {

		Process2020MarketPicture(buffer, length, &transactionCode)
	} else if transactionCode == 2021 {

		Process2020MarketPicture(buffer, length, &transactionCode)
	} else if transactionCode == 2016 {
		//fmt.Println("Transaction Code ", transactionCode)
		ProcessPacket2016(buffer, length)
		//fmt.Println("Message Recieved For 2016")
		//time.Sleep(5)
	} else if transactionCode == 2011 || transactionCode == 2012 {
		ProcessIndexMarketPicture(buffer, length, &transactionCode)
	}

}

func parseIPv4(ipv4 string) [4]byte {
	parsedIP := net.ParseIP(ipv4).To4()
	if parsedIP == nil {
		return [4]byte{}
	}
	return [4]byte{
		parsedIP[0],
		parsedIP[1],
		parsedIP[2],
		parsedIP[3],
	}

}

func InitiateBroadcastConnection(wg *sync.WaitGroup, broadCastInterface string,
	broadcastIP string, broadcastPort int, broadcastBindingPort int) {
	defer wg.Done()
	interfaceAddress := broadCastInterface + ":0"

	multicastAddress := broadcastIP + ":" + strconv.Itoa(broadcastPort)

	fmt.Println(multicastAddress)
	localAddr, err := net.ResolveUDPAddr("udp", interfaceAddress)
	if err != nil {
		fmt.Println("Broadcaster Initiation Error resolving local address:", err)
		return
	}

	remoteAddr, err := net.ResolveUDPAddr("udp", multicastAddress) //multicastAddress)// "255.255.255.255:8080")
	if err != nil {
		fmt.Println("Error resolving remote address:", err)
		return
	}

	conn, err := net.DialUDP("udp", localAddr, remoteAddr)
	if err != nil {
		fmt.Println("Error creating UDP connection:", err)
		return
	}
	defer conn.Close()
	fmt.Println("Broadcaster Is Ready To Broadcast")

	for {
		for packet := range BroadCastPipeLine {

			//need to convert it string value here only  then send on udp broadcast.....
			//declara a structue to capture all fields then in structure method write a function to convert all fields to string...
			// the method will take a string and return the converted string....
			//individually element has to be converted
			fmt.Printf("Recieved Broadcast Packet")

			concatenatedString := ""

			var InstrumentCode = binary.BigEndian.Uint64(packet.MessageData[:8])
			if InstrumentCode <= 0 {

				values := []string{
					fmt.Sprintf("%d^%d^%d^%d^%d^%d^",
						binary.BigEndian.Uint64(packet.MessageData[:8]),
						binary.BigEndian.Uint32(packet.MessageData[8:12]),
						binary.BigEndian.Uint32(packet.MessageData[12:16]),
						binary.BigEndian.Uint32(packet.MessageData[16:20]),
						binary.BigEndian.Uint64(packet.MessageData[21:29]),
						binary.BigEndian.Uint32(packet.MessageData[29:33]),
					),

					//fmt.Sprintf("%d",  binary.LittleEndian.Uint32(packet.MessageData[8:12])),
				}

				concatenatedString = "7202^" + strings.Join(values, "^") + "<EOF>"

			} else {

				values := []string{
					fmt.Sprintf("%d^%d^%d^%d^%c^%d^%d^%d^%d^%d^%d^%d^%d^%d^%d^%d^%d^%d^",
						binary.BigEndian.Uint64(packet.MessageData[:8]),
						binary.BigEndian.Uint32(packet.MessageData[8:12]),
						binary.BigEndian.Uint32(packet.MessageData[12:16]),
						binary.BigEndian.Uint32(packet.MessageData[16:20]),
						packet.MessageData[20],
						binary.BigEndian.Uint64(packet.MessageData[21:29]),
						binary.BigEndian.Uint32(packet.MessageData[29:33]),
						binary.BigEndian.Uint32(packet.MessageData[33:37]),
						binary.BigEndian.Uint32(packet.MessageData[37:41]),
						binary.BigEndian.Uint32(packet.MessageData[41:45]),
						binary.BigEndian.Uint32(packet.MessageData[45:49]),
						binary.BigEndian.Uint32(packet.MessageData[49:53]),
						binary.BigEndian.Uint32(packet.MessageData[53:57]),
						binary.BigEndian.Uint32(packet.MessageData[57:61]),
						binary.BigEndian.Uint32(packet.MessageData[61:65]),
						binary.BigEndian.Uint32(packet.MessageData[65:69]),
						binary.BigEndian.Uint32(packet.MessageData[69:73]),
						binary.BigEndian.Uint32(packet.MessageData[73:77]),
					),

					//fmt.Sprintf("%d",  binary.LittleEndian.Uint32(packet.MessageData[8:12])),
				}

				concatenatedString = "7208^" + strings.Join(values, "^")

				//append buy values
				for i := 77; i < 157; i += 16 {
					buyValues := []string{
						fmt.Sprintf("%d|%d|%d^",
							binary.BigEndian.Uint32(packet.MessageData[i:i+4]),
							binary.BigEndian.Uint32(packet.MessageData[i+4:i+8]),
							binary.BigEndian.Uint32(packet.MessageData[i+12:i+16]),
						),

						//fmt.Sprintf("%d",  binary.LittleEndian.Uint32(packet.MessageData[8:12])),
					}
					concatenatedString = concatenatedString + strings.Join(buyValues, "^")

				}

				//append sell values
				for i := 157; i < 237; i += 16 {
					buyValues := []string{
						fmt.Sprintf("%d|%d|%d^",
							binary.BigEndian.Uint32(packet.MessageData[i:i+4]),
							binary.BigEndian.Uint32(packet.MessageData[i+4:i+8]),
							binary.BigEndian.Uint32(packet.MessageData[i+12:i+16]),
						),

						//fmt.Sprintf("%d",  binary.LittleEndian.Uint32(packet.MessageData[8:12])),
					}
					concatenatedString = concatenatedString + strings.Join(buyValues, "^")

				}

				lastValues := []string{
					fmt.Sprintf("%d^%d^%d^%d^%d^",
						binary.BigEndian.Uint32(packet.MessageData[237:241]),
						binary.BigEndian.Uint64(packet.MessageData[241:249]),
						binary.BigEndian.Uint32(packet.MessageData[249:253]),
						binary.BigEndian.Uint32(packet.MessageData[253:257]),
						binary.BigEndian.Uint32(packet.MessageData[257:261]),
					),

					//fmt.Sprintf("%d",  binary.LittleEndian.Uint32(packet.MessageData[8:12])),
				}

				concatenatedString = concatenatedString + strings.Join(lastValues, "^") + "<EOF>"
			}

			_, err = conn.Write([]byte(concatenatedString))
			if err != nil {
				fmt.Println("Error broadcasting UDP packet:", err)
				continue
			}

			//fmt.Println("Packet sent:", packet)
		}
	}

	//fmt.Println("UDP packet broadcasted successfully!")

}

func InitiateUDPConnection(multicastIP string, multicastPort int, interfaceIP string) {

	// Resolve the interface IP address
	fmt.Println("Interface IP ", interfaceIP)
	fmt.Println("Broadcast IP ", multicastIP)
	fmt.Println("Port  ", multicastPort)
	ip := net.ParseIP(interfaceIP)
	if ip == nil {
		fmt.Println("Invalid interface IP address")
		return
	}

	// Resolve the multicast IP address
	multicastAddr := net.ParseIP(multicastIP)
	if multicastAddr == nil {
		fmt.Println("Invalid multicast IP address")
		return
	}

	// Find the network interface associated with the desired IP
	iface, err := findInterface(ip)
	if err != nil {
		fmt.Println("Error finding interface:", err)
		return
	}

	// Create a UDP connection
	conn, err := net.ListenUDP("udp4", &net.UDPAddr{IP: ip, Port: multicastPort})
	if err != nil {
		fmt.Println("Error creating UDP connection:", err)
		return
	}
	defer conn.Close()
	//defer Close(BroadCastPipeLine)
	defer close(BroadCastPipeLine)
	fmt.Println("Interface ", iface)
	fmt.Println("multicast address ", multicastAddr)

	// Join the multicast group on the specified interface
	p := ipv4.NewPacketConn(conn)
	if err := p.JoinGroup(iface, &net.UDPAddr{IP: multicastAddr}); err != nil {
		fmt.Println("Error joining multicast group:", err.Error())
		return
	}

	fmt.Println("Joined multicast group successfully.")

	buffer := make([]byte, 1024)
	packetRecieved := 0

	for {
		n, _, err := conn.ReadFromUDP(buffer)
		if err != nil {
			fmt.Println("Error reading UDP message:", err)
			return
			// You may choose to continue receiving packets by using `continue` instead of returning.
		}

		fmt.Println("Received Bytes =", n)
		start := time.Now()
		ProcessPacket(&buffer, &n)
		end := time.Now()
		diff := end.Sub(start)
		diff += 10
		//fmt.Println("Total Time Consumed To Process Packet ", diff)
		packetRecieved += 1
		//fmt.Println("Total Packet Processed ", packetRecieved)

	}

}

// findInterface finds the network interface associated with the provided IP address.
func findInterface(ip net.IP) (*net.Interface, error) {
	ifaces, err := net.Interfaces()
	if err != nil {
		return nil, err
	}

	for _, iface := range ifaces {
		addrs, err := iface.Addrs()
		if err != nil {
			return nil, err
		}

		for _, addr := range addrs {
			if ipnet, ok := addr.(*net.IPNet); ok && ipnet.Contains(ip) {
				return &iface, nil
			}
		}
	}

	return nil, fmt.Errorf("interface not found for IP: %s", ip.String())
}

/*
func InitiateUDPConnection(multicastIP string, port int, interfaceIP string) {

	// Create a UDP socket


	multicastPort := port      // Replace with the desired multicast port
	fmt.Println("Interface IP ", interfaceIP)
	fmt.Println("Broadcast IP ", multicastIP)
	fmt.Println("Port  ", port)


	// Resolve the interface IP address
	ip := net.ParseIP(interfaceIP)
	if ip == nil {
		fmt.Println("Invalid interface IP address")
		return
	}

	// Resolve the multicast IP address
	multicastAddr := net.ParseIP(multicastIP)
	if multicastAddr == nil {
		fmt.Println("Invalid multicast IP address")
		return
	}

	// Find the network interface associated with the desired IP
	iface, err := findInterface(ip)
	if err != nil {
		fmt.Println("Error finding interface:", err)
		return
	}

	// Create a UDP connection
	conn, err := net.ListenUDP("udp4", &net.UDPAddr{IP: ip, Port: multicastPort})
	if err != nil {
		fmt.Println("Error creating UDP connection:", err)
		return
	}
	defer conn.Close()

	// Join the multicast group on the specified interface
	p := ipv4.NewPacketConn(conn)
	if err := p.JoinGroup(iface, &net.UDPAddr{IP: multicastAddr}); err != nil {
		fmt.Println("UDP Connection Error joining multicast group:", err)
		return
	}

	fmt.Println("Joined multicast group successfully.")

	buffer := make([]byte, 1024)
	packetRecieved := 0

	for {
		n, _, err := conn.ReadFromUDP(buffer)
		if err != nil {
			fmt.Println("Error reading UDP message:", err)
			return
			// You may choose to continue receiving packets by using `continue` instead of returning.
		}

		//fmt.Println("Received Bytes =", n)
		start := time.Now()
		ProcessPacket(&buffer, &n)
		end := time.Now()
		diff := end.Sub(start)
		diff+=10
		//fmt.Println("Total Time Consumed To Process Packet ", diff)
		packetRecieved += 1
		//fmt.Println("Total Packet Processed ", packetRecieved)

	}

}
*/
