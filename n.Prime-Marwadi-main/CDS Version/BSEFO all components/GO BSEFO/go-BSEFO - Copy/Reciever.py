import socket
import ctypes
import os, sys

class BroadcastStructure(ctypes.BigEndianStructure):
    _pack_ = 1
    _fields_ = [
        ("InstrumentID", ctypes.c_uint64),
        ("NumberOfTrades", ctypes.c_uint32),
        ("TradeVolume", ctypes.c_uint32),
        ("TradeValue", ctypes.c_uint32),
        ("TraceValue", ctypes.c_char),
        ("TimeStamp", ctypes.c_uint64),
        ("TodayCloseRate", ctypes.c_uint32),
        ("LTQ", ctypes.c_uint32),
        ("LTP", ctypes.c_int32),
        ("OpenPrice", ctypes.c_int32),
        ("LastTradingSessionClosePrice", ctypes.c_int32),
        ("High", ctypes.c_int32),
        ("Low", ctypes.c_int32),
        ("TotalBidQuantity", ctypes.c_uint32),
        ("TotalAskQuantity", ctypes.c_uint32),
        ("LowerCircuit", ctypes.c_int32),
        ("UpperCircuit", ctypes.c_int32),
        ("WeightedAveragePrice", ctypes.c_int32),
        ("BID", ctypes.c_uint32 *20),
        ("ASK", ctypes.c_uint32 *20),
        ("OpenInterestQuantity", ctypes.c_uint32),
        ("OpenInterestValue", ctypes.c_uint64),
        ("OpenInterestChange", ctypes.c_uint32),
        ("VarIMPercentage", ctypes.c_uint32),
        ("ELMVARPercentage", ctypes.c_uint32)
    ]

MCAST_GRP = sys.argv[2]
MCAST_PORT =int(sys.argv[3])
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
sock.bind((sys.argv[1], MCAST_PORT))
mreq = socket.inet_aton(MCAST_GRP) + socket.inet_aton(sys.argv[1])

sock.setsockopt(socket.IPPROTO_IP, socket.IP_ADD_MEMBERSHIP, mreq)


broadcast = BroadcastStructure()

while True:
    try:
        buffer= sock.recv(1024)
        print(buffer)
        broadcast= BroadcastStructure.from_buffer_copy(buffer)
        # Create an instance of the structure
        # Access and display the values of its elements

        print("InstrumentID:", broadcast.InstrumentID)
        print("NumberOfTrades:", broadcast.NumberOfTrades)
        print("TradeVolume:", broadcast.TradeVolume)
        print("TradeValue:", broadcast.TradeValue)
        print("TradeValueFlag:", broadcast.TraceValue)
        print("TimeStamp:", broadcast.TimeStamp)
        print("TodayCloseRate:", broadcast.TodayCloseRate)
        print("LTQ:", broadcast.LTQ)
        print("LTP:", broadcast.LTP)
        print("OpenPrice:", broadcast.OpenPrice)
        print("LastTradingSessionClosePrice:", broadcast.LastTradingSessionClosePrice)
        print("High:", broadcast.High)
        print("Low:", broadcast.Low)
        print("TotalBidQuantity:", broadcast.TotalBidQuantity)
        print("TotalAskQuantity:", broadcast.TotalAskQuantity)
        print("LowerCircuit:", broadcast.LowerCircuit)
        print("UpperCircuit:", broadcast.UpperCircuit)
        print("WeightedAveragePrice:", broadcast.WeightedAveragePrice)
        k=0
        for i in range(0,5):
            print("BIDPrice{}:".format(i+1),broadcast.BID[k])
            k+=1
            print("BidQuantity{}:".format(i + 1), broadcast.BID[k])
            k+=1
            print("BidOrders{}:".format(i + 1), broadcast.BID[k])
            k+=2
        k = 0
        for i in range(0, 5):
            print("SellPrice{}:".format(i + 1), broadcast.ASK[k])
            k += 1
            print("SellQuantity{}:".format(i + 1), broadcast.ASK[k])
            k += 1
            print("SellOrders{}:".format(i + 1), broadcast.ASK[k])
            k += 2
        print("OpenInterestQuantity:", broadcast.OpenInterestQuantity)
        print("OpenInterestValue:", broadcast.OpenInterestValue)
        print("OpenInterestChange:", broadcast.OpenInterestChange)
        print("Var/IM Percentage:", broadcast.VarIMPercentage)
        print("ELMVARPercentage:", broadcast.ELMVARPercentage)
    except Exception as e:
        #print("Exception ", str(e))
        pass

