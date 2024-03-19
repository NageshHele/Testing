package PacketStruct

import (
	"encoding/binary"
	"fmt"
	_ "log"
	"net"
	"syscall"
	"time"
)

const (
	InitialBroadcastMessageLength = 196
	FinalBroadcastMessageLength   = 212 //
	StartOfOpenInterest           = 196
	MaxByteValue                  = 0x7F
	MaxUnsignedByteValue          = 0xFF
)

type FinalBroadcastStructure struct {
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
	sharedMemoryMap             = make(map[uint32]*FinalBroadcastStructure)
	shortValue                  = make([]byte, 4)
	longValue                   = make([]byte, 4)
	longlongValue               = make([]byte, 8)
	IndexOfOpenInterest         = StartOfOpenInterest
	IndexOfNormalMessage        = 4
	Open                 int32  = 0
	High                 int32  = 0
	Low                  int32  = 0
	Close                int32  = 0
	TotalBidQuantity     uint32 = 0
	TotalOfferQuantity   uint32 = 0
	UpperCircuit         int32  = 0
	LowerCircuit         int32  = 0
	WeightedAveragePrice int32  = 0
)

func Sample1() {

	fmt.Println("Hi Welcome To Packet Structure")

}

func Insert2015Value(FinalStrcuture *FinalBroadcastStructure,
	OpenInterestQuantity *uint32, OpenInterestValue *uint64, OpenInterestChange *uint32) {

	binary.BigEndian.PutUint32(longValue, uint32(*OpenInterestQuantity))
	copy(FinalStrcuture.MessageData[IndexOfOpenInterest:IndexOfOpenInterest+4], longValue)
	IndexOfOpenInterest += 4

	binary.BigEndian.PutUint64(longlongValue, uint64(*OpenInterestValue))
	copy(FinalStrcuture.MessageData[IndexOfOpenInterest:IndexOfOpenInterest+8], longlongValue)
	IndexOfOpenInterest += 8
	binary.BigEndian.PutUint32(longValue, uint32(*OpenInterestQuantity))
	copy(FinalStrcuture.MessageData[IndexOfOpenInterest:IndexOfOpenInterest+4], longValue)
	IndexOfOpenInterest = StartOfOpenInterest

}

func ProcessPacket2015(buffer *[]byte, length *int) uint16 {

	numberOfRecords := uint16(binary.BigEndian.Uint16((*buffer)[26:28]))
	Header2015End := 28
	Var2015MessageSize := 36

	for i := 0; i < int(numberOfRecords); i++ {
		start := Header2015End
		InstrumentID := uint32(binary.BigEndian.Uint32((*buffer)[Header2015End : Header2015End+4]))
		Header2015End += 4
		OpenInterestQuantity := uint32(binary.BigEndian.Uint32((*buffer)[Header2015End : Header2015End+4]))
		Header2015End += 4
		OpenInterestValue := uint64(binary.BigEndian.Uint64((*buffer)[Header2015End : Header2015End+8]))
		Header2015End += 8
		OpenInterestChange := uint32(binary.BigEndian.Uint32((*buffer)[Header2015End : Header2015End+4]))
		Header2015End += 4
		Header2015End = start
		Header2015End += Var2015MessageSize
		fmt.Println("Instrment ID ", InstrumentID)
		fmt.Println("Open Interest Quantity ", OpenInterestQuantity)
		fmt.Println("Open Interest Value ", OpenInterestValue)
		fmt.Println("Open Interest Change ", OpenInterestChange)

		if val, ok := sharedMemoryMap[InstrumentID]; ok {
			Insert2015Value(val, &OpenInterestQuantity, &OpenInterestValue, &OpenInterestChange)
			fmt.Println("Instrument ID Exist ")
		} else {
			// Key doesn't exist, create a new value
			newValue := &FinalBroadcastStructure{}

			sharedMemoryMap[InstrumentID] = newValue
			Insert2015Value(newValue, &OpenInterestQuantity, &OpenInterestValue, &OpenInterestChange)
			fmt.Println("Instrument ID Created ")
		}

	}

	fmt.Println("Number Of Records ", numberOfRecords)

	return numberOfRecords

}

