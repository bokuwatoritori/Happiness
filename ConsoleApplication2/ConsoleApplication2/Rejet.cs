using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    public abstract class Rejet
    {
        const int bitParTrame = 8;
        protected int PartieRéclamé = -1;
        public static readonly int NAK = -1;
        public static readonly int DONNEE = -3;
        public static readonly int FINI = -2;
        public static readonly int ERROR = -10;


        protected abstract int ReceptionAckNak(Trame t);
        protected abstract int ReceptionPaquet(Trame t, bool erreur);


        /// <summary>
        /// Gère la réception de messages, autants données que ACK/NAK
        /// </summary>
        /// <param name="t"></param>
        public int Receive(Liaison.Hamming.retour t)
        {
            int etat = 0;
            Trame trame = new Trame(t.tabTrame);
            switch (trame.GetTypeTrame())
            {
                case Trame.TypeTrame.Data:
                    etat = ReceptionPaquet(trame, t.erreur);
                    break;
                case Trame.TypeTrame.End:
                    etat = ReceptionPaquet(trame, t.erreur);
                    break;
                case Trame.TypeTrame.Ack:
                case Trame.TypeTrame.Nack:
                    etat = ReceptionAckNak(trame);
                    break;
            }
            return etat;
        }



        /// <summary>
        /// Choisi la trame a envoyer (les données) selon l'algorithme choisi
        /// </summary>
        /// <returns>données a envoyer</returns>
        public abstract int choixDeTrame();
    }
}
