using System;
using System.Collections.Generic;
using System.Globalization;
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


        static private float affaiblissement = 0;
        static private float interference = 0;
        static private float dedoublement = 0;
        static private float pretard = 0;
        static private float perte = 0;


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

            Console.WriteLine("Bienvenue à cette simulation de communication entre 2 couches liaison.");
            while (!repondu)
            {
                Console.WriteLine("Souhaitez vous définir vos parametres de la couche liaison ? (Y/N)");
                texteEnvoi = Console.ReadLine();
                if (texteEnvoi.ToUpper().Equals("Y"))
                {
                    Console.WriteLine("Veuillez entrer l'adresse du fichier lecture : ");
                        texteEnvoi = Console.ReadLine();
                    while (texteEnvoi == null || !File.Exists(texteEnvoi))
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

                    repondu = false;
                        Console.WriteLine("Souhaitez vous utiliser un code correcteur d'erreur (Hamming) ? (Y/N)");
                    while (!repondu)
                        {
                        texteEnvoi = Console.ReadLine();
                        if (texteEnvoi.ToUpper().Equals("Y"))
                        {
                            correcteur = true;
                            Console.WriteLine("Utilisation d'un code correcteur d'erreur");
                        }
                        else if (texteEnvoi.ToUpper().Equals("N"))
                        {
                            correcteur = false;
                            Console.WriteLine("Utilisation d'un code detecteur d'erreur");
                        }
                        else if (!(texteEnvoi.ToUpper().Equals("Y")) || !(texteEnvoi.ToUpper().Equals("N")))
                        {
                            Console.WriteLine("Erreur, souhaitez vous utiliser un code correcteur d'erreur (Hamming) ? (Y/N)");
                            texteEnvoi = Console.ReadLine();
                        }
                        reponseint = 0;
                        repondu = true;
                    }

                     
                    repondu = false;    
                        Console.WriteLine("Souhaitez vous utiliser un rejet global ? (Y/N)");
                    while(!repondu)
                        {
                        texteEnvoi = Console.ReadLine();
                        if (texteEnvoi.ToUpper().Equals("Y"))
                        {
                            rejet = true;
                            Console.WriteLine("Utilisation au rejet global");
                        }
                        else if(texteEnvoi.ToUpper().Equals("N"))
                        {
                            rejet = false;
                            Console.WriteLine("Utilisation au rejet selectif");
                        }
                        else
                        {
                            Console.WriteLine("Erreur, souhaitez vous utiliser un rejet global ? (Y/N)");
                            texteEnvoi = Console.ReadLine();
                        }
                        repondu = true;
                    }

                    repondu = false;
                        Console.WriteLine("Quelle taille de fenetre souhaitez vous utiliser ? Choisissez un nombre superieur a 0");
                    texteEnvoi = Console.ReadLine();
                    while (Convert.ToInt32(texteEnvoi) < 0)
                        {
                            Console.WriteLine("Erreur, veuillez choisir un nombre superieur a 0");
                        texteEnvoi = Console.ReadLine();
                        }
                    tailleFen = Convert.ToInt32(texteEnvoi);

                        Console.WriteLine("Quel nombre maximal de tentatives de renvoi souhaitez vous ? Choisissez un nombre superieur a 0");
                    texteEnvoi = Console.ReadLine();
                    while (Convert.ToInt32(texteEnvoi) <= 0)
                        {
                            Console.WriteLine("Erreur, veuillez choisir un nombre superieur a 0");
                        texteEnvoi = Console.ReadLine();
                        }
                    nbEnvois = Convert.ToInt32(texteEnvoi);


                        emetteur = new Liaison.CoucheLiaison(true, correcteur, rejet, nbEnvois, tailleFen);
                        recepteur = new Liaison.CoucheLiaison(false, correcteur, rejet, nbEnvois, tailleFen);
                    repondu = true;

                    }
                    else if (texteEnvoi.ToUpper().Equals("N"))
                    {
                        repondu = true;
                        emetteur = new Liaison.CoucheLiaison(true, true, true);
                        recepteur = new Liaison.CoucheLiaison(false);
                    }
                }

            // parametrisation couche physique
            repondu = false;
            Console.WriteLine("Souhaitez vous définir vos parametres de la couche physique ? (Y/N)");
            while (!repondu)
            {
                texteEnvoi = Console.ReadLine();
                try
                {
                    if (texteEnvoi.ToUpper().Equals("Y"))
                {
                    repondu = true;
                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour l'affaiblissement");
                    texteEnvoi = Console.ReadLine();
                        while (texteEnvoi == null || !((float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) <= 1.0 && (float) Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                        affaiblissement = (float) Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture);
                        Console.WriteLine(affaiblissement.ToString());
                    texteEnvoi = null;

                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour l'interference : ");
                    texteEnvoi = Console.ReadLine();
                        while (texteEnvoi == null || !((float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) <= 1.0 && (float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                        interference = (float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture);
                    texteEnvoi = null;

                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour le dedoublement : ");
                    texteEnvoi = Console.ReadLine();
                        while (texteEnvoi == null && !((float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) <= 1.0 && (float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                        dedoublement = (float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture);
                    texteEnvoi = null;

                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour le retard : ");
                    texteEnvoi = Console.ReadLine();
                        while (texteEnvoi == null && !((float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) <= 1.0 && (float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                        pretard = (float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture);
                    texteEnvoi = null;

                    Console.WriteLine("Veuillez un chiffre entre 0 et 1 pour définir le taux de probabilité pour la perte : ");
                    texteEnvoi = Console.ReadLine();
                        while (texteEnvoi == null && !((float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) <= 1.0 && (float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture) >= 0.0))
                    {
                        Console.WriteLine("Erreur, entrer un taux de proba correct entre 0 et 1");
                        texteEnvoi = Console.ReadLine();
                    }
                        perte = (float)Convert.ToDecimal(texteEnvoi, CultureInfo.InvariantCulture);
                    texteEnvoi = null;

                    couchePhy = new Physique.CouchePhysique(affaiblissement, interference, dedoublement, pretard, perte);

                }
                    else if (texteEnvoi.ToUpper().Equals("N"))
                {
                    repondu = true;
                    couchePhy = new Physique.CouchePhysique(0.03f,0.03f,0.01f,0.05f,0.01f);
                }

                }
                catch (OverflowException) { }
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
