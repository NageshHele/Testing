package main

import (
	"fmt"
	_ "log"
	_ "net/http"
	"os"
	"gopkg.in/yaml.v3"
	"io/ioutil"

	"example.com/GO-BSEFO/PacketStruct"
	//"example.com/GO-BSEFO/ProcessPacket"
	"sync"
)

var (
	address           = "224.0.0.1" //"227.0.0.22"//"226.1.1.1"
	inetIPAddress     = "127.0.0.1" //"10.111.27.8"
	port          int = 8080        //12997//11401
	bufferSize        = 2000

	broadCastInterface= "127.0.0.1"
	broadcastIP="224.1.1.1"
	broadcastBindingPort=8081
	broadcastPort=8083


)

type ConnectionParameter struct {
	BSEBroadcastInterfaceName             string `yaml:"BSEBroadcastInterfaceName"`
	BSEBroadcastMulticastIP string `yaml:"BSEBroadcastMulticastIP"`
	BSEBroadcastPort            int32 `yaml:"BSEBroadcastPort"`
	DestinationBroadcastInterface            string `yaml:"DestinationBroadcastInterface"`
	DestinationMulticastIP           string `yaml:"DestinationMulticastIP"`
	BindingPort        int32 `yaml:"BindingPort"`
	DestinationPort        int32 `yaml:"DestinationPort"`

}




type ConnectionSetting struct {
	Name string `yaml:"name"`

	Settings        ConnectionParameter `yaml:"Settings"`
	

}


var(

	ConnectionParameterConfig ConnectionSetting
)




func main() {

    data, err := ioutil.ReadFile(os.Args[1])
	if err != nil {
		fmt.Println(err)
		return
	}
	err = yaml.Unmarshal(data, &ConnectionParameterConfig)
	if err != nil {
		fmt.Println(err)
		return
	}

	// Print the data
	fmt.Println(ConnectionParameterConfig)
	var wg sync.WaitGroup
	wg.Add(1)
	fmt.Println("Welcome To BSE FO Broadcaster")
	broadCastInterface=ConnectionParameterConfig.Settings.DestinationBroadcastInterface
	broadcastIP=ConnectionParameterConfig.Settings.DestinationMulticastIP
	broadcastPort=int(ConnectionParameterConfig.Settings.DestinationPort)
	broadcastBindingPort= int(ConnectionParameterConfig.Settings.BindingPort)
	go PacketStruct.InitiateBroadcastConnection(&wg,broadCastInterface, broadcastIP, broadcastBindingPort,broadcastPort)
	address=ConnectionParameterConfig.Settings.BSEBroadcastMulticastIP
	port= int(ConnectionParameterConfig.Settings.BSEBroadcastPort)
	inetIPAddress=ConnectionParameterConfig.Settings.BSEBroadcastInterfaceName
	PacketStruct.InitiateUDPConnection(address, port, inetIPAddress, )
    wg.Wait()
}
