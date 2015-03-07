using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2.Physique
{
    class CouchePhysique
    {
        private Perturbations perturb;
        private bool[] trame;
        private volatile bool _shouldStop;
        public CouchePhysique()
        {
            perturb = new Perturbations();
        }

        public CouchePhysique(float affaiblissement, float interference, float dedoublement, float pretard, float perte)
        {
            perturb = new Perturbations(affaiblissement, interference, dedoublement, pretard, perte);
        }


        public void boucle()
        {
            while (_shouldStop)
            {
                Noyau.synchcond1.WaitOne();
                Noyau.synchcond2.WaitOne();
                //On attend que pretEmettre et donneeRecue soient faux
                Noyau.mutex1.WaitOne();
                Noyau.mutex2.WaitOne();
                //On s'assure que personne ne touche aux variables en meme temps
                //On verifie tout de meme pour respecter l'enonce 2.3
                if (!Noyau.pretEmettre && !Noyau.donneRecue)
                {
                    Console.WriteLine("Couche Physique : Debut du transfert sur la couche physique...");
                    trame = new bool[Noyau.envoieSource.Length];
                    for (int i = 0; i < trame.Length; i++)
                    {
                        trame[i] = Noyau.envoieSource[i];
                    }
                    Console.WriteLine("Couche Physique : Application des perturbations ...");
                    perturb.trame = trame;
                    perturb.Affaiblissement();
                    perturb.Interference();
                    perturb.Dedoublement();
                    if (perturb.Retard() && perturb.Perte())
                    {
                        Noyau.receptionDestination = new bool[trame.Length];
                        for (int i = 0; i < Noyau.receptionDestination.Length; i++)
                        {
                            Noyau.receptionDestination[i] = trame[i];
                        }
                    }
                    else
                    {
                        Noyau.receptionDestination = null;
                    }
                    Noyau.pretEmettre = true;
                    Noyau.donneRecue = true;
                    perturb.Clean();
                    Console.WriteLine("Couche Physique : Trame arrivee a destination ...");
                }
                Noyau.mutex1.Release();
                Noyau.mutex2.Release();

            }

        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
    }
}
