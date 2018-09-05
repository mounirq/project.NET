using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;

namespace ProjetNet.Models
{
    internal interface IDataProvider
    {
        #region Public Methods

        List<DataFeed> GetDataFeeds(IOption option, DateTime from);

        #endregion Public Methods
    }
}
