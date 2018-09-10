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
using PricingLibrary.FinancialProducts;

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
        private IOption selectedOption;
        private ObservableCollection<String> componentSelectedShareIds;
        private ObservableCollection<double> componentSelectedWeights;
        private Dictionary<string, double> selectedUnderlyingAndWeights;
        private JsonHandler jsonHandlerVM;

        private Visibility plotVisibility = Visibility.Hidden;

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel()
        {
            WindowPlotVM = new PlotViewModel();
            userInputVM = new UserInputViewModel();
            HedgingToolVM = new HedgingToolViewModel(UserInputVM);
            JsonHandlerVM = new JsonHandler();

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
            ComponentSavedOptions = new ObservableCollection<IOption>();
            JsonHandlerVM.LoadOptions();
            foreach (VanillaCall option in JsonHandlerVM.ListVanillaCalls)
            {
                ComponentSavedOptions.Add(option);
            }
            foreach (BasketOption option in JsonHandlerVM.ListBasketOptions)
            {
                ComponentSavedOptions.Add(option);
            }

            PlotCommand = new DelegateCommand(CanPlot);
            //try
            //{
            AddShareCommand = new DelegateCommand(AddShare);
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show(ex.Message.ToString());
            //    return;
            //}
            DeleteUnderlyingsCommand = new DelegateCommand(DeleteUnderlyings);
            AddOptionCommand = new DelegateCommand(AddOption);
            LoadOptionCommand = new DelegateCommand(LoadOption);
        }



        #endregion Public Constructors

        #region Public Properties

        public ObservableCollection<AbstractDataProviderViewModel> ComponentDatatypeList { get; private set; }
        public ObservableCollection<String> ComponentOptionTypeList { get; private set; }

        public ObservableCollection<String> ComponentExistingSharesIds { get; private set; }
        public ObservableCollection<IOption> ComponentSavedOptions { get; private set; }

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

        public DelegateCommand AddOptionCommand { get; private set; }

        public DelegateCommand LoadOptionCommand { get; private set; }

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
        internal JsonHandler JsonHandlerVM { get => jsonHandlerVM; set => jsonHandlerVM = value; }
        public IOption SelectedOption { get => selectedOption; set => selectedOption = value; }

        #endregion Public Properties

        #region Public Methods

        private void AddShare()
        {
            //if(SelectedShare == null) { throw new ArgumentException("Please Enter the underlying share"); }
            if (SelectedUnderlyingAndWeights.ContainsKey(SelectedShare))
            {
                //if (SelectedWeight <= 0)
                //{
                //    throw new ArgumentException("The weight must be positive");
                //}
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

        private void AddOption()
        {
            try
            {
                IOption optionToAdd = HedgingToolVM.HedgTool.constructOption();
                JsonHandlerVM.SaveOption(optionToAdd);
                ComponentSavedOptions.Add(optionToAdd);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
                return;
            }
        }

        private void LoadOption()
        {
            UserInputVM.NameOption = SelectedOption.Name;
        }

        private void CanPlot()
        {
            this.WindowPlotVM = new PlotViewModel();
            PlotVisibility = Visibility.Visible;
            HedgingToolVM.UserInputVM = UserInputVM;
            HedgingToolVM.ComputeTest();
            double[] optionValues = HedgingToolVM.OptionValue.ToArray();
            double[] portfolioValues = HedgingToolVM.PortfolioValue.ToArray();
            string[] dateValues = HedgingToolVM.DateValue.ToArray();

            WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(optionValues, portfolioValues);

            WindowPlotVM.Labels = dateValues;

            WindowPlotVM.YFormatter = value => value.ToString("C");

            Console.WriteLine("la frequence  est : " + UserInputVM.RebalancementFrequency);

        }

        #endregion Public Methods
    }
}
