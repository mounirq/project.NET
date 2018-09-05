using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using PricingLibrary.Computations;
using PricingLibrary.Utilities;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;

namespace ProjetNet.Models
{

    internal class HistoricalData : IData
    {
        public List<DataFeed> GetDataFeeds(IOption option, DateTime from)
        {
            using (DataBaseAccessDataContext asdc = new DataBaseAccessDataContext())
            {
                var r = new List<DataFeed>();
                var underlyingIds = option.UnderlyingShareIds;
                var req = asdc.HistoricalShareValues.Where(el => underlyingIds.Contains(el.id) && el.date >= from && el.date <= option.Maturity).ToList();
                var dates = req.Select(el => el.date).Distinct().ToList();
                foreach (DateTime d in dates)
                {
                    Dictionary<string, decimal> priceList = new Dictionary<string, decimal>();
                    foreach (string id in underlyingIds)
                    {
                        decimal price = req.First(el => (DateTime.Compare(el.date,d)==0 && (el.id.Trim() == id.Trim()))).value;
                        priceList.Add(id, price);
                    }
                    r.Add(new DataFeed(d, priceList));
                }
                return r;
            }
        }

       /* public static void Main(string[] args)
        {
            // header
            Console.WriteLine("***********************");
            Console.WriteLine("*    Test Load Data   *");
            Console.WriteLine("***********************");
            var HistoricalData = new HistoricalData();
            DateTime from = new DateTime(2010, 01, 01);
            DateTime maturity = new DateTime(2010, 01, 12);
            Share share1 = new Share("ACCOR SA", "AC FP");
            Share share2 = new Share("CREDIT AGRICOLE SA", "ACA FP");
            Share[] underlyingShares = new Share[] {share1, share2};
            double[] weights = new double[] { 0.3, 0.7 };
            double strike = 7000;
            IOption option = new BasketOption("panier", underlyingShares, weights, maturity, strike);
            List<DataFeed> lst = HistoricalData.GetDataFeeds(option, from);
            foreach (DataFeed element in lst)
            {
                Console.WriteLine("\n\n" + element.Date.ToString() + "\n" + string.Join(",", element.PriceList.Select(kv => kv.Key + "=" + kv.Value).ToArray()));
            }
            Console.WriteLine("**************End of Loading****************");
            Console.ReadKey(true);
        }*/
    }
}
