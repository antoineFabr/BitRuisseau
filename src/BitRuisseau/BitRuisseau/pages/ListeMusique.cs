using BitRuisseau.data;
using BitRuisseau.services;
using MQTTnet;
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
            List<Song> ExistSong = Protocol.GetSongs();
            Connect();
            ExistSong.ForEach(x => ListeSong.Items.Add(x.Title));
        }
        string jsonPath = Path.Combine(Application.StartupPath, "data", "song.json");

        private async void Connect()
        {
            mqtt_client mqttClient = new mqtt_client();

            bool isConnect = await mqttClient.ConnectToBroker();

            if (!isConnect)
            {
                MessageBox.Show("Impossible de se connecter au broker mqtt");
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog ofd = new FolderBrowserDialog())
            {


                string[] extensions = { ".mp3", ".wav" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = ofd.SelectedPath;

                    var songs = Directory.GetFiles(selectedFolder, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.EndsWith(".wav") || f.EndsWith(".mp3"))
                        .ToList();
                    string ExistSongJ = System.IO.File.ReadAllText(jsonPath);
                    List<Song> ExistSong = Protocol.GetSongs();


                    songs.ForEach(x =>
                    {
                        FileInfo file = new FileInfo(x);
                        var tfile = TagLib.File.Create(x);
                        string name = Path.GetFileNameWithoutExtension(file.Name);
                        Song newSong = new Song()
                        {
                            Path = file.FullName,
                            Title = tfile.Tag.Title,
                            Duration = tfile.Properties.Duration,
                            Year = Convert.ToInt32(tfile.Tag.Year),
                            album = tfile.Tag.Album,
                            Size = (int)file.Length,
                            Artist = tfile.Tag.AlbumArtists[0],

                        };


                        ExistSong.Add(newSong);


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
            mqtt_client protocol = new mqtt_client();
            // on récupere toutes les ip pour demander le catalogue
            List<string> med = protocol.GetOnlineMediatheque().ToList();

            List<ISong> Songs = new List<ISong>();
            // pour chaque IP on lui demande son catalogue et on ajoute chaque musique à la liste des songs remote
            med.ForEach(x =>
            {
                List<ISong> newSongs = protocol.AskCatalog(x);

                newSongs.ForEach(x => Songs.Add(x));
            });

            ListeRemoteSong.Items.Clear();
            Songs.ForEach(x => ListeRemoteSong.Items.Add(x.Title));
        }

        private void ListeRemoteSong_OnClickItems(object sender, EventArgs e)
        {

        }

        private void ListeSong_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ListeSong_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("click");

        }
    }
}