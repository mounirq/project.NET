using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Computations;
using PricingLibrary.Utilities;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;

namespace ProjetNet.Models
{
    internal class SimulatedData : IData
    {
        public List<DataFeed> GetDataFeeds(IOption option, DateTime from)
        {
            SimulatedDataFeedProvider simulateur = new SimulatedDataFeedProvider();
            return simulateur.GetDataFeed(option, from);
        }


        public static void Main(string[] args)
        {
            // header
            Console.WriteLine("************************************");
            Console.WriteLine("*    Test Simulation CallVanille   *");
            Console.WriteLine("************************************");
            var simulatedData = new SimulatedData();
            DateTime from = new DateTime(2018, 09, 04);
            DateTime maturity = new DateTime(2019, 09, 04);
            string name = "VanillaCall";
            double strike = 7000;
            Share underlying = new Share("vod.l", "vod.l");
            IOption option = new VanillaCall(name, underlying, maturity, strike);
            var lst = simulatedData.GetDataFeeds(option, from);
            foreach (DataFeed element in lst)
            {
                Console.WriteLine("\n\n\n\n" + element.Date.ToString() + "\n" + string.Join(",", element.PriceList.Select(kv => kv.Key + "=" + kv.Value).ToArray()));
            }
            Console.WriteLine("**************End of simulation****************");
            //Console.ReadKey(true);
        }
    }
}
