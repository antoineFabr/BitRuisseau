using BitRuisseau.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau.services
{
    public class mqtt_client : IProtocol
    {
        public string[] GetOnlineMediatheque()
        {
            string[] mediatheque = new string[3];
            return mediatheque;
        }

        public void SayOnline()
        {

        }

        public List<ISong> AskCatalog(string name)
        {
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