func CheckMemeoryExistForInstrumentID(InstrumentID *uint32) *FinalBroadcastStructure {

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

func Process2020MarketPicture(buffer *[]byte, length *int) uint16 {

	NumberOfRecords := uint16(binary.BigEndian.Uint16((*buffer)[26:28]))
	start := 28
	IndexOfNormalMessage = 4

	for i := 0; i < 1; i++ {

		InstrumentCode := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
		start += 4
		FinalStrcuture := CheckMemeoryExistForInstrumentID(&InstrumentCode)
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
		//fmt.Println(" Value In Lakhs Or Crore ", tradeValueInLacOrCR)
		start += 20
		//fmt.Println("Position Of Start Index ", start)
		Timestamp := uint64(binary.BigEndian.Uint64((*buffer)[start : start+8]))
		//fmt.Println("TimeStamp :", Timestamp)
		start += 8
		binary.BigEndian.PutUint64(longlongValue, uint64(Timestamp))
		copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+8], longlongValue)
		IndexOfNormalMessage += 8

		CloseRate := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
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

		////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////Decompression Logic Started/////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////Retriving Open Price////////////////////////////////
		if (*buffer)[start] == MaxByteValue && (*buffer)[start+1] == MaxByteValue {
			//fmt.Println("Got Maximum Byte")
			temp := int32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
			//fmt.Println("Temp ", temp)
			start += 4
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

		if (*buffer)[start] == MaxByteValue && (*buffer)[start+1] == MaxByteValue {
			//fmt.Println("Got Maximum Byte")
			temp := int32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
			//fmt.Println("Temp ", temp)
			start += 4
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

		if (*buffer)[start] == MaxByteValue && (*buffer)[start+1] == MaxByteValue {
			//fmt.Println("Got Maximum Byte")
			temp := int32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
			//fmt.Println("Temp ", temp)
			start += 4
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

		if (*buffer)[start] == MaxByteValue && (*buffer)[start+1] == MaxByteValue {
			//fmt.Println("Got Maximum Byte")
			temp := int32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
			//fmt.Println("Temp ", temp)
			start += 4
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
		fmt.Println("Start Index Value ", start)
		fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+20])

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

		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32763 {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+4])
			start += 4

		} else {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
			start += 2
		}

		////////////////////////////////////////Retriving Bid Quantity////////////////////////////////
		fmt.Println("Start Index Value ", start)
		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+4])
			temp := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
			//fmt.Println("Temp ", temp)
			start += 4
			TotalBidQuantity = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(TotalBidQuantity))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
			temp := uint16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			TotalBidQuantity = LTQ + uint32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(Low))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Total Offer Quantity////////////////////////////////

		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767 {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+4])
			temp := uint32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
			//fmt.Println("Temp ", temp)
			start += 4
			TotalOfferQuantity = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(TotalOfferQuantity))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
			temp := uint16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			TotalOfferQuantity = LTQ + uint32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(TotalOfferQuantity))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Lower Circuit////////////////////////////////

		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767  {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+4])

			temp := int32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
			//fmt.Println("Temp ", temp)
			start += 4
			LowerCircuit = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(LowerCircuit))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+2])
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			LowerCircuit = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(LowerCircuit))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		////////////////////////////////////////Upper Circuit////////////////////////////////

		if int((*buffer)[start])<<8|int((*buffer)[start+1]) == 32767  {
			fmt.Printf("Hexadecimal representation: %X\n", (*buffer)[start:start+4])
			temp := int32(binary.BigEndian.Uint32((*buffer)[start : start+4]))
			//fmt.Println("Temp ", temp)
			start += 4
			UpperCircuit = temp
			//fmt.Println("Open ", Open)
			binary.BigEndian.PutUint32(longValue, uint32(UpperCircuit))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		} else {
			fmt.Printf("Hexadecimal Representation: %X\n", (*buffer)[start:start+2])
			temp := int16(binary.BigEndian.Uint16((*buffer)[start : start+2]))
			start += 2
			UpperCircuit = LTP + int32(temp)
			binary.BigEndian.PutUint32(longValue, uint32(UpperCircuit))
			copy(FinalStrcuture.MessageData[IndexOfNormalMessage:IndexOfNormalMessage+4], longValue)
			IndexOfNormalMessage += 4

		}

		fmt.Println("LTP  ", LTP)
		fmt.Println("Open Price ", Open)
		fmt.Println("Close Price ", Close)
		fmt.Println("High Price ", High)
		fmt.Println("Low Price ", Low)
		fmt.Println("Upper Circuit ", UpperCircuit)
		fmt.Println("Lower Circuit ", LowerCircuit)
		fmt.Println("Weighted Average Price ", WeightedAveragePrice)
		fmt.Println("LTQ ", LTQ)
		fmt.Println("Total Bid Quantity ", TotalBidQuantity)
		fmt.Println("Total Ask Quantity ", TotalOfferQuantity)

	}

	//fmt.Println(" Number Of Record ", NumberOfRecords)
	return NumberOfRecords

}

func ProcessPacket(buffer *[]byte, length *int) {

	transactionCode := uint32(binary.BigEndian.Uint32((*buffer)[:4]))
	//fmt.Println("Transaction Code ", transactionCode)
	if transactionCode == 2015 {

		ProcessPacket2015(buffer, length)

	} else if transactionCode == 2020 {

		Process2020MarketPicture(buffer, length)
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

func InitiateUDPConnection(multicastIP string, port int, interfaceIP string) {

	// Create a UDP socket
	multicastPort := port
	multicastGroup := multicastIP
	sourceIP := interfaceIP

	sock, err := syscall.Socket(syscall.AF_INET, syscall.SOCK_DGRAM, 0)
	if err != nil {
		fmt.Println("Socket initialization failed. Error:", err)
		return
	}

	// Set socket options
	err = syscall.SetsockoptInt(sock, syscall.SOL_SOCKET, syscall.SO_REUSEADDR, 1)
	if err != nil {
		fmt.Println("Failed to set socket options. Error:", err)
		return
	}

	// Bind to the multicast address
	addr := &syscall.SockaddrInet4{
		Port: multicastPort,
		Addr: parseIPv4(multicastGroup),
	}
	err = syscall.Bind(sock, addr)
	if err != nil {
		fmt.Println("Failed to bind socket. Error:", err)
		return
	}

	// Join the multicast group with the specific source IP
	mreq := &syscall.IPMreq{
		Multiaddr: parseIPv4(multicastGroup),
		Interface: parseIPv4(sourceIP),
	}
	err = syscall.SetsockoptIPMreq(sock, syscall.IPPROTO_IP, syscall.IP_ADD_MEMBERSHIP, mreq)
	if err != nil {
		fmt.Println("Failed to join multicast group. Error:", err)
		return
	}

	fmt.Println("Socket initialized successfully")
	// Continue with further processing or listening to incoming multicast message
	defer syscall.Close(sock)
	buffer := make([]byte, 2000)
	for {
		n, _, err := syscall.Recvfrom(sock, buffer, 0)
		if err != nil {
			fmt.Println("Failed to receive data. Error:", err)
			continue
		}

		fmt.Println("Received Bytes =", n)
		start := time.Now()
		ProcessPacket(&buffer, &n)
		end := time.Now()
		diff := end.Sub(start)
		fmt.Println("Total Time Consumed To Process Packet ", diff)

	}

}
