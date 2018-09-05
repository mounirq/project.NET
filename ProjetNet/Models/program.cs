using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    class program
    {


        static void Main(string[] args)
        {
            Console.WriteLine("Démarrer");
            Console.ReadLine();


            LinqSQL();

            LinqLambda();

        }


        static void LinqSQL()
        {
            Console.WriteLine("Récupération à l'aide de LINQ; syntaxe à la SQL");
            using (DataBaseAccessDataContext asdc = new DataBaseAccessDataContext())
            {
                var q2 = (from lignes in asdc.HistoricalShareValues
                          select lignes.id).Distinct();
                foreach (string nom in q2)
                {
                    Console.WriteLine("Nom: {0}", nom);
                }
            }
            Console.ReadLine();
        }


        static void LinqLambda()
        {
            Console.WriteLine("Récupération à l'aide de LINQ; syntaxe 'lambda-calcul'");
            using (DataBaseAccessDataContext asdc = new DataBaseAccessDataContext())
            {
                var q3 = asdc.HistoricalShareValues.Select(ligne => ligne.id).Distinct();

                foreach (string nom in q3)
                {
                    Console.WriteLine("Nom: {0}", nom);
                }
                Console.ReadLine();
            }
        }

    }
}
