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


        public HedgingTool HedgTool
        {
            get { return this.hedgTool; }
            set
            {
                SetProperty(ref this.hedgTool, value);
            }
        }

        public List<double> OptionValue { get => HedgTool.OptionValue; }

        public List<double> PortfolioValue { get => HedgTool.PortfolioValue; }

        public void ComputeTest()
        {
            HedgTool.update();
        }

    }
}
