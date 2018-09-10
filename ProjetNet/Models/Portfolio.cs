using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using PricingLibrary.Utilities;


namespace ProjetNet.Models
{
    internal class Portfolio
    {
        #region Private Fields

        private double firstPortfolioValue;
        private Dictionary<String, double> portfolioComposition;
        private double currentPortfolioValue;
        private DateTime currentDate;
        private double cashRiskFree;

        #endregion Private Fields

        #region Public Properties
        public double FirstPortfolioValue { get => firstPortfolioValue; set => firstPortfolioValue = value; }
        public Dictionary<string, double> PortfolioComposition { get => portfolioComposition; set => portfolioComposition = value; }
        public double CurrentPortfolioValue { get => currentPortfolioValue; set => currentPortfolioValue = value; }
        public DateTime CurrentDate { get => currentDate; set => currentDate = value; }

        #endregion Public Properties

        #region Public Constructors

        public Portfolio()
        {
            this.firstPortfolioValue = 0;
            this.portfolioComposition = new Dictionary<string, double>();
            this.currentPortfolioValue = 0;
        }

        public Portfolio(double firstPortfolioValue, Dictionary<String, double> portfolioComposition, DateTime currentDate)
        {
            this.firstPortfolioValue = firstPortfolioValue;
            this.portfolioComposition = portfolioComposition;
            currentPortfolioValue = firstPortfolioValue;
            this.currentDate = currentDate;
        }

        #endregion Public Constructors

        #region Public Methods
        internal double[] updatePortfolio(int numberOfDaysBetweenConvering, ParametersEstimation parameters, Share[] underlyingShares, IOption option, DateTime currentDay, DataFeed dataFeed, Pricer pricer)
        {
            SimulatedDataFeedProvider simulator = new SimulatedDataFeedProvider();
            int totalDays = simulator.NumberOfDaysPerYear;
            double optionValue;
            double portfolioValue;
            if (option.GetType() == typeof(VanillaCall))
            {
                double spot = (double)dataFeed.PriceList[underlyingShares[0].Id];
                PricingResults pricingResults = pricer.PriceCall((VanillaCall)option, currentDay, totalDays, spot, parameters.Volatility[0]);
                double callPrice = pricingResults.Price;
                double delta = pricingResults.Deltas[0];
                double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(numberOfDaysBetweenConvering / totalDays);
                double cashRisk = delta * spot;
                double cashRiskFree = (this.portfolioComposition[underlyingShares[0].Id] - delta) * spot + this.cashRiskFree * freeRate;

                this.currentPortfolioValue = cashRisk + cashRiskFree;
                this.portfolioComposition[option.UnderlyingShareIds[0]] = delta;
                this.currentDate = currentDay;
                this.cashRiskFree = cashRiskFree;
                optionValue = callPrice;

            }
            else
            {
                double[] spots = fillSpots(dataFeed, underlyingShares);
                PricingResults pricingResults = pricer.PriceBasket((BasketOption)option, currentDay, totalDays, spots, parameters.Volatility, parameters.Correlation);
                double[] deltas = pricingResults.Deltas;
                int numberOfUnderlyingShares = underlyingShares.Length;
                double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(numberOfDaysBetweenConvering / totalDays);

                double cashRisk = Tools.productScalar(deltas, spots);
                double[] previousDeltas = new double[numberOfUnderlyingShares];
                for (int i = 0; i < numberOfUnderlyingShares; i++)
                {
                    previousDeltas[i] = this.portfolioComposition[underlyingShares[i].Id];
                }

                double cashRiskFree = Tools.productScalar(Tools.minusArrays(previousDeltas, deltas), spots) + this.cashRiskFree * freeRate;

                this.currentPortfolioValue = cashRisk + cashRiskFree;
                this.cashRiskFree = cashRiskFree;

                for (int i = 0; i < numberOfUnderlyingShares; i++)
                {
                    this.portfolioComposition[underlyingShares[i].Id] = deltas[i];
                }
                this.currentDate = currentDay;
                double pricing = pricingResults.Price;

                optionValue = pricing;

            }
            portfolioValue = this.currentPortfolioValue;

            return new double[2] { optionValue, portfolioValue };

        }

        internal double[] estimatePortfolioFirstDay(ParametersEstimation parameters, Share[] shares, IOption option, DateTime currentDay, DataFeed dataFeed, Pricer pricer)
        {
            SimulatedDataFeedProvider simulator = new SimulatedDataFeedProvider();
            int totalDays = simulator.NumberOfDaysPerYear;
            double optionValue;
            double portfolioValue;
            if (option.GetType() == typeof(VanillaCall))
            {
                double spot = (double)dataFeed.PriceList[shares[0].Id];
                PricingResults pricingResults = pricer.PriceCall((VanillaCall)option, currentDay, totalDays, spot, parameters.Volatility[0]);
                double callPrice = pricingResults.Price;
                this.firstPortfolioValue = callPrice;
                this.currentPortfolioValue = callPrice;
                this.currentDate = currentDay;
                double delta = pricingResults.Deltas[0];

                this.portfolioComposition.Add(option.UnderlyingShareIds[0], delta);
                this.cashRiskFree = callPrice - spot * delta;
            }
            else
            {
                double[] spots = fillSpots(dataFeed, shares);
                PricingResults pricingResults = pricer.PriceBasket((BasketOption)option, currentDay, totalDays, spots, parameters.Volatility, parameters.Correlation);
                double basketPrice = pricingResults.Price;
                double[] deltas = pricingResults.Deltas;
                this.firstPortfolioValue = basketPrice;
                this.currentPortfolioValue = basketPrice;
                this.currentDate = currentDay;
                this.cashRiskFree = basketPrice - Tools.productScalar(deltas, spots);
                for (int i = 0; i < shares.Length; i++)
                {
                    this.portfolioComposition.Add(shares[i].Id, deltas[i]);
                }
            }
            optionValue = this.firstPortfolioValue;
            portfolioValue = this.firstPortfolioValue;

            return new double[2] { optionValue, portfolioValue };
        }

        public static double[] fillSpots(DataFeed data, Share[] shares)
        {
            int size = shares.Length;
            double[] spots = new double[size];
            for (int i = 0; i < size; i++)
            {
                spots[i] = (double)data.PriceList[shares[i].Id];
            }

            return spots;
        }

         #endregion Public Methods

    }
}