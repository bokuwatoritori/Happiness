using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class RejetGlobal : Rejet /* des choses à modifier à cause du merge raté */
    {
        private int numeroTrameAttendue;
        int partieEnvoye;
        int dernierePartieRecu;
        int time;//nombre de trames


        public RejetGlobal(int nombre)
        {
            partieEnvoye = -1;
            dernierePartieRecu = 0;
            numeroTrameAttendue = 0;
            time = nombre;
        }

        protected override int ReceptionAckNak(Trame t)
        {
            if (t.GetTypeTrame() == Trame.TypeTrame.Ack)
            {
                dernierePartieRecu = t.GetNumero();
                return dernierePartieRecu;
            }
            return NAK;
        }

        protected override int ReceptionPaquet(Trame trame, bool erreur)
        {
            if (!erreur)
            {
                if (trame.GetNumero() == numeroTrameAttendue) //on recoit la bonne trame
                {
                    Console.Out.WriteLine("Machine : Reception données - Trame : " + trame.GetNumero());
                    Console.Out.WriteLine("Adresse source :" + trame.GetAdrSource() + "- Taille données :" + trame.GetTailleDonnees());
                    Liaison.CoucheLiaison.EnvoiAck(trame.GetNumero(), trame.GetAdrDestination(), trame.GetAdrSource());
                    numeroTrameAttendue++;
                    Console.Out.WriteLine("Machine " + trame.GetAdrSource() + " : Envoi accusé de réception paquet" + trame.GetNumero());
                    if (trame.GetTypeTrame() != Trame.TypeTrame.End)
                    {
                        Noyau.ecrireFichier(Transtypage.IntegerToBits(trame.GetDonnees(), 8)); //on envoit la trame a la couche reseau
                    }
                }
                else if (trame.GetNumero() == numeroTrameAttendue - 1)//le ack precedemment envoye n'a pas ete recu
                {
                    Liaison.CoucheLiaison.EnvoiAck(trame.GetNumero(), trame.GetAdrDestination(), trame.GetAdrSource());
                    Console.Out.WriteLine("Machine " + trame.GetAdrSource() + " : Envoi accusé de réception paquet" + trame.GetNumero());
                }
                return trame.GetNumero();
            }
            return ERROR;
        }

        public override int choixDeTrame()
        {
            if (dernierePartieRecu < (partieEnvoye - time))
            {
                partieEnvoye = dernierePartieRecu + 1;
            }
            else
            {
                partieEnvoye++;
            }
            return partieEnvoye;
        }
    }
}
