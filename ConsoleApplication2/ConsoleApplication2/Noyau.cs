using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    static class Noyau
    {
        static private bool rejet;
        static private bool correcteur;
        static private int tailleFen;
        static private int nbEnvois;
        static private int nbTotal = 1;
        static public int increment = 0;


        static private float affaiblissement;
        static private float interference;
        static private float dedoublement;
        static private float pretard;
        static private float perte;


        static public bool[] envoieSource;
        static public bool[] receptionDestination;
        static public bool pretEmettre=true;
        static public bool donneRecue=false;
        static public string adresselecture = "fichier.txt";
        static public string adresseecriture = "fichier2.txt";
        static public Semaphore mutex1 = new Semaphore(1, 1);
        static public Semaphore mutex2 = new Semaphore(1, 1);
        static public Semaphore synchcond1 = new Semaphore(0, 1);
        static public Semaphore synchcond2 = new Semaphore(1, 1);
        static public bool hammingCorrecteur = false;
        static public System.IO.StreamWriter outputFile;

        public static void Run()
        {
            int reponseint;
            bool repondu = false;
            string texteEnvoi;
            Liaison.CoucheLiaison emetteur = null;
            Liaison.CoucheLiaison recepteur = null;

            Physique.CouchePhysique couchePhy = null;

            while (!repondu)
            {
                Console.WriteLine("Bienvenue à cette simulation de communication entre 2 couches liaison.");
                Console.WriteLine("Souhaitez vous définir vos parametres de la couche liaison ? (Y/N)");
                reponseint = Console.Read();
                try
                {
                    if ((char)reponseint == 'Y' || (char)reponseint == 'y')
                    {
                        repondu = true;
                        Console.WriteLine("Veuillez entrer l'adresse du fichier lecture");

                        texteEnvoi = Console.ReadLine();
                        while (texteEnvoi == null && !File.Exists(texteEnvoi))
                        {
                            Console.WriteLine("Erreur, entrer l'adresse du fichier lecture : ");
                            texteEnvoi = Console.ReadLine();
                        }
                        Noyau.adresselecture = texteEnvoi;
                        texteEnvoi = null;

                        Console.WriteLine("Veuillez entrer l'adresse du fichier d'ecriture : ");
                        texteEnvoi = Console.ReadLine();
                        while (texteEnvoi == null)
                        {
                            Console.WriteLine("Erreur, entrer l'adresse du fichier d'ecriture : ");
                            texteEnvoi = Console.ReadLine();
                        }
                        adresseecriture = texteEnvoi;
                        texteEnvoi = null;

                        Console.WriteLine("Souhaitez vous utiliser un code correcteur d'erreur (Hamming) ? (Y/N)");
                        reponseint = Console.Read();
                        while (Convert.ToChar(reponseint) != 'Y' || Convert.ToChar(reponseint) != 'y' || Convert.ToChar(reponseint) != 'N' || Convert.ToChar(reponseint) != 'n')
                        {
                            Console.WriteLine("Erreur, souhaitez vous utiliser un code correcteur d'erreur (Hamming) ? (Y/N)");
                            reponseint = Console.Read();
                        }
                        if (Convert.ToChar(reponseint) != 'Y' || Convert.ToChar(reponseint) != 'y')
                        {
                            correcteur = true;
                            Console.WriteLine("Utilisation d'un code correcteur d'erreur");
                        }
                        else
                        {
                            correcteur = false;
                            Console.WriteLine("Utilisation d'un code detecteur d'erreur");
                        }

                        Console.WriteLine("Souhaitez vous utiliser un rejet global ? (Y/N)");
                        reponseint = Console.Read();
                        while (Convert.ToChar(reponseint) != 'Y' || Convert.ToChar(reponseint) != 'y' || Convert.ToChar(reponseint) != 'N' || Convert.ToChar(reponseint) != 'n')
                        {
                            Console.WriteLine("Erreur, souhaitez vous utiliser un rejet global ? (Y/N)");
                            reponseint = Console.Read();
                        }
                        if (Convert.ToChar(reponseint) != 'Y' || Convert.ToChar(reponseint) != 'y')
                        {
                            rejet = true;
                            Console.WriteLine("Utilisation au rejet global");
                        }
                        else
                        {
                            rejet = false;
                            Console.WriteLine("Utilisation au rejet selectif");
                        }

                        Console.WriteLine("Quelle taille de fenetre souhaitez vous utiliser ? Choisissez un nombre superieur a 0");
                        reponseint = Console.Read();
                        while (reponseint < 0 || reponseint == null)
                        {
                            Console.WriteLine("Erreur, veuillez choisir un nombre superieur a 0");
                            reponseint = Console.Read();
                        }
                        tailleFen = reponseint;

                        Console.WriteLine("Quel nombre maximal de tentatives de renvoi souhaitez vous ? Choisissez un nombre superieur a 0");
                        reponseint = Console.Read();
                        while (reponseint < 0 || reponseint == null)
                        {
                            Console.WriteLine("Erreur, veuillez choisir un nombre superieur a 0");
                            reponseint = Console.Read();
                        }
                        nbEnvois = reponseint;


                        emetteur = new Liaison.CoucheLiaison(true, correcteur, rejet, nbEnvois, tailleFen);
                        recepteur = new Liaison.CoucheLiaison(false, correcteur, rejet, nbEnvois, tailleFen);

                    }
                    else if (Convert.ToChar(reponseint) == 'N' || Convert.ToChar(reponseint) == 'n')
                    {
                        repondu = true;
                        emetteur = new Liaison.CoucheLiaison(true, true, true);
                        recepteur = new Liaison.CoucheLiaison(false);
                    }
                }
                catch (OverflowException) { }
            }

            // parametrisation couche physique
            repondu = false;
            while (!repondu)
            {
                Console.WriteLine("Souhaitez vous définir vos parametres de la couche physique ? (Y/N)");
                reponseint = Console.Read();
                if (Convert.ToChar(reponseint) == 'Y' || Convert.ToChar(reponseint) == 'y')
                {
                    repondu = true;
                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour l'affaiblissement");
                    texteEnvoi = Console.ReadLine();
                    while (texteEnvoi == null && !(float.Parse(texteEnvoi) <= 1.0 && float.Parse(texteEnvoi) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                    affaiblissement = float.Parse(texteEnvoi);
                    texteEnvoi = null;

                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour l'interference : ");
                    texteEnvoi = Console.ReadLine();
                    while (texteEnvoi == null && !(float.Parse(texteEnvoi) <= 1.0 && float.Parse(texteEnvoi) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                    interference = float.Parse(texteEnvoi);
                    texteEnvoi = null;

                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour le dedoublement : ");
                    texteEnvoi = Console.ReadLine();
                    while (texteEnvoi == null && !(float.Parse(texteEnvoi) <= 1.0 && float.Parse(texteEnvoi) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                    dedoublement = float.Parse(texteEnvoi);
                    texteEnvoi = null;

                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour le retard : ");
                    texteEnvoi = Console.ReadLine();
                    while (texteEnvoi == null && !(float.Parse(texteEnvoi) <= 1.0 && float.Parse(texteEnvoi) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                    pretard = float.Parse(texteEnvoi);
                    texteEnvoi = null;

                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour la perte : ");
                    texteEnvoi = Console.ReadLine();
                    while (texteEnvoi == null && !(float.Parse(texteEnvoi) <= 1.0 && float.Parse(texteEnvoi) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                    perte = float.Parse(texteEnvoi);
                    texteEnvoi = null;

                    couchePhy = new Physique.CouchePhysique(affaiblissement, interference, dedoublement, pretard, perte);

                }
                else if(Convert.ToChar(reponseint) == 'N' || Convert.ToChar(reponseint) == 'n')
                {
                    repondu = true;
                    couchePhy = new Physique.CouchePhysique(0,0,0,0,0);
                }
            }
            outputFile = new System.IO.StreamWriter(adresseecriture);
            outputFile.AutoFlush = true;
            Thread couchePhyThread = new Thread(couchePhy.boucle);
            couchePhyThread.Start();
            Thread TEmetteur = new Thread(emetteur.boucle);
            Thread TRecepteur = new Thread(recepteur.boucle);
            TEmetteur.Start();
            TRecepteur.Start();
        }

        public static int ligneFichierLecture()
        {
            string[] str;
            str = File.ReadAllLines(adresselecture);
            int compteur = 0;
            for (int i = 0; i < str.Length; i++)
            {
                compteur += str[i].Length;
            }
            return compteur;
        }


        public static void lireFichier(char[] retour)
        {
            string[] str;
            str = File.ReadAllLines(adresselecture);
            Console.WriteLine("Voici le contenu du fichier");
            nbTotal = ligneFichierLecture();
            int compteur = 0;
            char[] test;
            for (int i = 0; i < str.Length; i++)
            {
                Console.WriteLine(str[i]);
                test = str[i].ToCharArray();
                for (int j = 0; j < test.Length; j++)
                {
                    retour[j + compteur] = test[j];
                }
                compteur += str[i].Length;
            }
        }

        public static void ecrireFichier(bool[] trame)
        {
            increment++;
            int res = (increment*100) / nbTotal;
            string str = "";
            for (int i = 0; i < trame.Length; i++)
            {
                if (trame[i])
                {
                    str += "1";
                }
                else
                {
                    str += "0";
                }
            }
                Console.WriteLine("Ecriture dans le fichier de : " + str + " = [" + res + "%]");
            outputFile.WriteLine(str);
        }

    }
}
