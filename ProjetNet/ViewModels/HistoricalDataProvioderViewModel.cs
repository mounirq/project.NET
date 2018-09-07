using ProjetNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.ViewModels
{
    internal class HistoricalDataProvioderViewModel : AbstractDataProviderViewModel
    {
        #region Public Constructors

        public HistoricalDataProvioderViewModel() : base(new HistoricalDataProvider()) { }

        #endregion Public Constructors

        #region Public Properties

        public override string Name
        {
            get
            {
                return "BackTest";
            }
        }

        #endregion Public Properties

    }
}
