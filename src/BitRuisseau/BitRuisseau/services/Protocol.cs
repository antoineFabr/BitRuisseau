using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitRuisseau.data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BitRuisseau.services
{
    public class Protocol : IProtocol
    {
        public string[] GetOnlineMediatheque()
        {
            return new string[0];
        }

        public void SayOnline()
        {
            return;
        }
        public List<ISong> AskCatalog(string name)
        {
            

            return new List<ISong>();
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
