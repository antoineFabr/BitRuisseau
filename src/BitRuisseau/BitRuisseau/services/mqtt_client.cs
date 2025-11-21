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
using System.Text.Json;

namespace BitRuisseau.services
{
    public class mqtt_client : IProtocol
    {
        private MqttClientFactory factory = new MqttClientFactory();
        public static string brokerHost = "mqtt.blue.section-inf.ch";
        private IMqttClient mqttClient;

        private readonly string _brokerIp = GetPreferredIpAddress(mqtt_client.brokerHost).ToString();
        public async Task<bool> ConnectToBroker()
        {
            try
            {

                mqttClient = factory.CreateMqttClient();
                var options = new MqttClientOptionsBuilder()
               .WithTcpServer("mqtt.blue.section-inf.ch", 1883)      // Broker etml blue
               .WithClientId("Antoine" + Guid.NewGuid()) // ID unique
               .WithCredentials("ict","321")
               .Build();

                await mqttClient.ConnectAsync(options);
                await mqttClient.SubscribeAsync("BitRuisseau");

                Listener();

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

        public void Listener()
        {
            if (mqttClient == null)
            {
                Console.WriteLine("Listener appelé avant la connexion MQTT !");
                return;
            }

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    string topic = e.ApplicationMessage.Topic;
                    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                    Console.WriteLine($"[MQTT] Message reçu sur {topic}: {payload}");

                    // Désérialisation JSON --> Objet Message
                    var msg = JsonSerializer.Deserialize<Message>(payload);

                    if (msg == null)
                    {
                        Console.WriteLine("Message JSON invalide.");
                        return;
                    }

                    // Route le message
                    HandleIncomingMessage(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur dans Listener(): {ex.Message}");
                }
            };
        }

        private void HandleIncomingMessage(Message msg)
        {
            Console.WriteLine($"Message Action = {msg.Action}");

            switch (msg.Action)
            {
                case "askOnline":
                    break;

                case "online":
                    break;

                case "askCatalog":
                    break;

                case "sendCatalog":
                    break;

                case "askMedia":
                    break;

                case "sendMedia":
                    break;

                default:
                    Console.WriteLine($"Action inconnue ${msg.Action}");
                    break;
            }
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
