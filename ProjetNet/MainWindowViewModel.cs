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
using System.Linq;
using System.Windows;

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
        private ObservableCollection<double> componentSelectedWeights;
        private Dictionary<string, double> selectedUnderlyingAndWeights;

        private Visibility plotVisibility = Visibility.Hidden;

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

            SelectedUnderlyingAndWeights = new Dictionary<string, double>();

            ComponentExistingSharesIds = new ObservableCollection<string>(ShareTools.GetAllShareIds());
            ComponentSelectedShareIds = new ObservableCollection<string>();
            ComponentSelectedWeights = new ObservableCollection<double>();
            PlotCommand = new DelegateCommand(CanPlot);
            AddShareCommand = new DelegateCommand(AddShare);
            DeleteUnderlyingsCommand = new DelegateCommand(DeleteUnderlyings);
        }



        #endregion Public Constructors

        #region Public Properties

        public ObservableCollection<AbstractDataProviderViewModel> ComponentDatatypeList { get; private set; }
        public ObservableCollection<String> ComponentOptionTypeList { get; private set; }

        public ObservableCollection<String> ComponentExistingSharesIds { get; private set; }

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

        public Visibility PlotVisibility
        {
            get { return this.plotVisibility; }
            set { SetProperty(ref this.plotVisibility, value); }
        }

        public DelegateCommand PlotCommand { get; private set; }
        public DelegateCommand AddShareCommand { get; private set; }

        public DelegateCommand DeleteUnderlyingsCommand { get; private set; }

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
        public ObservableCollection<double> ComponentSelectedWeights { get => componentSelectedWeights; set => componentSelectedWeights = value; }
        public Dictionary<string, double> SelectedUnderlyingAndWeights { get => selectedUnderlyingAndWeights; set => selectedUnderlyingAndWeights = value; }

        #endregion Public Properties

        #region Public Methods

        private void AddShare()
        {
            if (SelectedUnderlyingAndWeights.ContainsKey(SelectedShare))
            {
                SelectedUnderlyingAndWeights[SelectedShare] = SelectedWeight;
            }
            else
            {
                SelectedUnderlyingAndWeights.Add(SelectedShare, SelectedWeight);
            }
            ComponentSelectedShareIds.Clear();
            ComponentSelectedWeights.Clear();
            foreach(String key in SelectedUnderlyingAndWeights.Keys)
            {
                ComponentSelectedShareIds.Add(key);
                ComponentSelectedWeights.Add(SelectedUnderlyingAndWeights[key]);
            }
            UserInputVM.UnderlyingsIds = SelectedUnderlyingAndWeights.Keys.ToList();
            UserInputVM.Weights = SelectedUnderlyingAndWeights.Values.ToList();
        }

        private void DeleteUnderlyings()
        {
            UserInputVM.ClearWeight();
            UserInputVM.ClearUnderlyings();
            ComponentSelectedShareIds.Clear();
            ComponentSelectedWeights.Clear();
            SelectedUnderlyingAndWeights.Clear();
        }

        private void CanPlot()
        {
            this.WindowPlotVM = new PlotViewModel();
            PlotVisibility = Visibility.Visible;
            HedgingToolVM.UserInputVM = UserInputVM;
            HedgingToolVM.ComputeTest();
            double[] optionValues = HedgingToolVM.OptionValue.ToArray();
            double[] portfolioValues = HedgingToolVM.PortfolioValue.ToArray();

            WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(optionValues, portfolioValues);

            WindowPlotVM.Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };

            WindowPlotVM.YFormatter = value => value.ToString("C");

            Console.WriteLine("la frequence  est : " + UserInputVM.RebalancementFrequency);

        }

        #endregion Public Methods
    }
}
