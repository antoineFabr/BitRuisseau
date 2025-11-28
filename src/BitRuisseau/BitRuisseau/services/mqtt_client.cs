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
    public class mqtt_client
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
                    SayOnline();
                    break;

                case "online":
                    GetOnlineMediatheque(msg);
                    break;

                case "askCatalog":
                    SendCatalog(msg.Sender);
                    break;

                case "sendCatalog":
                    GetCatalog(msg);
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

        public async void AskOnline()
        {
            string localIp = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
            Message msg = new Message() { Action = "askOnline", Sender = localIp, Recipient = "0.0.0.0" };

            string payload = JsonSerializer.Serialize(msg);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("BitRuisseau")
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message);
        }
        public string[] GetOnlineMediatheque(Message msg)
        {

            // 1. Définir le chemin
            string jsonPath = Path.Combine(Application.StartupPath, "data", "Mediatheque.json");

            // 2. Vérifier si le fichier existe pour éviter un crash
            if (!System.IO.File.Exists(jsonPath))
            {
                // Gérer l'erreur ou créer un fichier vide par défaut
                System.IO.File.WriteAllText(jsonPath, "{\"mediatheques\": []}");
            }

            // 3. Lire le JSON
            string mediathequesJ = System.IO.File.ReadAllText(jsonPath);

            Mediatheque mediathequesObj = JsonSerializer.Deserialize<Mediatheque>(mediathequesJ) ?? new Mediatheque();

            if (mediathequesObj.mediatheques == null)
            {
                mediathequesObj.mediatheques = new string[0]; 
            }

            var listeModifiable = mediathequesObj.mediatheques.ToList();
            string nouveauMedia = msg.Sender;

            if (!listeModifiable.Contains(nouveauMedia))
            {
                listeModifiable.Add(nouveauMedia);
            }

            mediathequesObj.mediatheques = listeModifiable.ToArray();

            var newJsonData = JsonSerializer.Serialize(mediathequesObj, new JsonSerializerOptions { WriteIndented = true });

            System.IO.File.WriteAllText(jsonPath, newJsonData);

            return listeModifiable.ToArray();
        }

        public async void SayOnline()
        {
            string localIp = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
            Message msg = new Message() { Action = "online", Sender = localIp, Recipient = "0.0.0.0"};
            string payload = JsonSerializer.Serialize(msg);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("BitRuisseau")
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message);

            // via mqtt
        }
        
        public async void AskCatalog(Message msg)
        {
            string localIp = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
            string payload = JsonSerializer.Serialize(msg);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("BitRuisseau")
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message);
            // demande mqtt

        }
        public List<Catalog> GetCatalog(Message msg)
        {

            string jsonPath = Path.Combine(Application.StartupPath, "data", "Catalog.json");

            // 2. Vérifier si le fichier existe pour éviter un crash
            if (!System.IO.File.Exists(jsonPath))
            {
                // Gérer l'erreur ou créer un fichier vide par défaut
                System.IO.File.WriteAllText(jsonPath, "{}");
            }

            // 3. Lire le JSON
            string CatalogsJ = System.IO.File.ReadAllText(jsonPath);
            MessageBox.Show(CatalogsJ);

            List<Catalog> CatalogsObj = JsonSerializer.Deserialize<List<Catalog>>(CatalogsJ) ?? new List<Catalog>();



            var listeModifiable = CatalogsObj.ToList();
            List<ISong> newMusic = msg.SongList;

            var cat = new Catalog() { sons = newMusic, holder = msg.Sender };
            listeModifiable.Add(cat);

            

            var newJsonData = JsonSerializer.Serialize(listeModifiable, new JsonSerializerOptions { WriteIndented = true });

            System.IO.File.WriteAllText(jsonPath, newJsonData);

            return listeModifiable;
        }

        public async void SendCatalog(string name)
        {
            string jsonPath = Path.Combine(Application.StartupPath, "data", "song.json");
            string songJ = File.ReadAllText(jsonPath);
            var songs = JsonSerializer.Deserialize<List<Song>>(songJ);
            MessageBox.Show(songJ);

            string localIp = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
            Message msg = new Message() { Action = "sendCatalog", Sender = localIp, Recipient = name, SongList = songs };
            string payload = JsonSerializer.Serialize(msg);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("BitRuisseau")
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message);
        }


        public void AskMedia(Message msg)
        {

        }


        public void SendMedia(string name, int startByte, int endByte)
        {

        }
        public static List<Song> GetSongs()
        {
            string jsonPath = Path.Combine(Application.StartupPath, "data", "song.json");
            string SongJ = System.IO.File.ReadAllText(jsonPath);
            List<Song> Songs;
            if (string.IsNullOrWhiteSpace(SongJ))
            {
                Songs = new List<Song>();
            }
            else
            {
                Songs = JsonSerializer.Deserialize<List<Song>>(SongJ);
            }
            return Songs;
        }
    }
}
