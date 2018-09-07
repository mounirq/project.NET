using System;
using System.Collections.Generic;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;


namespace ProjetNet.Models
{
    internal class Portfolio
    {
        #region Private Fields

        private double firstPortfolioValue;
        private Dictionary<String, double> portfolioComposition;
        private double currentPortfolioValue;
        private DateTime currentDate;

        #endregion Private Fields

        #region Public Constructors

        public Portfolio(){}

        public Portfolio(double firstPortfolioValue, Dictionary<String, double> portfolioComposition, DateTime currentDate)
        {
            this.firstPortfolioValue = firstPortfolioValue;
            this.portfolioComposition = portfolioComposition;
            currentPortfolioValue = firstPortfolioValue;
            this.currentDate = currentDate;
        }

        #endregion Public Constructors

        private static double getPricingResult(DataFeed data, VanillaCall callOption, Pricer pricer, double volatility, int numberOfDaysPerYear)
        {
            DateTime currentDay = data.Date;
            double spot = (double)data.PriceList[callOption.UnderlyingShare.Id];
            PricingResults pricingResults = pricer.PriceCall(callOption, currentDay, numberOfDaysPerYear, spot, volatility);
            return pricingResults.Price;
        }



        #region Public Methods

        public static double updateSpot(DataFeed data, Share share)
        {
            return (double)data.PriceList[share.Id];
        }

        public void updateValue(double previousSpot, DataFeed data, Pricer pricer, VanillaCall callOption, double volatility, int numberOfDaysPerYear)
        {
            DateTime currentDay = data.Date;
            Share share = callOption.UnderlyingShare;
            double spot = updateSpot(data, share);
            
            PricingResults pricingResults = pricer.PriceCall(callOption, currentDay, numberOfDaysPerYear, spot, volatility);
            double currentDelta = pricingResults.Deltas[0];
            double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberOfDaysPerYear);
            double cashRiskFree = (this.portfolioComposition[share.Id] - currentDelta) * spot + 
                (this.currentPortfolioValue - this.portfolioComposition[share.Id] * previousSpot) * freeRate;

            this.portfolioComposition[share.Id] = currentDelta;

            this.currentPortfolioValue = currentDelta * spot + cashRiskFree;
            this.currentDate = currentDay;
        }

        public double updatePortfolioValue(Pricer pricer, VanillaCall callOption, List<DataFeed> dataFeeds, int numberOfDaysPerYear, Share share, int totalDays, double volatility,  DateTime beginDate)
        {
            double spot = (double)dataFeeds[0].PriceList[share.Id];
            PricingResults pricingResults = pricer.PriceCall(callOption, beginDate, totalDays, spot, volatility);
            double callPrice = pricingResults.Price;

            this.firstPortfolioValue = callPrice;
            this.currentPortfolioValue = callPrice;

            double deltaInit = pricingResults.Deltas[0];
            double riskFreecash = callPrice - deltaInit * spot;

            double currentDelta;
            double prevDelta = deltaInit;
            double payoff = 0;
            DateTime currentDay;
            int i = 0;
            double[] optionValue = new Double[totalDays];
            double[] portfolioValue = new Double[totalDays];
            optionValue[0] = callPrice;
            portfolioValue[0] = this.currentPortfolioValue;

            foreach (DataFeed data in dataFeeds)
            {
                if (i != 0 && i != totalDays)
                {
                    currentDay = data.Date;
                    spot = (double)data.PriceList[share.Id];
                    pricingResults = pricer.PriceCall(callOption, currentDay, numberOfDaysPerYear, spot, volatility);
                    currentDelta = pricingResults.Deltas[0];

                    double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberOfDaysPerYear);
                    riskFreecash = (prevDelta - currentDelta) * spot + riskFreecash * freeRate;
                    this.currentPortfolioValue = currentDelta * spot + riskFreecash;
                    prevDelta = currentDelta;

                    optionValue[i] = pricingResults.Price;
                    //Console.WriteLine("Valeur option = " + optionValue[i]);
                    portfolioValue[i] = this.currentPortfolioValue;
                    //Console.WriteLine("Valeur portefeuille = " + portfolioValue[i]);
                }
                else if (i == totalDays)
                {
                    payoff = callOption.GetPayoff(data.PriceList);
                }
                i++;
            }

            /* Partie traçage de courbes*/
            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"C:\Users\ensimag\Desktop\WriteLinesVanille.txt"))
            {
                for (int index= 0; index < totalDays; index++)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(optionValue[index]);
                    file.WriteLine(portfolioValue[index]);
                }
            }
            
            return (this.currentPortfolioValue - payoff) / this.firstPortfolioValue;

        }
        #endregion Public Methods

    }
}
