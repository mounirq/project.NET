using System;
using System.Collections.Generic;
using System.Linq;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;

namespace ProjetNet.Models
{
    internal class SimulatedDataProvider : IDataProvider
    {
        public List<DataFeed> GetDataFeeds(IOption option, DateTime from)
        {
            SimulatedDataFeedProvider simulateur = new SimulatedDataFeedProvider();
            return simulateur.GetDataFeed(option, from);
        }


        public static void Main(string[] args)
        {
            // header
            
            var simulatedData = new SimulatedDataProvider();
            DateTime from = new DateTime(2018, 09, 04);
            DateTime maturity = new DateTime(2019, 09, 04);
            string name = "VanillaCall";
            double strike = 7000;
            Share underlying = new Share("vod.l", "vod.l");
            IOption option = new VanillaCall(name, underlying, maturity, strike);
            var lst = simulatedData.GetDataFeeds(option, from);

            Console.WriteLine("************************************");
            Console.WriteLine("*    Test Simulation CallVanille   *");
            Console.WriteLine("************************************");
            foreach (DataFeed element in lst)
            {
                Console.WriteLine("\n\n\n\n" + element.Date.ToString() + "\n" + string.Join(",", element.PriceList.Select(kv => kv.Key + "=" + kv.Value).ToArray()));
            }
            Console.WriteLine("**************End of simulation Vanille****************");

            Share share1 = new Share("vod.l", "vod.l");
            Share share2 = new Share("ftse", "ftse");
            string nameBasket = "Basket";
            double strikeBasket = 7000;
            Share[] sharesBasket = { share1, share2 };
            Double[] weights = { 0.3, 0.7 };
            DateTime maturityBasket = new DateTime(2019, 09, 04);
            IOption optionBasket = new BasketOption(nameBasket, sharesBasket, weights, maturityBasket, strikeBasket);
            var simulationBasket = simulatedData.GetDataFeeds(optionBasket, from);

            Console.WriteLine("\n\n\n\n");
            Console.WriteLine("************************************");
            Console.WriteLine("*    Test Simulation BasketOption   *");
            Console.WriteLine("************************************");
            foreach (DataFeed element in simulationBasket)
            {
                Console.WriteLine("\n\n\n\n" + element.Date.ToString() + "\n" + string.Join(",", element.PriceList.Select(kv => kv.Key + "=" + kv.Value).ToArray()));
            }
            Console.WriteLine("**************End of simulation BasketOption****************");

            Console.ReadKey(true);
        }
    }
}
