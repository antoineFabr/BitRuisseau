using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau.data
{
    public class Song: ISong
    {
        public string Path { get; set; }
        public string Title { get; set; }

        public string Artist { get; set; }
        public int Year { get; set; }
        public int Size { get; set; }
        public string[] Featuring { get; set; }
        public string Hash { get; set; }

        public string album { get; set; }
        public TimeSpan Duration { get; set; }



    }
}
