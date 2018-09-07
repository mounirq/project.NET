using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    internal class HedgingTool
    {
        #region Private Fields

        private UserInput userInput;
        private double[] optionValue;
        private double[] portfolioValue;
        private double normalizedGain;

        #endregion Private Fields

        #region Public Constructors

        public HedgingTool(UserInput userInput)
        {
            this.userInput = userInput;
            int numberOfRebalancing = (int) DayCount.CountBusinessDays(userInput.StartDate, userInput.Maturity)/ userInput.RebalancementFrequency;

            this.optionValue = new double[numberOfRebalancing];
            this.portfolioValue = new double[numberOfRebalancing];
        }
        #endregion Public Constructors

        #region Public Methods

        public IOption constructOption()
        {
            IOption option = null;

            if (!(userInput.Weights == null) && userInput.Weights.Sum() != 1){throw new ArgumentException("The sum of the weights must equal");}
            List<Share> underlyingsShares = new List<Share>();
            foreach (string underlyingId in userInput.UnderlyingsIds)
            {
                String underlyingName = ShareName.GetShareName(underlyingId);
                underlyingsShares.Add(new Share(underlyingName, underlyingId));
            }
            if (userInput.OptionType.Equals("VanillaCall"))
            {
                if(underlyingsShares.Count > 1) { throw new ArgumentException("You have to choose only one option "); }
                option = new VanillaCall(userInput.OptionType, underlyingsShares[0], userInput.Maturity, userInput.Strike);
            }
            else if (userInput.OptionType.Equals("BasketOption"))
            {
                option = new BasketOption(userInput.OptionType, underlyingsShares.ToArray(), userInput.Weights, userInput.Maturity, userInput.Strike);
            }
            else
            {
                throw new ArgumentException("Unkown OptionType " + userInput.OptionType);
            }
            return option;
        }

        #endregion Public Methods

        #region Public Properties

        internal UserInput UserInput { get => userInput; set => userInput = value; }
        public double[] OptionValue { get => optionValue; set => optionValue = value; }
        public double[] PortfolioValue { get => portfolioValue; set => portfolioValue = value; }
        public double NormalizedGain { get => normalizedGain; set => normalizedGain = value; }

        #endregion Public Properties
    }
}
