module example.com/GO-BSEFO

replace example.com/GO-BSEFO/PacketStruct => ./PacketStruct

replace example.com/GO-BSEFO/ProcessPacket => ./ProcessPacket

go 1.13

require (
	example.com/GO-BSEFO/PacketStruct v0.0.0-00010101000000-000000000000
	golang.org/x/net v0.10.0 // indirect
	gopkg.in/yaml.v3 v3.0.1
)
