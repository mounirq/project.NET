using PricingLibrary.Computations;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities;
using PricingLibrary.Utilities.MarketDataFeed;
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
        private List<double> optionValue;
        private List<double> portfolioValue;
        private List<string> dateValue;
        private double normalizedGain;

        #endregion Private Fields

        #region Public Constructors

        public void update()
        {
            this.optionValue = new List<double>();
            this.portfolioValue = new List<double>();
            this.dateValue = new List<string>();
            IOption option = null;
            try
            {
                option = constructOption();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
                return;
            }

            List<DataFeed> dataFeeds;
            userInput.StartDate = Tools.NextBusinessDay(userInput.StartDate);

            DateTime startDateOfEstimation = Tools.MinusBusinessDays(userInput.StartDate, userInput.EstimationWindow);

            if (userInput.DataType.GetType() == typeof(HistoricalDataProvider))
            {
                HistoricalDataProvider historicalData = new HistoricalDataProvider();
                DateTime minDate = historicalData.GetMinDate();
                DateTime maxDate = historicalData.GetMaxDate();
                if (startDateOfEstimation < minDate || userInput.Maturity > maxDate)
                {
                    throw new ArgumentException("Unavailable historical data for the selected dates and estimationWindow, they must be between : "+
                        minDate.ToString("dd/MM/yyyy") + " and "+ maxDate.ToString("dd/MM/yyyy"));
                }
                dataFeeds = historicalData.GetDataFeeds(option, startDateOfEstimation);
            }
            else
            {
                SimulatedDataProvider simulatedData = new SimulatedDataProvider();
                dataFeeds = simulatedData.GetDataFeeds(option, startDateOfEstimation);
            }

            /* First step */
            ParametersEstimation parameters = new ParametersEstimation(dataFeeds, userInput.StartDate, userInput.EstimationWindow);

            DateTime currentDay = userInput.StartDate;
            Portfolio portfolio = new Portfolio();
            
            Pricer pricer = new Pricer();

            Share[] underlyingsShares = ShareTools.GenerateShares(userInput.UnderlyingsIds);

            double[] returnedValue = portfolio.estimatePortfolioFirstDay(parameters, underlyingsShares, option, currentDay, dataFeeds[userInput.EstimationWindow], pricer);
            optionValue.Add(returnedValue[0]);
            portfolioValue.Add(returnedValue[1]);
            dateValue.Add(currentDay.ToString("dd/MM/yyyy"));

            List<DataFeed> dataFeedSkipped = dataFeeds.Skip(userInput.EstimationWindow).ToList();

            int i = 1;
            foreach (DataFeed data in dataFeedSkipped)
            {
                if (i % userInput.RebalancementFrequency == 0 && !data.Date.Equals(dataFeedSkipped.Last().Date))
                {
                    currentDay = data.Date;
                    parameters = new ParametersEstimation(dataFeeds, currentDay, userInput.EstimationWindow);
                    returnedValue = portfolio.updatePortfolio(userInput.RebalancementFrequency, parameters, underlyingsShares, option, currentDay, data, pricer);
                    optionValue.Add(returnedValue[0]);
                    portfolioValue.Add(returnedValue[1]);
                    dateValue.Add(currentDay.ToString("dd/MM/yyyy"));
                }
                i++;
            }

            /* Last Step */
            DataFeed maturityData = dataFeedSkipped.Last();
            normalizedGain = portfolio.calculateGain(maturityData, underlyingsShares, option, pricer);
        }

        public HedgingTool(UserInput userInput)
        {
            this.userInput = userInput;
        }
        #endregion Public Constructors

        #region Public Methods
           
        public IOption constructOption()
        {
            IOption option = null;

            if ((userInput.Weights == null) || userInput.Weights.Sum() != 1) { throw new ArgumentException("The sum of the weights must equal 1"); }
            if (DateTime.Compare(userInput.StartDate, userInput.Maturity) >= 0) { throw new ArgumentException("The start date must be shorter than maturity"); }
                List<Share> underlyingsShares = new List<Share>();
            foreach (string underlyingId in userInput.UnderlyingsIds)
            {
                String underlyingName = ShareTools.GetShareName(underlyingId);
                underlyingsShares.Add(new Share(underlyingName, underlyingId));
            }
            if (userInput.OptionType.Equals("VanillaCall"))
            {
                if (underlyingsShares.Count > 1) { throw new ArgumentException("You have to choose only one underlying share for the Vanilla call"); }
                option = new VanillaCall(userInput.NameOption, underlyingsShares[0], userInput.Maturity, userInput.Strike);
            }
            else if (userInput.OptionType.Equals("BasketOption"))
            {
                if(userInput.Weights.Length == 1) { throw new ArgumentException("You have to choose multiple underlying shares for the Basket option");}
                option = new BasketOption(userInput.NameOption, underlyingsShares.ToArray(), userInput.Weights, userInput.Maturity, userInput.Strike);
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

        public double NormalizedGain { get => normalizedGain; set => normalizedGain = value; }
        public List<double> OptionValue { get => optionValue; set => optionValue = value; }
        public List<double> PortfolioValue { get => portfolioValue; set => portfolioValue = value; }
        public List<string> DateValue { get => dateValue; set => dateValue = value; }

        #endregion Public Properties
    }
}