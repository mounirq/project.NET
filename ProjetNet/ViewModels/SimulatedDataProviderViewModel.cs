using ProjetNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.ViewModels
{
    internal class SimulatedDataProviderViewModel : AbstractDataProviderViewModel
    {
        #region Public Constructors

        public SimulatedDataProviderViewModel() : base(new SimulatedDataProvider()) { }

        #endregion Public Constructors

        #region Public Properties

        public override string Name
        {
            get
            {
                return "Simulated";
            }
        }

        #endregion Public Properties

    }
}
