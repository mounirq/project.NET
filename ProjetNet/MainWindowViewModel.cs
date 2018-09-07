using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using ProjetNet.ViewModels;
using ProjetNet.Models;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;

namespace ProjetNet
{
    internal class MainWindowViewModel : BindableBase
    {
        #region Private Fields

        public ObservableCollection<AbstractDataProviderViewModel> ComponentDatatypeList { get; private set; }
        public ObservableCollection<String> ComponentExistingSharesIds { get; private set; }

        private UserInputViewModel userInputVM;
        private HedgingToolViewModel hedgingToolVM;
        private PlotViewModel windowPlotVM;

        private Boolean plotBool = false;

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel()
        {
            WindowPlotVM = new PlotViewModel();
            UserInputVM = new UserInputViewModel();
            //HedgingToolVM = new HedgingToolViewModel(UserInputVM);

            ComponentDatatypeList = new ObservableCollection<AbstractDataProviderViewModel>()
            {
                new SimulatedDataProviderViewModel(),
                new HistoricalDataProvioderViewModel()
            };
            ComponentExistingSharesIds = new ObservableCollection<string>(ShareName.GetAllShareIds());
            PlotCommand = new DelegateCommand(CanPlot);
            //PlotCommand = new DelegateCommand(CanPlot);
        }

        

        #endregion Public Constructors

        #region Public Properties


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

        #endregion Public Properties

        #region Public Methods

        private void CanPlot()
        {
            //this.WindowPlotVM = new PlotViewModel();
            WindowPlotVM.SeriesCollection = PlotViewModel.ValuesToPlot(new double[] { 4, 6, 5, 2, 7 }, new double[] { 6, 7, 3, 4, 6 });

            WindowPlotVM.Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };

            WindowPlotVM.YFormatter = value => value.ToString("C");

            Console.WriteLine("On est la");
        }

        #endregion Public Methods
    }
}
