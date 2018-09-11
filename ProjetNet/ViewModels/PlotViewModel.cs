using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Prism.Mvvm;

namespace ProjetNet.ViewModels
{
    public partial class PlotViewModel : BindableBase
    {

        private SeriesCollection _serie;
        private String[] _labels;
        public PlotViewModel()
        {
        }

        public SeriesCollection SeriesCollection
        {
            get { return _serie; }
            set { SetProperty(ref _serie, value); }
        }

        public String[] Labels
        {
            get { return _labels; }
            set { SetProperty(ref _labels, value); }
        }

        public Func<double, string> YFormatter { get; set; }

        public static SeriesCollection ValuesToPlot(double[] optionValues, double[] portfolioValues)
        {
            SeriesCollection serieCollectionReturn = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Theorical Option Value",
                    Values = new ChartValues<double>(optionValues)
                },
                new LineSeries
                {
                    Title = "Portfolio Value",
                    Values = new ChartValues<double>(portfolioValues)
                }
            };
            return serieCollectionReturn;
        }
    }
}

