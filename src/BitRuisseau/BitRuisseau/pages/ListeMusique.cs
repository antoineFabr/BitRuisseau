using BitRuisseau.data;
using TagLib;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string jsonPath = Path.Combine(Application.StartupPath, "data", "song.json");
        private void Form1_Load(object sender, EventArgs e)
        {

        }

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
                    List<Song> ExistSong;
                    if (string.IsNullOrWhiteSpace(ExistSongJ))
                    {
                        ExistSong = new List<Song>();
                    }
                    else
                    {
                        ExistSong = JsonSerializer.Deserialize<List<Song>>(ExistSongJ);
                    }
                    songs.ForEach(x =>
                    {
                        FileInfo file = new FileInfo(x);
                        var tfile = TagLib.File.Create(x);


                        string name = Path.GetFileNameWithoutExtension(file.Name);
                        Song newSong = new Song() { path = file.FullName, name = name, duréeSeconde = tfile.Properties.Duration, album = tfile.Tag.Album, artist = tfile.Tag.FirstPerformer};
                       

                        ExistSong.Add(newSong);
                        var newSongJ = JsonSerializer.Serialize(newSong);
                        System.IO.File.WriteAllText(jsonPath, newSongJ);

                    });

               
                    
                    // Vide la liste avant de la remplir
                    ListeSong.Items.Clear();
                    string SongJ = System.IO.File.ReadAllText(jsonPath);
                    List<Song> NewSong = JsonSerializer.Deserialize<List<Song>>(ExistSongJ);
                    ExistSong.ForEach(x => ListeSong.Items.Add(x.name));




                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}