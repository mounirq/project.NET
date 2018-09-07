using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;

namespace ProjetNet.Models
{

    internal class HistoricalDataProvider : IDataProvider
    {
        public List<DataFeed> GetDataFeeds(IOption option, DateTime from)
        {
            using (DataBaseAccessDataContext asdc = new DataBaseAccessDataContext())
            {
                var res = new List<DataFeed>();
                var underlyingIds = option.UnderlyingShareIds;
                var req = asdc.HistoricalShareValues.Where(el => underlyingIds.Contains(el.id) && el.date >= from && el.date <= option.Maturity).ToList();
                var dates = req.Select(el => el.date).Distinct().ToList();
                foreach (DateTime d in dates)
                {
                    Dictionary<string, decimal> priceList = new Dictionary<string, decimal>();
                    foreach (string id in underlyingIds)
                    {
                        decimal price = req.First(el => (DateTime.Compare(el.date, d) == 0 && (el.id.Trim() == id.Trim()))).value;
                        priceList.Add(id, price);
                    }
                    res.Add(new DataFeed(d, priceList));
                }
                return res;
            }
        }

    }
}
