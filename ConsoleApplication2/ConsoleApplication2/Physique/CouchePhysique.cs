﻿using System;
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
        private volatile bool _shouldStop = false;
        //La création d'une couche physique ne sert au départ qu'à créer l'objet de perturbation lié
        //Cet objet de la classe Perturbation contiendra les paramètres pour chaque perturbation jusqu'à la fin de l'exécution
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
            //Tant que personne n'appelle la méthode RequestToStop, le programme tourne en boucle
            while (!_shouldStop)
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
                    //Lorsque les conditions sont favorables, on copie le continue d'envoie source dans une trame qui va etre perturbée
                    trame = new bool[Noyau.envoieSource.Length];

                    for (int i = 0; i < trame.Length; i++)
                    {
                        trame[i] = Noyau.envoieSource[i];
                    }
                    //On lui applique les perturbations
                    perturb.trame = trame;
                    perturb.Affaiblissement();
                    perturb.Interference();
                    perturb.Dedoublement();
                    //Retard et perte envoie false lorsque la trame est perdue (ou renvoyée plus tard)
                    if (perturb.Retard() && perturb.Perte())
                    {
                        //Si la trame n'est pas perdue, on la copie dans reception destination
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
                    //On reinitialise pretEmettre et donneRecue, on nettoie la perturbation (le pointeur de la trame a modifier est nettoyé).
                    Noyau.pretEmettre = true;
                    Noyau.donneRecue = true;
                    perturb.Clean();
                }
                else
                {
                    //Dans le cas où la conditions etait fausse, on relache nous meme la synchro conditionnelle
                    Noyau.synchcond1.Release();
                    Noyau.synchcond2.Release();
                }
                //On sort des exclusions mutuelles
                Noyau.mutex2.Release();
                Noyau.mutex1.Release();

            }

        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
    }
}
