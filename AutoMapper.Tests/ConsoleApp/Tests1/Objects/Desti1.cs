using System;

namespace ConsoleApp
{

    internal class TypeAutreDesti
    {
        public string iChampX { get; set; }
    }

    internal class TypeAutreDesti2
    {
        public string oneDouble3 { get; set; }
    }

    internal class Desti1
    {
        public int hour { get; set; }
        public int minutes { get; set; }
        public int seconds { get; set; }

        public string sValeurDirecte { get; set; }
        public int memeNomPasMemeType { get; set; }

        public int AutreValeur { get; set; } = 55;
        public int Autre_Valeur2 { get; set; }

        public int monAutreValeur { get; set; } = 888;

        public int ValeurDeTypeAConvertir { get; set; }

        public TypeAutreDesti oTypePerso { get; set; }

        public string oneDouble1 { get; set; }
        public string oneDouble2 { get; set; }

        //public TypeAutreDesti2 oTypeAutre2 { get; set; }

        public string oneFloat;

        public int maSomme;

        public string poids;

        public string unEntier; //La conversion d'un nombre en string est automatique
        public string reponse; //La conversion d'un bool en string est automatique
    }
}
