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

        public void Affaiblissement()
        {
            Console.WriteLine("Perturbations de la couche physique : Affaiblissement applique...");
            for (int i = 0; i < this.trame.Length; i++)
            {
                if (rnd.Next(1000) > (1 - affaiblissement) * 1000)
                {
                    this.trame[i] = true;
                }
            }
        }

        public void Interference()
        {
            int rand;
            Console.WriteLine("Perturbations de la couche physique : Interference applique...");
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

        public void Dedoublement()
        {
            if (rnd.Next(1000) > (1 - dedoublement) * 1000)
            {
                Console.WriteLine("Perturbations de la couche physique : Dedoublement applique...");
                retard = new bool[trame.Length];
                for (int i = 0; i < retard.Length; i++)
                {
                    retard[i] = trame[i];
                }
            }
        }

        public bool Retard()
        {
            if (rnd.Next(1000) > (1 - pretard) * 1000 && retard == null)
            {
                Console.WriteLine("Perturbations de la couche physique : Retard applique...");
                retard = new bool[trame.Length];
                for (int i = 0; i < retard.Length; i++)
                {
                    retard[i] = trame[i];
                }
                return false;
            }
            if (rnd.Next(1000) > 700 && retard != null)
            {
                Console.WriteLine("Perturbations de la couche physique : Retard termine...");
                for (int i = 0; i < Math.Min(trame.Length, retard.Length); i++)
                {
                    trame[i] = retard[i];
                }
                retard = null;
            }
            return true;
        }

        public bool Perte()
        {
            if (rnd.Next(1000) > (1 - perte) * 1000)
            {
                Console.WriteLine("Perturbations de la couche physique : Perte applique...");
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
