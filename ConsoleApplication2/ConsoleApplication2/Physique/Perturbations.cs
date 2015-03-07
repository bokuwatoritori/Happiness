using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2.Physique
{
    class Perturbations
    {
        private bool[] retard;
        float affaiblissement, interference, dedoublement, pretard, perte;
        public bool[] trame { get; set; }
        private Random rnd = new Random();
        //Chaque perturbation a une certaine probabilité, definissable dans le constructeur
        public Perturbations(float affaiblissement, float interference, float dedoublement, float pretard, float perte)
        {
            this.affaiblissement = affaiblissement;
            this.interference = interference;
            this.dedoublement = dedoublement;
            this.pretard = pretard;
            this.perte = perte;
            trame = new bool[1];
        }
        public Perturbations()
        {
            this.affaiblissement = 0.01f;
            this.interference = 0.01f;
            this.dedoublement = 0.02f;
            this.pretard = 0.02f;
            this.perte = 0.005f;

        }

        //L'affaiblissement, lorsqu'il est percevable après l'interprétation de la couche physique revient à ramener quelques bits à 0
        public void Affaiblissement()
        {
            for (int i = 0; i < this.trame.Length; i++)
            {
                if (rnd.Next(1000) > (1 - affaiblissement) * 1000)
                {
                    this.trame[i] = true;
                }
            }
        }

        //Les interférences, après l'interprétation de la couche physique revient a transformer certains bits en 1 et d'autres en 0
        public void Interference()
        {
            int rand;
            for (int i = 0; i < trame.Length; i++)
            {
                rand = rnd.Next(1000);
                if (rand > (1 - interference) * 1000)
                {
                    trame[i] = true;
                }
                if (rand < interference * 1000)
                {
                    trame[i] = false;
                }
            }
        }

        //Le dedoublement garde en mémoire la trame dedoublee et laisse la trame intacte
        public void Dedoublement()
        {
            if (rnd.Next(1000) > (1 - dedoublement) * 1000)
            {
                retard = new bool[trame.Length];
                for (int i = 0; i < retard.Length; i++)
                {
                    retard[i] = trame[i];
                }
            }
        }

        //Le retard garde en mémoire la trame et la supprime de la trame actuelle
        public bool Retard()
        {
            if (rnd.Next(1000) > (1 - pretard) * 1000 && retard == null)
            {
                retard = new bool[trame.Length];
                for (int i = 0; i < retard.Length; i++)
                {
                    retard[i] = trame[i];
                }
                return false;
            }
            //Ici est géré le fait qu'une trame retardée ou dédoublée puisse réapparaitre
            if (rnd.Next(1000) > 700 && retard != null)
            {
                for (int i = 0; i < Math.Min(trame.Length, retard.Length); i++)
                {
                    trame[i] = retard[i];
                }
                retard = null;
            }
            return true;
        }

        //La perte revient à supprimer une trame (effectuée dans notre cas dans la couche physique).
        public bool Perte()
        {
            if (rnd.Next(1000) > (1 - perte) * 1000)
            {
                return false;
            }
            return true;
        }

        public void Clean()
        {
            trame = null;
        }
    }
}
