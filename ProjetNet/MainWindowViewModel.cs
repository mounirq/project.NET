using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using ProjetNet.ViewModels;
using ProjetNet.Models;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;

namespace ProjetNet
{
    internal class MainWindowViewModel : BindableBase
    {
        #region Private Fields

        public ObservableCollection<AbstractDataProviderViewModel> ComponentDatatypeList { get; private set; }
        public ObservableCollection<String> ComponentOptionTypeList { get; private set; }
        public ObservableCollection<String> ComponentExistingSharesIds { get; private set; }
        

        private string selectedShare;
        private double selectedWeight;

        private UserInputViewModel userInputVM;
        private HedgingToolViewModel hedgingToolVM;
        private PlotViewModel windowPlotVM;

        private Boolean plotBool = false;

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel()
        {
            WindowPlotVM = new PlotViewModel();
            userInputVM = new UserInputViewModel();
            HedgingToolVM = new HedgingToolViewModel(UserInputVM);

            ComponentDatatypeList = new ObservableCollection<AbstractDataProviderViewModel>()
            {
                new SimulatedDataProviderViewModel(),
                new HistoricalDataProvioderViewModel()
            };
            ComponentOptionTypeList = new ObservableCollection<String>()
            {
                "VanillaOption",
                "BasketOption"
            };
            //ComponentOptionTypeList.ToString
            ComponentExistingSharesIds = new ObservableCollection<string>(ShareName.GetAllShareIds());
            PlotCommand = new DelegateCommand(CanPlot);
            AddShareCommand = new DelegateCommand(AddShare);
            //PlotCommand = new DelegateCommand(CanPlot);
        }

        

        #endregion Public Constructors

        #region Public Properties


        public PlotViewModel WindowPlotVM
        {
            get { return this.windowPlotVM; }
            set { SetProperty(ref this.windowPlotVM, value); }
        }

        public ObservableCollection<String> ComponentSelectedShareIds
        {
            get { return new ObservableCollection<String>(UserInputVM.UnderlyingsIds); }
        }

        public UserInputViewModel UserInputVM
        {
            get { return this.userInputVM; }
            set { SetProperty(ref this.userInputVM, value); }
        }
        //public UserInputViewModel UserInputVM
        //{
        //    get { return this.HedgingToolVM.UserInputVM; }
        //}

        public HedgingToolViewModel HedgingToolVM
        {
            get { return this.hedgingToolVM; }
            set { SetProperty(ref this.hedgingToolVM, value); }
        }

        public bool PlotBool
        {
            get { return this.plotBool; }
            set { this.plotBool = value; }
        }

        public DelegateCommand PlotCommand { get; private set; }
        public DelegateCommand AddShareCommand { get; private set; }
        
        public string SelectedShare
        {
            get { return this.selectedShare; }
            set { SetProperty(ref this.selectedShare, value); }
        }
        public double SelectedWeight
        {
            get { return this.selectedWeight; }
            set { SetProperty(ref this.selectedWeight, value); }
        }

        #endregion Public Properties

        #region Public Methods

        private void AddShare()
        {
            UserInputVM.UnderlyingsIds.Add(SelectedShare);
            UserInputVM.Weights.Add(SelectedWeight);
        }

        private void CanPlot()
        {
            //this.WindowPlotVM = new PlotViewModel();
            //HedgingToolVM.UserInputVM.Weights = new List<double>() { 1 };
            //HedgingToolVM.ComputeTest();
            //double[] optionValues = HedgingToolVM.OptionValue.ToArray();
            //double[] portfolioValues = HedgingToolVM.PortfolioValue.ToArray();

            //WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(optionValues, portfolioValues);

            //WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(new double[] { 4, 6, 5, 2, 7 }, new double[] { 6, 7, 3, 4, 6 });

            WindowPlotVM.Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };

            WindowPlotVM.YFormatter = value => value.ToString("C");

            Console.WriteLine("la frequence  est : " + UserInputVM.RebalancementFrequency);
            
        }

        #endregion Public Methods
    }
}
