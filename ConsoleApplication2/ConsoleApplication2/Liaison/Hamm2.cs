using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2.Liaison
{
    public class Hamm2
    {
        public struct retour
        {
            public bool erreur;
            public bool[] tabTrame;
        };

        // Verifie si la trame recue est correcte et retourne un tableau de bool indiquant si les bits de parite sont a la bonne valeur ou non
        private static bool[] verifHamming(bool[] tabverif)
        {
            bool[] taberror = new bool[6];
            // verification pour le premier bit de parité (pour le bit 1)
            int cpt = 0;
            System.Console.WriteLine(" Début de la vérification des bits de parité de la trame reçue");
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
        private static bool[] corrigeHamming(bool[] taberror, bool[] tabTemp)
        {
            int bitErrone = 0;
            bool[] tabDef = new bool[33];
            System.Console.WriteLine(" Correction de l'erreur detectée avec Hamming");
            for (int i = 0; i < 6; i++)
            {
                if (taberror[i])
                {
                    bitErrone += (int)Math.Pow(2.0, (double)i);
                }
                if(bitErrone>0)
                    tabDef[bitErrone - 1] = !tabDef[bitErrone - 1];
            }
            return tabDef;
        }
        // Ajoute les bits de parite a la trame que l'on souhaite envoyer. La taille de la trame passera de 27 bits à 33.
        // Retourne la trame avec Hamming ==> celle a 33 bits
        public static bool[] ajouteHamming(bool[] trameDonnees)
        {
            bool[] trameHamming = new bool[33];
            int i = 2;
            int j = 0;
            System.Console.WriteLine(" Rajout du code de Hamming à la trame");
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
        // Lorsque la trame recue est correcte, on peut donc retirer les bits de parite ajoutes par Hamming, pour remettre la trame dans sa forme originale (27 bits)
        static bool[] enleveHamming(bool[] tabHamming)
        {
            int i = 2;
            int j = 0;
            bool[] tabDefinitif = new bool[27];
            System.Console.WriteLine(" Suppression du code Hamming de la trame");
            for (j = 0; j < 27; i++, j++)
            {
                if ((i == 3) || (i == 7) || (i == 15) || (i == 31) && i < 33) i++;
                tabDefinitif[j] = tabHamming[i];
            }
            return tabDefinitif;
        }

        public static retour Hamming(bool[] tabTemp)
        {
            retour R;
            int cpt = 0;
            bool[] taberror = verifHamming(tabTemp);
            R.erreur = taberror.Any(x => x == true);
            //int adresse = 0;
            //int idThread = 10; // A MODIFIER. Adresse de l'instance de thread
            // On vérifie s'il y a effectivement des erreurs
            System.Console.WriteLine(" La trame recue est: ");
            AfficheTab(tabTemp, 33);
            System.Console.WriteLine(" Les bits de parité faux sont les suivants");
            AfficheTab(taberror, 6);

            if (R.erreur && tabTemp[19]) // on verifie dans la trame si on doit corriger en cas d'erreur ou demander une nouvel envoi
            {
                if (cpt == 1) // S'il n'y a qu'un seul bit de parité qui ne correspond pas, ce qui n'est pas possible, il y a une erreur non corrigeable.
                {
                    R.erreur = true;
                }
                else
                {
                    if (taberror[5]) // Cas spécifique du dernier bit de parité (32), qui ne sert que pour un seul bit de donnée, et qui fonctionne avec le premier bit de parité
                    {
                        if (cpt == 2 && taberror[0]) // Si il n'y a que 2 bits de parité à un et que le second est le n°1, on peut corriger
                        {
                            tabTemp = corrigeHamming(taberror, tabTemp);
                            enleveHamming(tabTemp);
                            R.erreur = false;
                        }
                        else // Sinon, cela n'est pas possible, on se refere a une bit qui n'est pas dans la trame (n° bit > 33)
                        {
                            R.erreur = true;
                        }
                    }
                    else // pour tous les autres cas
                    {
                        tabTemp = corrigeHamming(taberror, tabTemp);
                        enleveHamming(tabTemp);
                        R.erreur = false;
                    }
                }
            }
            if (!R.erreur) // Si pas d'erreurs, on enleve hamming
            {
                enleveHamming(tabTemp);
            }
            R.tabTrame = tabTemp;
            System.Console.WriteLine(" La trame corrigée et nettoyée d'Hamming est: ");
            AfficheTab(tabTemp, 27);
            return R;
        }
        public static void AfficheTab(bool[] tab, int tailleTab)
        {
            String s = " ";
            for (int i = 0; i < tailleTab; i++)
            {
                s += tab[i] + " | ";
            }
            System.Console.WriteLine(s);
        }
    }
}
