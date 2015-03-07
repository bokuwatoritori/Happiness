using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    public class RejetSelectif : Rejet
    {
        protected Dictionary<int, bool[]> reception; //les packets a envoyer/recus
        bool nackMode = false; //en mode rejet
        int partiesRecues = -1; // le compteur des packets recus (si on est en NACK 2, ce compteur sera a 1, meme si on a recu 3,4 et 5. 
        int? overridePacketToSend;
        int tailleFenetre;
        int partieEnvoye = -1;

        public RejetSelectif(int taille)
        {
            reception = new Dictionary<int, bool[]>();
            tailleFenetre = taille;
        }
        protected override int ReceptionPaquet(Trame trame, bool erreur)
        {
            int partie = trame.GetNumero();
            if (!erreur)
            {
                if (partie == partiesRecues + 1) //reception de ce quon voulait
                {
                    Inserer(partie, trame.ToBool());
                    nackMode = false;
                    partiesRecues = partie;
                    while (reception.Keys.Any(x => x == partiesRecues + 1))
                        partiesRecues++; //Si on a recu le paquet 3 et 4 en attendant le packet fautif 2, le partierecu va continuer a 4.

                    Liaison.CoucheLiaison.EnvoiAck(partiesRecues, trame.GetAdrDestination(), trame.GetAdrSource());
                    TransmissionFichier();
                    return partiesRecues;
                }
                else //reception dun packet trop loin
                {
                    if (partiesRecues - PartieRéclamé < tailleFenetre)
                        Inserer(partie, trame.ToBool());

                    if (nackMode)
                    {
                        Liaison.CoucheLiaison.EnvoiAck(partiesRecues, trame.GetAdrDestination(), trame.GetAdrSource());
                        return NAK;
                    }
                    else
                    {
                        Liaison.CoucheLiaison.EnvoiNack(partiesRecues + 1, trame.GetAdrDestination(), trame.GetAdrSource());
                        nackMode = true;
                        return NAK;
                    }
                }
            }
            return NAK;
        }

        protected void Inserer(int partie, bool[] packet)
        {
            if (!reception.Keys.Any(x => x == partie)) // Un packet jamais recu avant!
            {
                reception.Add(partie, packet);
            }
        }


        protected override int ReceptionAckNak(Trame trame)
        {
            bool isAck = trame.GetTypeTrame() == Trame.TypeTrame.Ack;
            int numTrame = trame.GetNumero();
            if (isAck) // ACK
            {
                partiesRecues = numTrame;
                return partiesRecues;
            }
            else // NAK
            {
                if (!overridePacketToSend.HasValue)
                    overridePacketToSend = numTrame;
                return NAK;
            }
        }


        //Noyau.mutex1.WaitOne(); //P
        //Noyau.mutex1.Release()//V

        public override int choixDeTrame()
        {
            if (overridePacketToSend.HasValue)
            {
                partieEnvoye = overridePacketToSend.Value;
                overridePacketToSend = null;
            }
            else
            {
                partieEnvoye++;
            }

            return partieEnvoye;
        }

        private void TransmissionFichier()
        {
            Trame trame;
            for (int i = reception.Keys.Min(); i < partiesRecues; i++)
            {
                trame = new Trame(reception[i]);
                if (trame.GetTypeTrame() != Trame.TypeTrame.End)
                Noyau.ecrireFichier(Transtypage.IntegerToBits(trame.GetDonnees(), 8)); //on envoit la trame a la couche reseau
                reception.Remove(i);
            }
        }
    }
}
