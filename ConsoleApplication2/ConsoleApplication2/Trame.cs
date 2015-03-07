using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    public class Trame
    {
        /* Composition trame :
        * Type : 2 bits (données 00, ack 01, nack 10, données dernière trame 00)
        * Taille données : 4 bits
        * Numéro trame : 8 bits
        * Code Hamming (détection ou correction?): 1 bit
        * Adresse source : 2bits
        * Adresse de destination : 2bits
        * Données : 8bits
        */

        public enum TypeTrame
        {
            Data = 0,
            Ack = 1,
            Nack = 2,
            End = 3
        }
        private static readonly int longueur = 27; //trame sans code de Hamming
        private bool[] trame;
        public Trame(bool[] t)
        {
            trame = new bool[longueur];
            t.CopyTo(trame, 0);
        }
        public Trame(TypeTrame type, int taille, int numero, bool correction, int adrSrc, int adrDst, bool[] donnee)
        {
            // Todo: Constructeur qui en appel un autre
            trame = new bool[longueur];
            Transtypage.IntegerToBits((int)type, 2).CopyTo(trame, 0);
            Transtypage.IntegerToBits(taille, 4).CopyTo(trame, 2);
            Transtypage.IntegerToBits(numero, 8).CopyTo(trame, 6);
            trame[14] = correction;
            Transtypage.IntegerToBits(adrSrc, 2).CopyTo(trame, 15);
            Transtypage.IntegerToBits(adrDst, 2).CopyTo(trame, 17);
            donnee.CopyTo(trame, 19);
        }
        public Trame(TypeTrame type, int taille, int numero, bool correction, int adrSrc, int adrDst, char donnee)
        {
            trame = new bool[longueur];
            Transtypage.IntegerToBits((int)type, 2).CopyTo(trame, 0);
            Transtypage.IntegerToBits(taille, 4).CopyTo(trame, 2);
            Transtypage.IntegerToBits(numero, 8).CopyTo(trame, 6);
            trame[14] = correction;
            Transtypage.IntegerToBits(adrSrc, 2).CopyTo(trame, 15);
            Transtypage.IntegerToBits(adrDst, 2).CopyTo(trame, 17);
            Transtypage.IntegerToBits((int)donnee, 8).CopyTo(trame, 19);
        }

        //On affiche chaque bool en valeur numérique (0 ou 1) et pas false/true
        public override String ToString()
        {
            String affichage = "Trame : ";
            foreach (var bit in trame)
            {
                if (bit)
                    affichage += "1 ";
                else
                    affichage += "0 ";
            }
            return affichage;
        }

        //Deux bits sont réservés pour le type de la trame (donnée,Ack,Nack ou Fin)
        public TypeTrame GetTypeTrame()
        {
            TypeTrame type = 0;
            type = (TypeTrame)Transtypage.BitsToInteger(trame, 0, 2);
            return type;
        }
        //Quatre bits sont réservés pour la taille de donnée à l'intérieur d'une trame (max = 15 bits)
        public int GetTailleDonnees()
        {
            int taille = 0;
            taille = Transtypage.BitsToInteger(trame, 2, 4);
            return taille;
        }

        //Huit bits sont réservés pour le nombre de trames à envoyer, on peut donc gérer un nombre conséquent de trame
        //Cela compense le fait que les données soient réduites (max = 255 trames) 
        public int GetNumero()
        {
            int numero = 0;
            numero = Transtypage.BitsToInteger(trame, 6, 8);
            return numero;
        }
        //Un bit est reserve pour savoir si le code est detecteur ou correcteur d'erreur
        public bool IsCorrecteur()
        {
            return trame[14];
        }

        //Quatre bits sont reserves pour les adresses, les deux premiers sont pour la source...
        public int GetAdrSource()
        {
            int adresse = 0;
            adresse = Transtypage.BitsToInteger(trame, 15, 2);
            return adresse;
        }
        //... et les deux derniers pour la destination. Cependant, un seul bit pour chaque adresse aurait suffit.
        public int GetAdrDestination()
        {
            int adresse = 0;
            adresse = Transtypage.BitsToInteger(trame, 17, 2);
            return adresse;
        }
        //Les données dans notre cas sont sur huit bits, mais cela pour changer
        public int GetDonnees()
        {
            int donnee = 0;
            donnee = Transtypage.BitsToInteger(trame, 19, 8);
            return donnee;
        }
        public bool[] ToBool()
        {
            bool[] copie = new bool[longueur];
            trame.CopyTo(copie, 0);
            return copie;
        }
    }
}
