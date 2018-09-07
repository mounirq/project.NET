using System;
using System.Collections.Generic;
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
    }
}
