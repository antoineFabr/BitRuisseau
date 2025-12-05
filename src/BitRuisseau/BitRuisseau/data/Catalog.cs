using System.Collections.Generic;

namespace BitRuisseau.data
{
    // Le Catalog devient une "Fiche chanson" enrichie
    public class Catalog : Song
    {
        // La liste des IP/Noms des gens qui ont cette musique
        public List<string> Holders { get; set; } = new List<string>();

        // On enlève les anciennes propriétés 'sons' et 'holder' unique
    }
}