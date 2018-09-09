using PricingLibrary.FinancialProducts;
using Prism.Mvvm;
using ProjetNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.ViewModels
{
    internal class UserInputViewModel : BindableBase
    {
        #region Private Fields

        private UserInput underlyingUserInput;

        private String optionType;
        private DateTime startDate;
        private DateTime maturity;
        private double strike;
        private String[] underlyingsIds;
        private Double[] weights;
        private AbstractDataProviderViewModel dataType;
        private int estimationWindow;
        private int rebalancementFrequency;

        #endregion Private Fields

        #region Public Constructors

        public UserInputViewModel()
        {
            //this.underlyingUserInput = new UserInput(OptionType, Maturity, Strike, UnderlyingsIds, Weights, StartDate, dataType.Data, EstimationWindow);
            this.underlyingUserInput = new UserInput();
        }

        #endregion Public Constructors

        #region Public Properties

        public UserInput UnderlyingUserInput
        {
            get { return this.underlyingUserInput; }
            set { SetProperty(ref this.underlyingUserInput, value); }

        }

        public String OptionType
        {
            get { return this.optionType; }
            set
            {
                SetProperty(ref this.optionType, value);
                //RaisePropertyChanged("OptionTypeAsV");
                //RaisePropertyChanged("OptionTypeAsB");
                UnderlyingUserInput.OptionType = value;
            }
        }

        public DateTime Maturity
        {
            get { return this.maturity; }
            set
            {
                SetProperty(ref this.maturity, value);
                UnderlyingUserInput.Maturity = value;
            }
        }

        public Double Strike
        {
            get { return this.strike; }
            set
            {
                SetProperty(ref this.strike, value);
                UnderlyingUserInput.Strike = value;
            }
        }

        public String[] UnderlyingsIds
        {
            get { return this.underlyingsIds; }
            set
            {
                SetProperty(ref this.underlyingsIds, value);
                UnderlyingUserInput.UnderlyingsIds = value;
            }
        }

        public Double[] Weights
        {
            get { return this.weights; }
            set
            {
                SetProperty(ref this.weights, value);
                UnderlyingUserInput.Weights = value;
            }
        }

        public DateTime StartDate
        {
            get { return this.startDate; }
            set
            {
                SetProperty(ref this.startDate, value);
                UnderlyingUserInput.StartDate = value;
            }
        }

        public AbstractDataProviderViewModel DataType
        {
            get { return this.dataType; }
            set
            {
                SetProperty(ref this.dataType, value);
                UnderlyingUserInput.DataType = value.Data;
            }
        }

        public int EstimationWindow
        {
            get { return this.underlyingUserInput.EstimationWindow; }
            set
            {
                SetProperty(ref this.estimationWindow, value);
                UnderlyingUserInput.EstimationWindow = value;
            }
        }

        public int RebalancementFrequency
        {
            get { return this.underlyingUserInput.RebalancementFrequency; }
            set
            {
                SetProperty(ref this.rebalancementFrequency, value);
                UnderlyingUserInput.RebalancementFrequency = value;
            }
        }

        #endregion Public Properties
                
    }
}
