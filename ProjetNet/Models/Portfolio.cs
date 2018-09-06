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

        public static void Main(string[] args)
        {
            int strike = 7;
            double volatility = 0.4;
            DateTime maturity = new DateTime(2019, 09, 04);
            DateTime beginDate = new DateTime(2018, 09, 04);
            int totalDays = DayCount.CountBusinessDays(beginDate, maturity);

            Share[] share = { new Share("Sfr", "1254798") };
            if(share.Length == 1)
            {
                VanillaCall callOption = new VanillaCall("Vanille", share[0], maturity, strike);
                
                /* Simulated data feeds : */
                SimulatedDataFeedProvider simulator = new SimulatedDataFeedProvider();
                List<DataFeed> dataFeeds = simulator.GetDataFeed(callOption, beginDate);
                int numberOfDaysPerYear = simulator.NumberOfDaysPerYear;

                /* Portfolio initialisation : */
                Portfolio portfolio = new Portfolio();
                portfolio.portfolioComposition = new Dictionary<String, double>();

                Pricer pricer = new Pricer();
                double spot = (double)dataFeeds[0].PriceList[share[0].Id];
                PricingResults pricingResults = pricer.PriceCall(callOption, beginDate, totalDays, spot, volatility);
                double callPrice = pricingResults.Price;
                portfolio.firstPortfolioValue = callPrice;
                portfolio.currentPortfolioValue = callPrice;
                portfolio.currentDate = beginDate;
                double previousDelta = pricingResults.Deltas[0];

                portfolio.portfolioComposition.Add(share[0].Id, previousDelta);

                double payoff = 0;
                double[] optionValue = new Double[totalDays];
                double[] portfolioValue = new Double[totalDays];
                optionValue[0] = callPrice;
                portfolioValue[0] = portfolio.currentPortfolioValue;

                /* Skip the first day : because it's already initialized*/
                foreach (DataFeed data in dataFeeds.Skip(1))
                {
                    if(data != dataFeeds.Last())
                    {
                        portfolio.updateValue(spot, data, pricer, callOption, volatility, numberOfDaysPerYear);
                        spot = updateSpot(data, share[0]);
                    }

                    /* For the last day :  */
                    else
                    {
                        portfolio.currentDate = data.Date;
                        payoff = callOption.GetPayoff(data.PriceList);
                    }
                }

                double valuePortfolio = (portfolio.currentPortfolioValue - payoff) / portfolio.firstPortfolioValue;
                Console.WriteLine("Valeur = " + valuePortfolio);

                Portfolio portefolioTest = new Portfolio();
                double finalportfolioValue = portfolio.updatePortfolioValue(pricer, callOption, dataFeeds, numberOfDaysPerYear, share[0], totalDays, volatility, beginDate);

                Console.WriteLine("Valeur = " + finalportfolioValue);
            }
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
                    double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberOfDaysPerYear);
                    spot = (double)data.PriceList[share.Id];
                    pricingResults = pricer.PriceCall(callOption, currentDay, numberOfDaysPerYear, spot, volatility);
                    currentDelta = pricingResults.Deltas[0];
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

            /* Partie traçage de courbes
            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"C:\Users\ensimag\Desktop\WriteLines.txt"))
            {
                for (int index= 0; index < totalDays; index++)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(optionValue[index]);
                    file.WriteLine(portfolioValue[index]);
                }
            }
            */
            return (this.currentPortfolioValue - payoff) / this.firstPortfolioValue;

        }
        #endregion Public Methods

    }
}
