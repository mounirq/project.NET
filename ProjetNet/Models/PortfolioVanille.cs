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
    internal class PortfolioVanille
    {
        #region Private Fields

        private double firstPortfolioValue;
        private double currentPortfolioValue;
        private VanillaCall call;

        #endregion Private Fields

        #region Public Constructors

        public PortfolioVanille()
        {

        }

        public PortfolioVanille(double firstPortfolioValue, double currentPortfolioValue, VanillaCall call)
        {
            this.firstPortfolioValue = firstPortfolioValue;
            this.currentPortfolioValue = currentPortfolioValue;
            this.call = call;
        }

        #endregion Public Constructors

        public static void Main(string[] args)
        {
            int strike = 7;
            double volatility = 0.4;
            DateTime maturity = new DateTime(2019, 09, 04);
            DateTime beginDate = new DateTime(2018, 09, 04);
            int totalDays = DayCount.CountBusinessDays(beginDate, maturity);

            Share share = new Share("Sfr", "1254798");
            VanillaCall callOption = new VanillaCall("Vanille", share, maturity, strike);

            PortfolioVanille portfolio = new PortfolioVanille();

            double portfolioValue = portfolio.PortfolioValue(callOption, share, totalDays, volatility, beginDate);

            Console.WriteLine("Valeur = " + portfolioValue);

        }


        #region Public Methods
        public double PortfolioValue(VanillaCall callOption, Share share, int totalDays, double volatility,  DateTime beginDate)
        {
            SimulatedDataFeedProvider simulator = new SimulatedDataFeedProvider();
            List<DataFeed> dataFeeds = simulator.GetDataFeed(callOption, beginDate);
            int numberOfDaysPerYear = simulator.NumberOfDaysPerYear;

            Pricer pricer = new Pricer();
            double spot = (double)dataFeeds[0].PriceList[share.Id];
            PricingResults pricingResults = pricer.PriceCall(callOption, beginDate, totalDays, spot, volatility);
            double callPrice = pricingResults.Price;

            this.firstPortfolioValue = callPrice;
            this.currentPortfolioValue = callPrice;
            this.call = callOption;

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
                    Console.WriteLine("Valeur option = " + optionValue[i]);
                    portfolioValue[i] = this.currentPortfolioValue;
                    Console.WriteLine("Valeur portefeuille = " + portfolioValue[i]);
                }
                else if(i == totalDays)
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
            return (this.currentPortfolioValue-payoff)/this.firstPortfolioValue;
            
        }
        #endregion Public Methods


    }
}
