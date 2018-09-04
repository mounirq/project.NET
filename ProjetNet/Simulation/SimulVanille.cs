using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Computations;
using PricingLibrary.Utilities;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;

namespace ProjetNet.Simulation
{
    class SimulVanille
    {
        public List<DataFeed> Simulate()
        {
            DateTime Maturity = new DateTime(2019, 09, 04);
            DateTime from = new DateTime(2018, 09, 04);
            string Name = "Vanille";
            double Strike = 1000;
            Share UnderlyingShare = new Share("Vanille", "Vanille");
            IOption option = new VanillaCall(Name, UnderlyingShare, Maturity, Strike);
            SimulatedDataFeedProvider simulateur = new SimulatedDataFeedProvider();
            return simulateur.GetDataFeed(option, from);
        }


        public static void Main(string[] args)
        {
            // header
            Console.WriteLine("************************************");
            Console.WriteLine("*    Test Simulation CallVanille   *");
            Console.WriteLine("************************************");
            var cls = new SimulVanille();
            var lst = cls.Simulate();
            foreach(DataFeed element in lst)
            {
                Console.WriteLine("\n\n\n\n" + element.Date.ToString() + "\n" + string.Join(",", element.PriceList.Select(kv => kv.Key + "=" + kv.Value).ToArray()));
            }
            Console.WriteLine("\nType enter to exit");
            Console.ReadKey(true);
        }
    }
}
