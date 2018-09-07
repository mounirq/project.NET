using PricingLibrary.FinancialProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    internal class UserInput
    {
        #region Private Fields

        private String optionType;
        private DateTime startDate;
        private DateTime maturity;
        private double strike;
        private String[] undelyingsIds;
        private Double[] weights;
        private IDataProvider dataType;
        private int estimationWindow;
        private int rebalancementFrequency;

        #endregion Private Fields

        #region Public Constructors

        public UserInput(){ }

        public UserInput(string optionType, DateTime startDate, DateTime maturity, double strike, string[] undelyingsIds, double[] weights, IDataProvider dataType, int estimationWindow, int rebalancementFrequency)
        {
            this.optionType = optionType;
            this.maturity = maturity;
            this.strike = strike;
            this.undelyingsIds = undelyingsIds;
            this.weights = weights;
            this.startDate = startDate;
            this.dataType = dataType;
            this.estimationWindow = estimationWindow;
            this.rebalancementFrequency = rebalancementFrequency;
        }

        #endregion Public Constructors

        #region Public Properties

        public String OptionType
        {
            get { return this.optionType; }
            set { this.optionType = value; }
        }

        public DateTime Maturity
        {
            get { return this.maturity; }
            set { this.maturity = value; }
        }

        public Double Strike
        {
            get { return this.strike; }
            set { this.strike = value; }
        }

        public String[] UnderlyingsIds
        {
            get { return this.undelyingsIds; }
            set { this.undelyingsIds = value; }
        }

        public Double[] Weights
        {
            get { return this.weights; }
            set { this.weights = value; }
        }

        public DateTime StartDate
        {
            get { return this.startDate; }
            set { this.startDate = value; }
        }

        public IDataProvider DataType
        {
            get { return this.dataType; }
            set { this.dataType = value; }
        }

        public int EstimationWindow
        {
            get { return this.estimationWindow; }
            set {
                this.estimationWindow = value;
            }
        }

        public int RebalancementFrequency { get => rebalancementFrequency; set => rebalancementFrequency = value; }

        #endregion Public Properties
    }
}
