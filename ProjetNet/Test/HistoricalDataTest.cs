using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using ProjetNet.Models;


namespace ProjetNet.Test
{
    class HistoricalDataTest
    {
        public static void Main(string[] args)
        {
            // header
            Console.WriteLine("***********************");
            Console.WriteLine("*    Test Load Data   *");
            Console.WriteLine("***********************");
            var HistoricalData = new HistoricalDataProvider();
            DateTime from = new DateTime(2010, 01, 01);
            DateTime maturity = new DateTime(2010, 04, 12);
            Share share1 = new Share("ACCOR SA", "AC FP");
            Share share2 = new Share("CREDIT AGRICOLE SA", "ACA FP");
            Share[] underlyingShares = new Share[] { share1, share2 };
            double[] weights = new double[] { 0.3, 0.7 };
            double strike = 7000;
            IOption option = new BasketOption("panier", underlyingShares, weights, maturity, strike);
            List<DataFeed> lst = HistoricalData.GetDataFeeds(option, from);
            foreach (DataFeed element in lst)
            {
                Console.WriteLine("\n\n" + element.Date.ToString() + "\n" + string.Join(",", element.PriceList.Select(kv => kv.Key + "=" + kv.Value).ToArray()));
            }
            Console.WriteLine("**************End of Loading****************");
            String[] id = lst[0].PriceList.Keys.ToArray();
            Console.WriteLine("les ids : " + id[0] + " et " + id[1]);

            Console.ReadKey(true);
        }
    }
}
