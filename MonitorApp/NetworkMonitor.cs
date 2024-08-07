using PcapDotNet.Core;
using PcapDotNet.Core.Extensions;
using PcapDotNet.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficMonitor.MonitorApp
{
    public class NetworkMonitor
    {
        private PacketDevice? _packetDevice;
        private IList<LivePacketDevice>? _allDevices;
        private PacketCommunicator? _communicator;

        public void InitializeDevice()
        {
            _allDevices = LivePacketDevice.AllLocalMachine;
            if( _allDevices.Count == 0 )
            {
                throw new Exception("No network devices found.");
            }


            // Print available devices
            Console.WriteLine("Available devices:");
            for (int i = 0; i < _allDevices.Count; i++)
            {
                LivePacketDevice device = _allDevices[i];
                Console.WriteLine($"{i}: {device.Description}");
            }

            // Allow user to select a device
            Console.Write("Select a device index: ");
            int deviceIndex = int.Parse(Console.ReadLine());
            _packetDevice = _allDevices[deviceIndex];
            Console.WriteLine($"Using device: {_packetDevice.Description}");

            _communicator = _packetDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);


        }

        public void StartCapture()
        {
            var result = _communicator?.ReceivePackets(0, PacketHandler);

            Console.WriteLine("Receiving Packets...");
            Console.WriteLine(result);
        }

        public void PacketHandler(Packet packet)
        {
            Console.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);
            AnalyzePacket(packet);
        }

        public void AnalyzePacket(Packet packet)
        {
            
            var ethernetPacket = packet.Ethernet;
            var ipPacket = ethernetPacket.IpV6;
            var tcpPacket = ipPacket.Tcp;



            //Log Details
            string srcAddress = ipPacket.Source.ToString();
            string dstAddress = ipPacket.CurrentDestination.ToString();
            int srcPort = tcpPacket.SourcePort;
            int? dstPort= tcpPacket.DestinationPort;

            if (tcpPacket?.DestinationPort != null)
            {
                dstPort = tcpPacket.DestinationPort;

            }

            string message;

            if(dstPort.HasValue)
            {
                message = $"Source : {srcAddress}:{srcPort}, Destination: {dstAddress}:{dstPort}";
                
            }
            else
            {
                message = $"Source : {srcAddress}:{srcPort}, Destination: {dstAddress}";
            }


            Console.WriteLine(message);
            LogToFile(message);

        }

        public void LogToFile(string logMessage)
        {
            
            using (StreamWriter writer = File.AppendText("network_log.txt"))
            {
                writer.WriteLine(logMessage);
                Console.WriteLine(logMessage);
                
            }


        }


    }
}
