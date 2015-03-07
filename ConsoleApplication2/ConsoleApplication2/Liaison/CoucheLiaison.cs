using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2.Liaison
{
    public class CoucheLiaison
    {
        public bool isEmetteur;
        bool hamming;
        bool rejet;
        char[] car;
        bool[] reception;
        bool[][] emission;
        StreamReader file;
        Trame trame;
        private volatile bool _shouldStop;
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

            }
        }

        public void Push()
        {
            char[] fichierEclate = Noyau.lireFichier();
            int nbrTrame = fichierEclate.Length;
            emission = new bool[nbrTrame][];
            for (int i = 0; i < nbrTrame; i++)
            {
                Transtypage.IntegerToBits((int)fichierEclate[i], 8).CopyTo(emission[i], 8 * i);
            }
            envoiTermine = false;
            while (!envoiTermine)
            {
                envoyer();
                if (recevoir() == nbrTrame)
                    envoiTermine = true;
            }
        }



        void envoyer()//Tableau de 8 bool de donnee a envoyer
        {
            noTrame = rj.choixDeTrame();
            Trame envoie = new Trame(emission.Length == 1 ? Trame.TypeTrame.End : Trame.TypeTrame.Data, emission[noTrame].Length, noTrame, Noyau.hammingCorrecteur, 1, 0, emission[noTrame]);
            bool[] paquet = Hamming.ajouteHamming(envoie.ToBool());
            Send(paquet);
        }

        public static void Send(bool[] p)
        {
            if (p.Length != 33)
                throw new InvalidOperationException("Je ne vais pas envoyer a la couche physique un paquet sans hamming!");

            bool[] envoieSource = new bool[p.Length];
            p.CopyTo(envoieSource, 0);
            Noyau.envoieSource = envoieSource;
            Noyau.pretEmettre = false;
            Noyau.synchcond1.Release(); //V
        }

        int recevoir()//Tableau de 8 bool de donnee a recevoir
        {
            int retour = -4;
            if (Noyau.receptionDestination != null)
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
            }
            return retour;
        }
        public void RequestStop()
        {
            _shouldStop = true;
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
