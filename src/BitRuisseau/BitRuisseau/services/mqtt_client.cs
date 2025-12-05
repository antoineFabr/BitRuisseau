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
                    SendMedia(msg);
                    break;

                case "sendMedia":
                    HandleReceivedMedia(msg);
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


        public async Task AskMedia(Message msg)
        {
            try
            {
                string payload = JsonSerializer.Serialize(msg);
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("BitRuisseau")
                    .WithPayload(payload)
                    .Build();

                await mqttClient.PublishAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi de AskMedia : {ex.Message}");
            }
        }

        public async void SendMedia(Message msg)
        {
            string localIp = Dns.GetHostEntry(Dns.GetHostName())
               .AddressList
               .First(x => x.AddressFamily == AddressFamily.InterNetwork)
               .ToString();
            MessageBox.Show("caca");
            // 1. Vérifier si la demande m'est destinée
            // (Note : msg.Recipient peut être une IP ou un Hostname, vérifiez les deux si besoin)
            if (msg.Recipient != localIp && msg.Recipient != "0.0.0.0") return;

            MessageBox.Show($"Quelqu'un ({msg.Sender}) me demande un morceau de musique (Hash: {msg.Hash})");

            // 2. Trouver le fichier correspondant au Hash dans ma liste locale
            List<Song> mySongs = GetSongs();
            Song requestedSong = mySongs.FirstOrDefault(s => s.Hash == msg.Hash);

            if (requestedSong == null || !File.Exists(requestedSong.Path))
            {
                MessageBox.Show("Erreur : Je ne possède pas ce fichier ou le chemin est incorrect.");

                return;
            }

            try
            {
                // 3. Lire UNIQUEMENT les octets demandés (FileStream)
                using (FileStream fs = new FileStream(requestedSong.Path, FileMode.Open, FileAccess.Read))
                {
                    int start = msg.StartByte ?? 0;
                    int end = msg.EndByte ?? (int)fs.Length;

                    // Sécurité : ne pas lire plus loin que la fin du fichier
                    if (end > fs.Length) end = (int)fs.Length;

                    int lengthToRead = end - start;
                    if (lengthToRead <= 0) return;

                    byte[] buffer = new byte[lengthToRead];

                    // On se déplace au StartByte
                    fs.Seek(start, SeekOrigin.Begin);
                    // On lit le nombre d'octets requis
                    int bytesRead = fs.Read(buffer, 0, lengthToRead);

                    // 4. Encodage en Base64 (selon protocole.md)
                    string base64Data = Convert.ToBase64String(buffer);

                    // 5. Préparation de la réponse sendMedia
                    Message responseMsg = new Message()
                    {
                        Action = "sendMedia",
                        Sender = localIp,
                        Recipient = msg.Sender, // Je réponds à celui qui a demandé
                        Hash = msg.Hash,
                        StartByte = start,
                        EndByte = end,
                        SongData = base64Data // Le contenu audio
                    };

                    // 6. Envoi
                    string payload = JsonSerializer.Serialize(responseMsg);
                    var mqttMessage = new MqttApplicationMessageBuilder()
                        .WithTopic("BitRuisseau")
                        .WithPayload(payload)
                        .Build();

                    await mqttClient.PublishAsync(mqttMessage);
                    MessageBox.Show($"Segment envoyé ! ({bytesRead} octets)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lecture/envoi fichier : {ex.Message}");
            }
        }


        public async void DownloadSong(Catalog songToDownload)
        {
            // 1. Vérifications de base
            if (songToDownload.Holders == null || songToDownload.Holders.Count == 0)
            {
                MessageBox.Show("Aucune source disponible pour ce fichier.");
                return;
            }

            string localIp = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToString();

            int totalSize = songToDownload.Size;
            int numberOfHolders = songToDownload.Holders.Count;

            // 2. Calcul de la taille d'un morceau (Division entière)
            int chunkSize = totalSize / numberOfHolders;
            int currentStart = 0;

            Console.WriteLine($"Début du téléchargement de '{songToDownload.Title}' via {numberOfHolders} sources.");

            // 3. Boucle sur chaque possesseur pour lui demander sa part
            for (int i = 0; i < numberOfHolders; i++)
            {
                string holderIp = songToDownload.Holders[i];

                // On ne se demande pas à soi-même si on est dans la liste (cas rare mais possible)
                if (holderIp == localIp) continue;

                // Calcul de la fin du segment
                // Si c'est le dernier holder, il prend tout ce qui reste (pour gérer les divisions impaires)
                int currentEnd = (i == numberOfHolders - 1) ? totalSize : (currentStart + chunkSize);

                // Création du message respectant le protocole.md
                Message msg = new Message()
                {
                    Action = "askMedia",
                    Sender = localIp,
                    Recipient = holderIp, // On cible précisément cette personne
                    Hash = songToDownload.Hash, // L'identifiant unique du fichier
                    StartByte = currentStart,
                    EndByte = currentEnd,
                    SongList = null,
                    SongData = null
                };

                Console.WriteLine($"-> Demande à {holderIp} : Octets {currentStart} à {currentEnd}");

                // Envoi de la requête MQTT
                await AskMedia(msg);

                // Le prochain segment commence là où celui-ci finit
                currentStart = currentEnd;
            }
        }
        // Variable pour empêcher deux écritures simultanées sur le même fichier
        private static readonly object fileLock = new object();

        public void HandleReceivedMedia(Message msg)
        {
            // 1. Validation de base
            if (string.IsNullOrEmpty(msg.SongData) || string.IsNullOrEmpty(msg.Hash)) return;

            try
            {
                // 2. Convertir les données reçues (Base64 -> Bytes)
                byte[] data = Convert.FromBase64String(msg.SongData);

                // 3. Déterminer le nom du fichier et le dossier
                // On utilise le Hash comme nom de fichier temporaire pour éviter les conflits de noms
                // Idéalement, on renommera le fichier avec son Titre une fois fini, ou on cherche le titre via le Hash maintenant.
                string downloadFolder = Path.Combine(Application.StartupPath, "data", "Downloads");

                // Créer le dossier Downloads s'il n'existe pas
                if (!Directory.Exists(downloadFolder))
                {
                    Directory.CreateDirectory(downloadFolder);
                }

                // On essaie de retrouver l'extension originale via le catalogue, sinon .mp3 par défaut
                string extension = ".mp3";

                // Petite astuce : on regarde dans le catalogue global pour voir si on connait ce Hash
                // (Optionnel : améliore juste le nom du fichier)
                string catalogPath = Path.Combine(Application.StartupPath, "data", "Catalog.json");
                if (File.Exists(catalogPath))
                {
                    var catalog = JsonSerializer.Deserialize<List<Catalog>>(File.ReadAllText(catalogPath));
                    var songInfo = catalog?.FirstOrDefault(s => s.Hash == msg.Hash);
                    if (songInfo != null && !string.IsNullOrEmpty(songInfo.Extension))
                    {
                        extension = songInfo.Extension;
                    }
                }

                string filePath = Path.Combine(downloadFolder, $"{msg.Hash}{extension}");

                // 4. Écriture sécurisée sur le disque
                lock (fileLock)
                {
                    // OpenOrCreate : Crée le fichier si c'est le premier paquet, sinon l'ouvre
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        // IMPORTANT : On se déplace à l'endroit exact où ce morceau doit aller
                        if (msg.StartByte.HasValue)
                        {
                            fs.Seek(msg.StartByte.Value, SeekOrigin.Begin);
                        }

                        // On écrit les données
                        fs.Write(data, 0, data.Length);
                    }
                }

                Console.WriteLine($"Segment écrit pour {msg.Hash} : {data.Length} octets à la position {msg.StartByte}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'écriture du fichier : {ex.Message}");
            }
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
