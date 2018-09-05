using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    internal interface IData
    {
        #region Public Methods

        List<DataFeed> GetDataFeeds(IOption option, DateTime from);

        #endregion Public Methods
    }
}
