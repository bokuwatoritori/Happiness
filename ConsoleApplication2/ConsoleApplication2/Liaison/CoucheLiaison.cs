using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication2.Liaison
{
    public class CoucheLiaison
    {
        public bool isEmetteur;
        bool hamming;
        bool rejet;
        bool[][] emission;
        int noTrame = 0;
        Rejet rj;
        int timer = 5;
        int tailleFenetre = 5;
        bool envoiTermine = false;

        public CoucheLiaison(bool i)
        {
            isEmetteur = i;
            this.hamming = true;
            this.rejet = true;
            rj = new RejetGlobal(timer);
        }
        public CoucheLiaison(bool i, int timer)
        {
            isEmetteur = i;
            this.hamming = true;
            this.rejet = true;
            rj = new RejetGlobal(timer);
        }

        public CoucheLiaison(bool isEmetteur, bool isCorrecteur, bool isRejetGlobal)
        {
            this.isEmetteur = isEmetteur;
            this.hamming = isCorrecteur;
            this.rejet = isRejetGlobal;
            if (isRejetGlobal)
            {
                rj = new RejetGlobal(timer);
            }
            else
            {
                rj = new RejetSelectif(tailleFenetre);
            }
        }

        public CoucheLiaison(bool isEmetteur, bool isCorrecteur, bool isRejetGlobal, int timer, int tailleFenetre)
        {
            this.isEmetteur = isEmetteur;
            this.hamming = isCorrecteur;
            this.rejet = isRejetGlobal;
            this.tailleFenetre = tailleFenetre;
            this.timer = timer;
            if (isRejetGlobal)
            {
                rj = new RejetGlobal(timer);
            }
            else
            {
                rj = new RejetSelectif(tailleFenetre);
            }
        }

        public void boucle()
        {
            if (isEmetteur)
            {
                Push();
            }
            else
            {
                Pull();
            }
        }

        public void Pull()
        {
            while (recevoir() != Rejet.FINI)
            {
                //Console.WriteLine("      pull");
            }

        }

        public void Push()
        {
            char[] fichierEclate=new char[Noyau.ligneFichierLecture()];
            Noyau.lireFichier(fichierEclate);
            int nbrTrame = fichierEclate.Length;
            emission = new bool[nbrTrame][];
            for (int i = 0; i < nbrTrame; i++)
            {
                emission[i] = new bool[8];
                Transtypage.IntegerToBits((int)fichierEclate[i], 8).CopyTo(emission[i],0);
            }
            envoiTermine = false;
            while (!envoiTermine)
            {
                //Console.WriteLine("push");
                envoyer();
                if (recevoir() == nbrTrame)
                    envoiTermine = true;
            }
        }



        void envoyer()//Tableau de 8 bool de donnee a envoyer
        {
            Trame envoie;
            noTrame = rj.choixDeTrame();
            if(noTrame < emission.Length)
                envoie = new Trame(Trame.TypeTrame.Data, emission[noTrame].Length, noTrame, Noyau.hammingCorrecteur, 1, 0, emission[noTrame]);
            else
                 envoie =  new Trame(Trame.TypeTrame.End,0,noTrame,Noyau.hammingCorrecteur,1,0,new bool[8]);
            bool[] paquet = Hamming.ajouteHamming(envoie.ToBool());
            Send(paquet);
        }

        public static void Send(bool[] p)
        {
            if (p.Length != 33)
                throw new InvalidOperationException("Je ne vais pas envoyer a la couche physique un paquet sans hamming!");
            Noyau.mutex1.WaitOne();
            if (Noyau.pretEmettre)
            {
                Noyau.envoieSource = new bool[p.Length];
                for (int i = 0; i < p.Length; i++)
                {

                    Noyau.envoieSource[i] = p[i];
                }
                Noyau.pretEmettre = false;
                Noyau.synchcond1.Release(); //V
            }
            Noyau.mutex1.Release();
        }

        int recevoir()//Tableau de 8 bool de donnee a recevoir
        {
            int retour = -4;
            Noyau.mutex2.WaitOne();
            if (Noyau.receptionDestination != null && Noyau.donneRecue)
            {

                bool[] trame = new bool[Noyau.receptionDestination.Length];
                for (int i = 0; i < trame.Length; i++)
                {
                    trame[i] = Noyau.receptionDestination[i];
                }

                Trame tramePropre;
                Hamming.retour resultHamming;
                resultHamming = Hamming.HammingReception(trame);
                tramePropre = new Trame(resultHamming.tabTrame);
                if (tramePropre.GetAdrDestination() == Convert.ToInt32(isEmetteur))
                {
                    retour = rj.Receive(resultHamming);
                    Noyau.donneRecue = false;
                    Noyau.synchcond2.Release();
                }
                else if (tramePropre.GetAdrDestination() > 1) //Car taille d'adresse pas adaptée
                {
                    Noyau.donneRecue = false;
                    Noyau.synchcond2.Release();
                }

            }
            else if(Noyau.donneRecue)
            {
                Noyau.donneRecue = false;
                Noyau.synchcond2.Release();
            }
            Console.WriteLine(Noyau.donneRecue);
            if (Noyau.receptionDestination != null)
                Hamm2.AfficheTab(Noyau.receptionDestination, Noyau.receptionDestination.Length);
            else
                Console.WriteLine("ZBRRRRRRRRRRRRRRRA");
                
            Noyau.mutex2.Release();
            return retour;
        }


        public static void EnvoiAck(int numero, int adrSrc, int adrDest)
        {
            bool[] trameEnvoi = new bool[33];
            Trame trame = new Trame(Trame.TypeTrame.Ack, 0, numero, Noyau.hammingCorrecteur, adrSrc, adrDest, '\0');
            trameEnvoi = Hamming.ajouteHamming(trame.ToBool());
            Send(trameEnvoi);
        }

        public static void EnvoiNack(int numero, int adrSrc, int adrDest)
        {
            bool[] trameEnvoi = new bool[33];
            Trame trame = new Trame(Trame.TypeTrame.Nack, 0, numero, Noyau.hammingCorrecteur, adrSrc, adrDest, '\0');
            trameEnvoi = Hamming.ajouteHamming(trame.ToBool());
            Send(trameEnvoi);
        }
    }
}
