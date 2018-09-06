using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections.Generic;
using ProjetNet.ViewModels;
using LiveCharts;

namespace ProjetNet
{
    internal class MainWindowViewModel : BindableBase
    {
        #region Private Fields
        private Message helloMessage;
        public ObservableCollection<Message> Messages{ get; private set; }
        private PlotViewModel windowPlotVM;
        public Boolean plot = false;
        //public ObservableCollection<AbstractDataViewModel> ComponentInfoList { get; private set; }

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel()
        {
             helloMessage = new Message() { Number = 1};
            //Message helloMessage2 = new Message() { Number = 2 };
            //List<Message> myList = new List<Message>() { helloMessage1, helloMessage2 };
            //Messages = new ObservableCollection<Message>(myList);
            this.windowPlotVM = new PlotViewModel();
            PlotCommand = new DelegateCommand(CanPlot);
        }

        private void CanPlot()
        {
            plot = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public DelegateCommand PlotCommand { get; private set; }
        public Message HelloMessage { get { return helloMessage; } }

        #endregion Public Properties

        //#region Public Methods
        //public SeriesCollection SeriesCollection()
        //{
        //    return this.windowPlotVM.SeriesCollection;
        //}
        //public string[] Labels()
        //{
        //    return this.windowPlotVM.Labels;
        //}
        //public Func<double, string> YFormatter()
        //{
        //    return this.windowPlotVM.YFormatter;
        //}
        //#endregion Public Methods
    }
}
