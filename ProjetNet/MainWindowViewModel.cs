//using Prism.Commands;
//using Prism.Mvvm;
//using System;
//using System.Collections.ObjectModel;
//using ProjetNet.ViewModels;
//using ProjetNet.Models;
//using System.Windows.Input;
//using LiveCharts;
//using LiveCharts.Wpf;

//namespace ProjetNet
//{
//    internal class MainWindowViewModel : BindableBase
//    {
//        #region Private Fields


//        private string selectedShare;
//        private double selectedWeight;
//        public ObservableCollection<String> ComponentExistingSharesIds { get; private set; }

//        private UserInputViewModel userInputVM;
//        private HedgingToolViewModel hedgingToolVM;
//        private PlotViewModel windowPlotVM;
//        private ObservableCollection<String> componentSelectedShareIds;


//        private Boolean plotBool = false;

//        #endregion Private Fields

//        #region Public Constructors

//        public MainWindowViewModel()
//        {
//            WindowPlotVM = new PlotViewModel();
//            UserInputVM = new UserInputViewModel();
//            HedgingToolVM = new HedgingToolViewModel(UserInputVM);

//            ComponentDatatypeList = new ObservableCollection<AbstractDataProviderViewModel>()
//            {
//                new SimulatedDataProviderViewModel(),
//                new HistoricalDataProvioderViewModel()
//            };
//            ComponentOptionTypeList = new ObservableCollection<String>()
//            {
//                "VanillaCall",
//                "BasketOption"
//            };
//            //ComponentOptionTypeList.ToString
//            ComponentExistingSharesIds = new ObservableCollection<string>(ShareTools.GetAllShareIds());

//            PlotCommand = new DelegateCommand(CanPlot);
//            //PlotCommand = new DelegateCommand(CanPlot);
//        }



//        #endregion Public Constructors

//        #region Public Properties

//        public ObservableCollection<AbstractDataProviderViewModel> ComponentDatatypeList { get; private set; }
//        public ObservableCollection<String> ComponentOptionTypeList { get; private set; }


//        public PlotViewModel WindowPlotVM
//        {
//            get { return this.windowPlotVM; }
//            set { SetProperty(ref this.windowPlotVM, value); }
//        }


//        public UserInputViewModel UserInputVM
//        {
//            get { return this.userInputVM; }
//            set { SetProperty(ref this.userInputVM, value); }
//        }

//        public HedgingToolViewModel HedgingToolVM
//        {
//            get { return this.hedgingToolVM; }
//            set { SetProperty(ref this.hedgingToolVM, value); }
//        }

//        public bool PlotBool
//        {
//            get { return this.plotBool; }
//            set { this.plotBool = value; }
//        }

//        public DelegateCommand PlotCommand { get; private set; }

//        public ObservableCollection<string> ComponentSelectedShareIds { get => componentSelectedShareIds; set => componentSelectedShareIds = value; }
//        public string SelectedShare { get => selectedShare; set => selectedShare = value; }
//        public double SelectedWeight { get => selectedWeight; set => selectedWeight = value; }

//        #endregion Public Properties

//        #region Public Methods


//        private void AddShare()
//        {
//            UserInputVM.AddUnderlying(SelectedShare);
//            UserInputVM.AddWeight(SelectedWeight);
//            ComponentSelectedShareIds = new ObservableCollection<string>(UserInputVM.UnderlyingsIds);
//        }

//        private void CanPlot()
//        {
//            this.WindowPlotVM = new PlotViewModel();
//            HedgingToolVM.UserInputVM = UserInputVM;
//            HedgingToolVM.ComputeTest();
//            double[] optionValues = HedgingToolVM.OptionValue.ToArray();
//            double[] portfolioValues = HedgingToolVM.PortfolioValue.ToArray();


//            WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(optionValues, portfolioValues);

//            WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(new double[] { 4, 6, 5, 2, 7 }, new double[] { 6, 7, 3, 4, 6 });

//            WindowPlotVM.Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };

//            WindowPlotVM.YFormatter = value => value.ToString("C");

//            //Console.WriteLine("On est la");
//        }

//        #endregion Public Methods
//    }
//}


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

        private string selectedShare;
        private double selectedWeight;

        private UserInputViewModel userInputVM;
        private HedgingToolViewModel hedgingToolVM;
        private PlotViewModel windowPlotVM;
        private ObservableCollection<String> componentSelectedShareIds;

        public ObservableCollection<String> ComponentExistingSharesIds { get; private set; }



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
                "VanillaCall",
                "BasketOption"
            };
            //ComponentOptionTypeList.ToString
            ComponentExistingSharesIds = new ObservableCollection<string>(ShareTools.GetAllShareIds());
            PlotCommand = new DelegateCommand(CanPlot);
            AddShareCommand = new DelegateCommand(AddShare);
            //PlotCommand = new DelegateCommand(CanPlot);
        }



        #endregion Public Constructors

        #region Public Properties

        public ObservableCollection<AbstractDataProviderViewModel> ComponentDatatypeList { get; private set; }
        public ObservableCollection<String> ComponentOptionTypeList { get; private set; }


        public PlotViewModel WindowPlotVM
        {
            get { return this.windowPlotVM; }
            set { SetProperty(ref this.windowPlotVM, value); }
        }


        public UserInputViewModel UserInputVM
        {
            get { return this.userInputVM; }
            set { SetProperty(ref this.userInputVM, value); }
        }

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

        public ObservableCollection<string> ComponentSelectedShareIds { get => componentSelectedShareIds; set => componentSelectedShareIds = value; }

        #endregion Public Properties

        #region Public Methods

        private void AddShare()
        {
            UserInputVM.AddUnderlying(SelectedShare);
            UserInputVM.AddWeight(SelectedWeight);
            ComponentSelectedShareIds = new ObservableCollection<string>(UserInputVM.UnderlyingsIds);
        }

        private void CanPlot()
        {
            this.WindowPlotVM = new PlotViewModel();
            HedgingToolVM.UserInputVM = UserInputVM;
            HedgingToolVM.ComputeTest();
            double[] optionValues = HedgingToolVM.OptionValue.ToArray();
            double[] portfolioValues = HedgingToolVM.PortfolioValue.ToArray();

            WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(optionValues, portfolioValues);

            //WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(new double[] { 4, 6, 5, 2, 7 }, new double[] { 6, 7, 3, 4, 6 });

            WindowPlotVM.Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };

            WindowPlotVM.YFormatter = value => value.ToString("C");

            Console.WriteLine("la frequence  est : " + UserInputVM.RebalancementFrequency);

        }

        #endregion Public Methods
    }
}
