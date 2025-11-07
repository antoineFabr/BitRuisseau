using BitRuisseau.data;
using TagLib;
using System.Text.Json;
using System.Text.Json.Serialization;
using BitRuisseau.services;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string ExistSongJ = System.IO.File.ReadAllText(jsonPath);
            List<Song> ExistSong = Protocol.GetSongs();

            ExistSong.ForEach(x => ListeSong.Items.Add(x.Title));
        }
        string jsonPath = Path.Combine(Application.StartupPath, "data", "song.json");
        private void Load_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Fichier audio|*.mp3;*.wav";
                ofd.Multiselect = true;

                string[] extensions = { ".mp3", ".wav" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var songs = ofd.FileNames.ToList();
                    string ExistSongJ = System.IO.File.ReadAllText(jsonPath);
                    List<Song> ExistSong = Protocol.GetSongs();


                    songs.ForEach(x =>
                    {
                        FileInfo file = new FileInfo(x);
                        var tfile = TagLib.File.Create(x);
                        string name = Path.GetFileNameWithoutExtension(file.Name);
                        Song newSong = new Song() { 
                            path = file.FullName, 
                            Title = name, 
                            Duration = tfile.Properties.Duration,  
                            Artist = tfile.Tag.AlbumArtists[0], 
                            Featuring = tfile.Tag.AlbumArtists, 
                            Size= (int)file.Length, 
                            Year =  (int)tfile.Tag.Year 
                        };
                        ExistSong.Add(newSong);
                    });


                    var newSongJ = JsonSerializer.Serialize(ExistSong);
                    System.IO.File.WriteAllText(jsonPath, newSongJ);
                    // Vide la liste avant de la remplir
                    ListeSong.Items.Clear();

                    ExistSong.ForEach(x => ListeSong.Items.Add(x.Title));
                }
            }
        }
    }
}