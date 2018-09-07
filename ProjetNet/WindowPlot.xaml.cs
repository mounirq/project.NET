using LiveCharts;
using LiveCharts.Wpf;
using ProjetNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjetNet
{
    /// <summary>
    /// Logique d'interaction pour WindowPlot.xaml
    /// </summary>
    public partial class WindowPlot : Window
    {
        public WindowPlot()
        {
            InitializeComponent();
            this.DataContext = new PlotViewModel();
           
        }
    }
}
