using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections.Generic;
using ProjetNet.ViewModels;

namespace ProjetNet
{
    internal class MainWindowViewModel : BindableBase
    {
        #region Private Fields

        private UserInputViewModel userInputVM;
        public ObservableCollection<AbstractDataProviderViewModel> ComponentDatatypeList { get; private set; }

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel()
        {
            ComponentDatatypeList = new ObservableCollection<AbstractDataProviderViewModel>()
            {
                new SimulatedDataProviderViewModel(),
                new Histo()
            };
        }

        #endregion Public Constructors

        #region Public Properties


        #endregion Public Properties
    }
}
