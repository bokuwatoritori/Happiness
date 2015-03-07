using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Transtypage
    {
        //Nous permet de visualiser la valeur de 8 bits en int 
        public static int BitsToInteger(bool[] bits, int debut, int taille)
        {
            int resultat = 0;
            int puissance = 1;
            if (taille > sizeof(int) * 8)
            {
                //Console.Error.WriteLine("Erreur.");
                throw new ArgumentOutOfRangeException("Nombre de bits à convertir supérieur à la taille d'un int.");
            }
            for (int i = 0; i < taille; i++)
            {
                if (bits[debut + taille - 1 - i])
                    resultat += puissance;
                puissance *= 2;
            }
            return resultat;
        }

        //Facilite la transformation d'un caractère en bit, il suffit de le transtyper en Int puis de le passer en bool
        public static bool[] IntegerToBits(int valeur, int taille)
        {
            int reste = valeur;
            int cnt;
            List<bool> bits = new List<bool>();
            bool bit = false;
            for (cnt = 0; reste != 0 && cnt < taille; cnt++)
            {
                if (reste % 2 == 1)
                    bit = true;
                else
                    bit = false;
                bits.Insert(0, bit);
                reste /= 2;
            }
            while (cnt < taille)
            {
                bits.Insert(0, false);
                cnt++;
            }
            return bits.ToArray();
        }
    }
}
