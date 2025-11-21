using BitRuisseau.data;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace BitRuisseau.services
{
    public class mqtt_client : IProtocol
    {
        private MqttClientFactory factory = new MqttClientFactory();
        public static string brokerHost = "mqtt.blue.section-inf.ch";
               
        private readonly string _brokerIp = GetPreferredIpAddress(mqtt_client.brokerHost).ToString();
        public async Task<bool> ConnectToBroker()
        {
            try
            {

                var mqttClient = factory.CreateMqttClient();
                var options = new MqttClientOptionsBuilder()
               .WithTcpServer("mqtt.blue.section-inf.ch", 1883)      // Broker public gratuit
               .WithClientId("Antoine" + Guid.NewGuid()) // ID unique
               .WithCredentials("ict","321")
               .Build();

                await mqttClient.ConnectAsync(options);

                return true;
            }
            catch
            {
                return false;
            }
        }

        static IPAddress GetPreferredIpAddress(string host)
        {
            //priority on the dgep ipv4 address
            return Dns.GetHostAddresses(host)
                .Where(/*V4*/address => address.AddressFamily == AddressFamily.InterNetwork)
                .Where(address => address.ToString().StartsWith("10"))
                .FirstOrDefault(Dns.GetHostAddresses(host)[0]);
        }

        public string[] GetOnlineMediatheque()
        {
            // demande mqtt via mqtt
            string[] mediatheque = ["192.168.34.5", "168.143.53.43"];
            return mediatheque;
        }

        public void SayOnline()
        {
            // via mqtt
        }

        public List<ISong> AskCatalog(string name)
        {

            // demande mqtt
            List<ISong> songs = new List<ISong>();

            return songs;
        }


        public void SendCatalog(string name)
        {
            
        }


        public void AskMedia(string name, int startByte, int endByte)
        {

        }


        public void SendMedia(string name, int startByte, int endByte)
        {

        }
    }
}
