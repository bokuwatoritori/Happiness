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
        protected abstract int ReceptionPacket(Trame t, bool erreur);


        /* public bool[] pull() // Retourner lees packet de recu non réclamé (sauf si bond, genre 0,1,3 on envois 0,1 seulement)
         {
             if(parties.Keys.Any(x=> x == PartieRéclamé+1 ))
             {
                 int longueur = 1;
                 while (parties.Keys.Any(x => x == PartieRéclamé + longueur + 1))
                     longueur++;

                 var pris = parties.OrderBy(i => i.Key).Select(i=>i.Value).Take(longueur);
                 PartieRéclamé = PartieRéclamé + longueur;
                 return pris.Flatten();

             }
             else
             {
                 return new bool[] { };
             }
         }
         a trifouiller */

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
                    etat = ReceptionPacket(trame, t.erreur);
                    break;
                case Trame.TypeTrame.End:
                    etat = ReceptionPacket(trame, t.erreur);
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
