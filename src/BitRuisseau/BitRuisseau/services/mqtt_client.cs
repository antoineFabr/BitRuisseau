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
                    MessageBox.Show(ex.Message);
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
                    SendCatalog(msg);
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
            string localIp = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToString();

            // 1. On ignore nos propres messages pour éviter les boucles
            if (msg.Sender == localIp) return new List<Catalog>();

            string jsonPath = Path.Combine(Application.StartupPath, "data", "Catalog.json");
            List<Catalog> globalCatalog = new List<Catalog>();

            // 2. Chargement du catalogue existant (Gestion d'erreur incluse)
            if (System.IO.File.Exists(jsonPath))
            {
                try
                {
                    string jsonContent = System.IO.File.ReadAllText(jsonPath);
                    globalCatalog = JsonSerializer.Deserialize<List<Catalog>>(jsonContent) ?? new List<Catalog>();
                }
                catch
                {
                    Console.WriteLine("Format du catalogue invalide, on repart à zéro.");
                    globalCatalog = new List<Catalog>();
                }
            }

            // 3. Fusion des musiques reçues
            if (msg.SongList != null)
            {
                foreach (Song incomingSong in msg.SongList)
                {
                    // On cherche si cette musique (par son HASH) existe déjà dans notre catalogue
                    // (On utilise le Hash car le titre peut varier légèrement mais pas le contenu)
                    var existingEntry = globalCatalog.FirstOrDefault(c => c.Hash == incomingSong.Hash);

                    if (existingEntry != null)
                    {
                        // CAS A : La musique existe déjà
                        // On ajoute l'expéditeur à la liste des Holders s'il n'y est pas déjà
                        if (!existingEntry.Holders.Contains(msg.Sender))
                        {
                            existingEntry.Holders.Add(msg.Sender);
                        }
                    }
                    else
                    {
                        Catalog newEntry = new Catalog
                        {
                            Title = incomingSong.Title,
                            Artist = incomingSong.Artist,
                            Year = incomingSong.Year,
                            Duration = incomingSong.Duration,
                            Size = incomingSong.Size,
                            Featuring = incomingSong.Featuring,
                            Hash = incomingSong.Hash,

                            Holders = new List<string> { msg.Sender }
                        };

                        globalCatalog.Add(newEntry);
                    }
                }
            }

            // 4. Sauvegarde
            var options = new JsonSerializerOptions { WriteIndented = true };
            string output = JsonSerializer.Serialize(globalCatalog, options);
            System.IO.File.WriteAllText(jsonPath, output);

            return globalCatalog;
        }

        public async void SendCatalog(Message msg)
        {
            string localIp = Dns.GetHostEntry(Dns.GetHostName())
               .AddressList
               .First(x => x.AddressFamily == AddressFamily.InterNetwork)
               .ToString();

            // --- FILTRE ANTI-ECHO ---
            if (msg.Sender == localIp)
            {
                // C'est moi qui ai demandé le catalogue, je ne vais pas m'envoyer le mien à moi-même.
                return;
            }

            // --- CORRECTION ICI ---
            // Au lieu de lire le fichier manuellement (ce qui plante s'il est vide),
            // on utilise votre méthode existante qui est sécurisée.
            List<Song> songs = GetSongs();

            // On prépare le message de réponse
            Message newmsg = new Message()
            {
                Action = "sendCatalog",
                Sender = localIp,
                Recipient = msg.Sender,
                SongList = songs
            };

            string payload = JsonSerializer.Serialize(newmsg);
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
