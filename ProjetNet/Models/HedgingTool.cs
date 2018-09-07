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

            IOption option = constructOption();

            List<DataFeed> dataFeeds;
            
            DateTime startDateOfEstimation = userInput.StartDate.AddDays(-userInput.EstimationWindow);

            if (userInput.DataType.GetType() == typeof(HistoricalDataProvider))
            {
                HistoricalDataProvider historicalData = new HistoricalDataProvider();
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

            Share[] underlyingsShares = new Share[option.UnderlyingShareIds.Length];
            int k = 0;
            foreach (string underlyingId in userInput.UnderlyingsIds)
            {
                String underlyingName = ShareName.GetShareName(underlyingId);
                underlyingsShares[k] = new Share(underlyingName, underlyingId);
                k++;
            }

            double[] returnedValue = portfolio.estimatePortfolioFirstDay(parameters, underlyingsShares, option, currentDay, dataFeeds[userInput.EstimationWindow], pricer);
            optionValue[0] = returnedValue[0];
            portfolioValue[0] = returnedValue[1];

            List<DataFeed> dataFeedSkipped = new List<DataFeed>(); ;
            for(int j = userInput.EstimationWindow; j<dataFeeds.Count; j++)
            {
                dataFeedSkipped.Add(dataFeeds[j]);
            }

            int index = 1;
            int i = 1;
            foreach(DataFeed data in dataFeedSkipped.Skip(1))
            {
                if(i % userInput.RebalancementFrequency == 0 && ! data.Date.Equals(dataFeeds.Last().Date) )
                {
                    currentDay = data.Date;
                    //parameters = parametersEstimation(dataFeeds, currentDay, userInput.EstimationWindow);
                    returnedValue = portfolio.updatePortfolio(userInput.RebalancementFrequency, parameters, underlyingsShares, option, currentDay, dataFeeds[userInput.EstimationWindow], pricer);
                    optionValue[index] = returnedValue[0];
                    PortfolioValue[index] = returnedValue[1];

                    index++;
                }
                i++;
            }

        }
        #endregion Public Constructors

        public static void Main(string[] args)
        {
            UserInput userInput = new UserInput("VanillaCall", new DateTime(2018, 09, 04), new DateTime(2019, 09, 04), 9, new string[] { "12341" }, new double[] { 1 }, new SimulatedDataProvider(), 30, 60);
            HedgingTool hedging = new HedgingTool(userInput);

            int size = hedging.OptionValue.Length;
            using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(@"C:\Users\ensimag\Desktop\WriteLines.txt"))
            {
                for (int index = 0; index < size; index++)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(hedging.OptionValue[index]);
                    file.WriteLine(hedging.PortfolioValue[index]);
                }
            }
        }

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
