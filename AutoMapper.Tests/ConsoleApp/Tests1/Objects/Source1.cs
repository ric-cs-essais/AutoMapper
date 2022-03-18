using System;

namespace ConsoleApp
{
    internal class TypeAutreSource
    {
        public string sChamp1 { get; set; }
    }

    internal class TypeAutreSource2
    {
        public double oneDouble3 { get; init; }
    }

    internal class Source1
    {
        public DateTime dateTime { get; init; }

        public string sValeurDirecte { get; init; }

        public string memeNomPasMemeType { get; set; }
        
        public int autreValeur { get; init; }
        public int autreValeur2 { get; set; }

        public string ValeurDeTypeAConvertir { get; init; }

        public TypeAutreSource oTypeAutre { get; init; }

        public double oneDouble1 { get; init; }
        public double oneDouble2 { get; init; }

        public TypeAutreSource2 oTypeAutre2 { get; init; }

        public float oneFloat;

        public int toSomme1;
        public int toSomme2;

        public int poids;

        public int unEntier;
        public bool reponse;

    }
}
