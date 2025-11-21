using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitRuisseau.data;

namespace BitRuisseau.services
{
    public interface IProtocol
    {
        /// <summary>
        /// Get the list of all online mediatheque
        /// </summary>
        /// <param name="msg">Le message qu'on reçoit dans le canal BitRuisseau</param>
        /// <returns>An array of mediatheque name/ip</returns>
        public string[] GetOnlineMediatheque(Message msg);

        /// <summary>
        /// Send an "I'm online" message: {"name": "ip", "online": true}
        /// </summary>
        public void SayOnline();

        /// <summary>
        /// Ask for the catalog of a specific mediatheque {"des": "ip", "em": "ip", "action": "askCatalog"}
        /// </summary>
        /// <param name="msg">Le message qu'on reçoit dans le canal BitRuisseau</param>
        /// <returns>A list of songs</returns>
        public List<ISong> AskCatalog(Message msg);

        /// <summary>
        /// Send our catalog to a specific mediatheque {"des": "ip", "em": "ip", "songs": []}
        /// </summary>
        /// <param name="name">The name/ip of the mediatheque</param>
        public void SendCatalog(string name);

        /// <summary>
        /// Download the media from a mediatheque {"des": "ip", "em": "ip", "action": "askMedia", "sb": 1, "eb", 2}
        /// </summary>
        /// <param name="msg">Le message qu'on reçoit dans le canal BitRuisseau</param>
        public void AskMedia(Message msg);

        /// <summary>
        /// Send the media to someone {"des": "ip", "em": "ip", "action": "sendMedia", "sb": 1, "eb", 2, "data": []}/*bytesarray*/
        /// </summary>
        /// <param name="startByte">The first byte they need</param>
        /// <param name="endByte">The last byte they need</param>
        /// <param name="name">The name/ip of the mediatheque</param>
        public void SendMedia(string name, int startByte, int endByte);
    }
}
