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
        //double[] option, double[] portefeuille
        //public SeriesCollection SeriesCollection;
        //public String[] Labels;
        //public YFormatter
        private SeriesCollection _serie;
        private String[] _labels;
        public PlotViewModel()
        {
            //SeriesCollection = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "Series 1",
            //        Values = new ChartValues<double> { 4, 6, 5, 2 ,7 }
            //    },
            //    new LineSeries
            //    {
            //        Title = "Series 2",
            //        Values = new ChartValues<double> { 6, 7, 3, 4 ,6 }
            //    }
            //};

            //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            //YFormatter = value => value.ToString("C");

            ////modifying the series collection will animate and update the chart
            //SeriesCollection.Add(new LineSeries
            //{
            //    Values = new ChartValues<double> { 5, 3, 2, 4 },
            //    LineSmoothness = 0 //straight lines, 1 really smooth lines
            //});

            ////modifying any series values will also animate and update the chart
            //SeriesCollection[2].Values.Add(5d);

            //DataContext = this;
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

