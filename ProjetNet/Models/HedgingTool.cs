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
        private double normalizedGain;

        #endregion Private Fields

        #region Public Constructors

        public void update()
        {
            this.optionValue = new List<double>();
            this.portfolioValue = new List<double>();

            IOption option = constructOption();

            List<DataFeed> dataFeeds;
            userInput.StartDate = Tools.NextBusinessDay(userInput.StartDate);

            DateTime startDateOfEstimation = Tools.MinusBusinessDays(userInput.StartDate, userInput.EstimationWindow);

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

            Share[] underlyingsShares = ShareTools.GenerateShares(userInput.UnderlyingsIds);

            double[] returnedValue = portfolio.estimatePortfolioFirstDay(parameters, underlyingsShares, option, currentDay, dataFeeds[userInput.EstimationWindow], pricer);
            optionValue.Add(returnedValue[0]);
            portfolioValue.Add(returnedValue[1]);

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
                }
                i++;
            }
        }

        public HedgingTool(UserInput userInput)
        {
            this.userInput = userInput;
        }
        #endregion Public Constructors

        //public static void Main(string[] args)
        //{
        //UserInput userInput = new UserInput("VanillaCall", new DateTime(2018, 09, 24), new DateTime(2019, 09, 28), 9, new string[] { "AC FP" }, new double[] { 1 }, new SimulatedDataProvider(), 10, 20);

        //UserInput userInput = new UserInput("BasketOption", new DateTime(2018, 09, 24), new DateTime(2019, 09, 28), 9, new string[] { "AC FP", "ACA FP" }, new double[] { 0.7, 0.3 }, new SimulatedDataProvider(), 10, 20);

        //    HedgingTool hedging = new HedgingTool(userInput);
        //    hedging.update();

        //    int size = hedging.OptionValue.Count;
        //    using (System.IO.StreamWriter file =
        //       new System.IO.StreamWriter(@"C:\Users\ensimag\Desktop\PortefeuilleVanille.txt"))
        //    {
        //        for (int index = 0; index < size; index++)
        //        {
        //            // If the line doesn't contain the word 'Second', write the line to the file.
        //            file.WriteLine(hedging.OptionValue[index]);
        //            file.WriteLine(hedging.PortfolioValue[index]);
        //        }
        //    }
        //    Console.WriteLine("C'est terminé");
        //}

        #region Public Methods

        public IOption constructOption()
        {
            IOption option = null;

            if (!(userInput.Weights == null) && userInput.Weights.Sum() != 1) { throw new ArgumentException("The sum of the weights must equal"); }
            List<Share> underlyingsShares = new List<Share>();
            foreach (string underlyingId in userInput.UnderlyingsIds)
            {
                String underlyingName = ShareTools.GetShareName(underlyingId);
                underlyingsShares.Add(new Share(underlyingName, underlyingId));
            }
            if (userInput.OptionType.Equals("VanillaCall"))
            {
                if (underlyingsShares.Count > 1) { throw new ArgumentException("You have to choose only one option "); }
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

        public double NormalizedGain { get => normalizedGain; set => normalizedGain = value; }
        public List<double> OptionValue { get => optionValue; set => optionValue = value; }
        public List<double> PortfolioValue { get => portfolioValue; set => portfolioValue = value; }

        #endregion Public Properties
    }
}