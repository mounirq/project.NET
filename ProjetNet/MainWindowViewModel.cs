using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections.Generic;
using ProjetNet.ViewModels;
using LiveCharts;
using System.Windows;
using LiveCharts.Wpf;

namespace ProjetNet
{
    internal class MainWindowViewModel : BindableBase
    {
        #region Private Fields

        public ObservableCollection<AbstractDataProviderViewModel> ComponentDatatypeList { get; private set; }
        private Message helloMessage;
        public ObservableCollection<Message> Messages{ get; private set; }

        public Boolean plot = false;
        //public ObservableCollection<AbstractDataViewModel> ComponentInfoList { get; private set; }

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel()
        {
            //this.WindowPlotVM = new PlotViewModel();
            ComponentDatatypeList = new ObservableCollection<AbstractDataProviderViewModel>()
            {
                new SimulatedDataProviderViewModel(),
                new HistoricalDataProvioderViewModel()
            };
            //PlotCommand = new DelegateCommand(CanPlot);
        }

        private void CanPlot()
        {
            plot = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public DelegateCommand PlotCommand { get; private set; }
        public Message HelloMessage { get { return helloMessage; } }

        public PlotViewModel WindowPlotVM { get; set; }

        public UserInputViewModel UserInputVM { get; set; }

        #endregion Public Properties

        #region Public Methods

        #endregion Public Methods
    }
}
