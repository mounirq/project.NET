using LiveCharts;
using Prism.Mvvm;
using ProjetNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.ViewModels
{
    internal class HedgingToolViewModel : BindableBase
    {
        private HedgingTool hedgTool;

        public HedgingToolViewModel(UserInputViewModel userInputVM)
        {
            this.hedgTool = new HedgingTool(userInputVM.UnderlyingUserInput);
        }

    }
}
