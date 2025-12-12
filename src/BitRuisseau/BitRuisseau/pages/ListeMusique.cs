using BitRuisseau.data;
using BitRuisseau.services;
using MQTTnet;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using TagLib;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string ExistSongJ = System.IO.File.ReadAllText(jsonPath);

            List<Song> ExistSong = mqtt_client.GetSongs();
            Connect();
            ExistSong.ForEach(x => ListeSong.Items.Add(x.Title));
        }
        mqtt_client mqttClient = new mqtt_client();


        string jsonPath = Path.Combine(Application.StartupPath, "data", "song.json");
        string jsonPathMedia = Path.Combine(Application.StartupPath, "data", "Mediatheque.json");
        string jsonPathCat = Path.Combine(Application.StartupPath, "data", "Catalog.json");


        private async void Connect()
        {

            bool isConnect = await mqttClient.ConnectToBroker();

            if (!isConnect)
            {
                MessageBox.Show("Impossible de se connecter au broker mqtt");
            }
            else
            {
                MessageBox.Show("Connexion réussi au broker mqtt");

            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog ofd = new FolderBrowserDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = ofd.SelectedPath;

                    var songs = Directory.GetFiles(selectedFolder, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.EndsWith(".wav") || f.EndsWith(".mp3"))
                        .ToList();

                    // Relecture propre du fichier existant
                    string jsonContent = System.IO.File.Exists(jsonPath) ? System.IO.File.ReadAllText(jsonPath) : "";
                    List<Song> ExistSong = string.IsNullOrWhiteSpace(jsonContent)
                        ? new List<Song>()
                        : JsonSerializer.Deserialize<List<Song>>(jsonContent);

                    songs.ForEach(x =>
                    {
                        try
                        {
                            FileInfo file = new FileInfo(x);
                            var tfile = TagLib.File.Create(x);

                            // --- CORRECTION CRASH ARTISTE ---
                            // On vérifie si AlbumArtists contient quelque chose, sinon on cherche Performers, sinon "Inconnu"
                            string safeArtist = "Artiste Inconnu";
                            if (tfile.Tag.AlbumArtists != null && tfile.Tag.AlbumArtists.Length > 0)
                            {
                                safeArtist = tfile.Tag.AlbumArtists[0];
                            }
                            else if (tfile.Tag.Performers != null && tfile.Tag.Performers.Length > 0)
                            {
                                safeArtist = tfile.Tag.Performers[0];
                            }

                            // --- CORRECTION TITRE ---
                            // Si pas de titre dans les tags, on met le nom du fichier
                            string safeTitle = string.IsNullOrWhiteSpace(tfile.Tag.Title)
                                ? Path.GetFileNameWithoutExtension(file.Name)
                                : tfile.Tag.Title;

                            Song newSong = new Song()
                            {
                                Path = file.FullName,
                                Title = safeTitle,
                                Duration = tfile.Properties.Duration,
                                Year = (int)tfile.Tag.Year,
                                album = tfile.Tag.Album ?? "Album Inconnu", // Sécurité null
                                Size = (int)file.Length,

                                // Utilisation de la variable sécurisée
                                Artist = safeArtist,

                                // On garde le tableau complet pour le featuring, mais on vérifie null
                                Featuring = tfile.Tag.AlbumArtists ?? new string[0],

                                Hash = Helper.HashFile(file.FullName),

                                // IMPORTANT : On ajoute l'extension pour le téléchargement futur
                                Extension = file.Extension
                            };

                            ExistSong.Add(newSong);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Impossible de lire le fichier {x} : {ex.Message}");
                        }
                    });

                    var ExistSongJUpdated = JsonSerializer.Serialize(ExistSong, new JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(jsonPath, ExistSongJUpdated);

                    // Vide la liste avant de la remplir
                    ListeSong.Items.Clear();
                    ExistSong.ForEach(x => ListeSong.Items.Add(x.Title));
                }
            }
        }
        private void refresh_Click(object sender, EventArgs e)
        {
            mqttClient.AskOnline();
            string mediaJ = System.IO.File.ReadAllText(jsonPathMedia);

            Mediatheque media = JsonSerializer.Deserialize<Mediatheque>(mediaJ);

            if (media == null)
            {
                return;
            }
            List<string> mediaList = media.mediatheques.ToList();
            string localIp = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
            mediaList.ForEach(x =>
            {
                services.Message msg = new services.Message() { Action = "askCatalog", Sender = localIp, Recipient = x };
                mqttClient.AskCatalog(msg);
            });
            string ExternMusicJ = System.IO.File.ReadAllText(jsonPathCat);
            ListeRemoteSong.Items.Clear();
            var ExternMusic = JsonSerializer.Deserialize<List<Catalog>>(ExternMusicJ);
            if (ExternMusic is not null)
            {
                ExternMusic.ForEach(x =>
                {
                    if (x.Title != null)
                    {
                        ListeRemoteSong.Items.Add(x.Title);
                    }
                });
            }
            
        }

        private void ListeRemoteSong_OnClickItems(object sender, EventArgs e)
        {

        }

        private void ListeSong_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ListeSong_DoubleClick(object sender, EventArgs e)
        {
           

        }

        private void ListeRemoteSong_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Click double download");
            if (ListeRemoteSong.SelectedItem == null) return;

            // 2. Récupérer le titre sur lequel on a cliqué
            string selectedTitle = ListeRemoteSong.SelectedItem.ToString();

            // 3. Charger le fichier Catalog.json pour retrouver les infos techniques (Hash, Holders...)
            string jsonPathCat = Path.Combine(Application.StartupPath, "data", "Catalog.json");

            if (!System.IO.File.Exists(jsonPathCat))
            {
                MessageBox.Show("Catalogue introuvable.");
                return;
            }

            try
            {
                string jsonContent = System.IO.File.ReadAllText(jsonPathCat);
                List<Catalog> globalCatalog = JsonSerializer.Deserialize<List<Catalog>>(jsonContent);

                // 4. Trouver l'objet Catalog qui correspond au titre sélectionné
                // Note : Si deux musiques ont le même titre, cela prendra la première trouvée.
                var songToDownload = globalCatalog.FirstOrDefault(c => c.Title == selectedTitle);

                if (songToDownload != null)
                {
                    // Feedback visuel pour l'utilisateur
                    MessageBox.Show($"Demande de téléchargement envoyée pour : {songToDownload.Title}\nSources disponibles : {songToDownload.Holders.Count}");

                    // 5. Lancer le téléchargement via MQTT
                    mqttClient.DownloadSong(songToDownload);
                }
                else
                {
                    MessageBox.Show("Erreur : Impossible de retrouver les infos de cette musique dans le catalogue.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de lecture du catalogue : {ex.Message}");
            }
        }
    }
}