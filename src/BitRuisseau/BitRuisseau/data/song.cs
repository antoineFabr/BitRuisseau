using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau.data
{
    public class Song
    {
        public string path { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public TimeSpan duréeSeconde { get; set; }


    }
}
