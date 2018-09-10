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
        private UserInputViewModel userInputVM;

        public HedgingToolViewModel(UserInputViewModel userInputVM)
        {
            UserInputVM = userInputVM;
            HedgTool = new HedgingTool(UserInputVM.UnderlyingUserInput);

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
        internal UserInputViewModel UserInputVM { get => userInputVM; set => userInputVM = value; }
        
        public void ComputeTest()
        {
            HedgTool.update();
        }

    }
}
