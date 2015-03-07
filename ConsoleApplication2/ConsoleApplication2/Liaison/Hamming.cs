using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2.Liaison
{
    public class Hamming
    {

        public struct retour
        {
            public bool erreur;
            public bool[] tabTrame;
        };

        /// <summary>
        ///  Verifie si la trame recue est correcte et retourne un tableau de bool indiquant si les bits de parite sont a la bonne valeur ou non
        /// </summary>
        /// <param name="tabverif"></param>
        /// <returns>tableau de 6 bool (6 bit de parité): Si all true: Valide! on peux passer a enleveHamming. Sinon: il y a une erreur.</returns>
        public static bool[] verifHamming(bool[] tabverif)
        {
            bool[] taberror = new bool[6];
            // verification pour le premier bit de parité (pour le bit 1)
            int cpt = 0;
            for (int i = 2; i < 33; i += 2)
            {
                if (tabverif[i]) cpt++;
            }

            if (!((((cpt % 2) == 1) && tabverif[0]) || (((cpt % 2) == 0) && !tabverif[0]))) // Premier bit de partié correct
            {
                taberror[0] = true;
            }


            // 2e bit de parité ==> tout ce qui est 1 ou 2 modulo 4
            cpt = 0;
            for (int i = 2; i < 33; i++)
            {
                if (i % 4 == 1 || i % 4 == 2)
                {
                    if (tabverif[i]) cpt++;
                }
            }

            if (!((((cpt % 2) == 1) && tabverif[1]) || (((cpt % 2) == 0) && !tabverif[1]))) // Second bit de parité correct
            {
                taberror[1] = true;
            }
            // 3e bit de parité (4) ==> tout ce qui vaut 3,4,5,6 modulo 8
            cpt = 0;
            for (int i = 4; i < 33; i++)
            {
                if (i % 8 == 3 || i % 8 == 4 || i % 8 == 5 || i % 8 == 6)
                {
                    if (tabverif[i]) cpt++;
                }
            }

            if (!((((cpt % 2) == 1) && tabverif[3]) || (((cpt % 2) == 0) && !tabverif[3]))) // Troisieme bit de parité correct
            {
                taberror[2] = true;
            }

            // 4e bit de parité (8) ==> tout ce qui vaut 7 -> 14 modulo 16
            cpt = 0;
            for (int i = 8; i < 33; i++)
            {
                if (i % 16 == 7 || i % 16 == 8 || i % 16 == 9 || i % 16 == 10 || i % 16 == 11 || i % 16 == 12 || i % 16 == 13 || i % 16 == 14)
                {
                    if (tabverif[i]) cpt++;
                }
            }

            if (!((((cpt % 2) == 1) && tabverif[7]) || (((cpt % 2) == 0) && !tabverif[7]))) // Quatrième bit de parité correct
            {
                taberror[3] = true;
            }

            // 5e bit de parité (16) ==> tout ce qui vaut 15 -> 30 modulo 32
            cpt = 0;
            for (int i = 16; i < 31; i++)
            {
                if (tabverif[i]) cpt++;
            }

            if (!((((cpt % 2) == 1) && tabverif[15]) || (((cpt % 2) == 0) && !tabverif[15]))) // Quatrième bit de parité correct
            {
                taberror[4] = true;
            }
            // 6e bit de parité (32) ==> tout ce qui vaut 15 -> 30 modulo 32
            if (tabverif[32] != tabverif[31])
            {
                taberror[5] = true;
            }
            return taberror;
        }

        public static bool[] corrigeHamming(bool[] taberror, bool[] tabTemp)
        {
            int bitErrone = 0;
            bool[] tabDef = new bool[33];
            for (int i = 0; i < 6; i++)
            {
                if (taberror[i])
                {
                    bitErrone += (int)Math.Pow(2.0, (double)i);
                }
                if(bitErrone!=0)
                    tabDef[bitErrone - 1] = !tabDef[bitErrone - 1];
            }
            return tabDef;
        }


        /// <summary>
        /// Ajoute les bits de parite a la trame que l'on souhaite envoyer. 
        /// </summary>
        /// <param name="trameDonnees">une trame de 27 bit SANS hamming</param>
        /// <returns>une tramme de 33 bit avec haming</returns>
        public static bool[] ajouteHamming(bool[] trameDonnees)
        {
            bool[] trameHamming = new bool[33];
            int i = 2;
            int j = 0;
            for (i = 2; j < 27; i++, j++)
            {
                if ((i == 3 || i == 7 || i == 15 || i == 31) && i < 33) i++;
                trameHamming[i] = trameDonnees[j];
            }

            // verification pour le premier bit de parité (pour le bit 1) ==> un bits au numero impair (bits n° 3,5,7,9 ...)
            int cpt = 0;
            for (i = 2; i < 33; i += 2)
            {
                if (trameHamming[i]) cpt++;
            }
            if (cpt % 2 == 1) trameHamming[0] = true;

            // 2e bit de parité ==> tout ce qui est 1 ou 2 modulo 4
            cpt = 0;
            for (i = 2; i < 33; i++)
            {
                if (i % 4 == 1 || i % 4 == 2)
                {
                    if (trameHamming[i]) cpt++;
                }
            }
            if (cpt % 2 == 1) trameHamming[1] = true;

            // 3e bit de parité (4) ==> tout ce qui vaut 3,4,5,6 modulo 8
            cpt = 0;
            for (i = 4; i < 33; i++)
            {
                if (i % 8 == 3 || i % 8 == 4 || i % 8 == 5 || i % 8 == 6)
                {
                    if (trameHamming[i]) cpt++;
                }
            }
            if (cpt % 2 == 1) trameHamming[3] = true;


            // 4e bit de parité (8) ==> tout ce qui vaut 7 -> 14 modulo 16
            cpt = 0;
            for (i = 8; i < 33; i++)
            {
                if (i % 16 == 7 || i % 16 == 8 || i % 16 == 9 || i % 16 == 10 || i % 16 == 11 || i % 16 == 12 || i % 16 == 13 || i % 16 == 14)
                {
                    if (trameHamming[i]) cpt++;
                }
            }
            if (cpt % 2 == 1) trameHamming[7] = true;


            // 5e bit de parité (16) ==> tout ce qui vaut 15 -> 30 modulo 32
            cpt = 0;
            for (i = 16; i < 31; i++)
            {
                if (trameHamming[i]) cpt++;
            }
            if (cpt % 2 == 1) trameHamming[15] = true;

            // 6e bit de parité (32) ==> Ne s'occupe que du dernier bit de la trame
            trameHamming[31] = trameHamming[32];

            return trameHamming;
        }


        /// <summary>
        /// Lorsque la trame recue est correcte, on peut retirer les bits de parite ajoutes par Hamming, pour remettre la trame dans sa forme originale (27 bits)
        /// </summary>
        /// <param name="tabHamming">une trame avec les bit de haming quon souhaite enlever</param>
        /// <returns>sans bit de hamming (27 bit)</returns>
        public static void enleveHamming(bool[] tabHamming, bool[] tabReception)
        {
            int i = 2;
            int j = 0;
            bool[] tabDefinitif = new bool[27];
            for (j = 0; j < 27; i++, j++)
            {
                if ((i == 3) || (i == 7) || (i == 15) || (i == 31) && i < 33) i++;
                tabDefinitif[j] = tabHamming[i];
            }
            for (int h = 0; h < Math.Min(tabReception.Length, tabDefinitif.Length); h++)
            {
                tabReception[h] = tabDefinitif[h];
            }
        }


        /// <summary>
        /// Retorune si il y a erreur ou non et corrige si corrigble (selon bit 19)
        /// </summary>
        /// <param name="tabTemp">33 bit</param>
        /// <returns></returns>
        public static retour HammingReception(bool[] tabTemp)
        {
            retour R;
            bool[] taberror = verifHamming(tabTemp);
            bool[] tabFinale = new bool[27];
            R.erreur = taberror.Any(x => x == true);
            int QteErreurs = taberror.Count(x => x == true);
            bool Corrigeable = false;

            if (QteErreurs > 0 && tabTemp[19])  // on verifie dans la trame si on doit corriger en cas d'erreur
            {
                if (QteErreurs != 1)
                {
                    if (taberror[5])
                    {
                        if (QteErreurs == 2 && taberror[0])
                        {
                            Corrigeable = true;
                        }
                    }
                    else
                    {
                        Corrigeable = true;
                    }
                }
            }

            if (Corrigeable)
            {
                tabTemp = corrigeHamming(taberror, tabTemp);
                enleveHamming(tabTemp,tabFinale);
                R.erreur = false;
            }

            if (!R.erreur)
            {
                enleveHamming(tabTemp,tabFinale);
            }
            R.tabTrame = new bool[tabFinale.Length];
            for (int i = 0; i < tabFinale.Length; i++)
            {
                R.tabTrame[i] = tabFinale[i];
            }
            return R;
        }
    }
}
