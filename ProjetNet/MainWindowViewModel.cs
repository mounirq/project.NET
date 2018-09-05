using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections.Generic;

namespace ProjetNet
{
    internal class MainWindowViewModel : BindableBase
    {
        #region Private Fields
        private Message helloMessage;
        public ObservableCollection<Message> Messages{ get; private set; }
        //public ObservableCollection<AbstractDataViewModel> ComponentInfoList { get; private set; }

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel()
        {
             helloMessage = new Message() { Number = 1};
            //Message helloMessage2 = new Message() { Number = 2 };
            //List<Message> myList = new List<Message>() { helloMessage1, helloMessage2 };
            //Messages = new ObservableCollection<Message>(myList);
        }

        #endregion Public Constructors

        #region Public Properties

        public Message HelloMessage { get { return helloMessage; } }

        #endregion Public Properties
    }
}
