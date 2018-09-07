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
            int strike = 9;
            double volatilityOneDim = 0.4;
            DateTime maturity = new DateTime(2019, 09, 04);
            DateTime beginDate = new DateTime(2018, 09, 04);
            int totalDays = DayCount.CountBusinessDays(beginDate, maturity);

            Share[] share = { new Share("Sfr", "1254798"), new Share("Vodafone","1478963") };
            double[] weights = { 0.3, 0.7 };

            double[] volatility = new double[2];
            volatility[0] = 0.4;
            volatility[1] = 0.4;
            double[,] correlationMatrix = new double[2, 2];
            correlationMatrix[0, 0] = 1;
            correlationMatrix[1, 1] = 0.1;
            correlationMatrix[0, 1] = 1;
            correlationMatrix[1, 0] = 0.1;

            if (share.Length == 1)
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
                PricingResults pricingResults = pricer.PriceCall(callOption, beginDate, totalDays, spot, volatilityOneDim);
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

                int indexArrays = 1;
                /* Skip the first day : because it's already initialized*/
                foreach (DataFeed data in dataFeeds.Skip(1))
                {
                    if (data != dataFeeds.Last())
                    {
                        portfolio.updateValue(spot, data, pricer, callOption, volatilityOneDim, numberOfDaysPerYear);
                        spot = updateSpot(data, share[0]);

                        double pricing = getPricingVanillaCall(data, callOption, pricer, volatilityOneDim, numberOfDaysPerYear);

                        /* Fill arrays of optionValue and portfolioValue */
                        optionValue[indexArrays] = pricing;
                        portfolioValue[indexArrays] = portfolio.currentPortfolioValue;
                    }

                    /* For the last day : */
                    else
                    {
                        //TODO UPDATE LAST VALUE 
                        double cashRiskFreeBeforeUpdate = portfolio.currentPortfolioValue - portfolio.portfolioComposition[share[0].Id] * spot;
                        DateTime currentDay = data.Date;
                        double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberOfDaysPerYear);
                        spot = updateSpot(data, share[0]);
                        portfolio.currentPortfolioValue = spot * portfolio.portfolioComposition[share[0].Id]
                            + cashRiskFreeBeforeUpdate * freeRate;
                        portfolio.currentDate = currentDay;
                        payoff = callOption.GetPayoff(data.PriceList);
                    }
                    indexArrays++;
                }

                /* Partie traçage de courbes */
                using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(@"C:\Users\ensimag\Desktop\WriteLines.txt"))
                {
                    for (int index = 0; index < totalDays; index++)
                    {
                        // If the line doesn't contain the word 'Second', write the line to the file.
                        file.WriteLine(optionValue[index]);
                        file.WriteLine(portfolioValue[index]);
                    }
                }


                double valuePortfolio = (portfolio.currentPortfolioValue - payoff) / portfolio.firstPortfolioValue;
                Console.WriteLine("Valeur = " + valuePortfolio);

            }
            else
            {
                /* Init Values */
                BasketOption basketOption = new BasketOption("BasketOption", share, weights, maturity, strike);
                
                /* Simulated data feeds : */
                SimulatedDataFeedProvider simulator = new SimulatedDataFeedProvider();
                List<DataFeed> dataFeeds = simulator.GetDataFeed(basketOption, beginDate);
                int numberOfDaysPerYear = simulator.NumberOfDaysPerYear;

                /* Portfolio initialisation : */
                Portfolio portfolio = new Portfolio();
                int lengthUnderlyingAssets = share.Length;
                portfolio.portfolioComposition = new Dictionary<String, double>();
                Pricer pricer = new Pricer();
                double[] spots = fillSpots(dataFeeds[0], share);
                PricingResults pricingResults = pricer.PriceBasket(basketOption, beginDate, numberOfDaysPerYear, spots, volatility, correlationMatrix);
                double basketPrice = pricingResults.Price;
                portfolio.firstPortfolioValue = basketPrice;
                portfolio.currentPortfolioValue = basketPrice;
                portfolio.currentDate = beginDate;

                double[] deltas = pricingResults.Deltas;
                for (int i = 0; i<lengthUnderlyingAssets; i++)
                {
                    portfolio.portfolioComposition.Add(share[i].Id, deltas[i]);
                }

                double payoff = 0;
                double[] optionValue = new Double[totalDays];
                double[] portfolioValue = new Double[totalDays];
                optionValue[0] = basketPrice;
                portfolioValue[0] = portfolio.currentPortfolioValue;

                int indexArrays = 1;

                foreach(DataFeed data in dataFeeds.Skip(1))
                {
                    if (data != dataFeeds.Last())
                    {
                        portfolio.updateValueBasket(data, spots, basketOption, share ,pricer, volatility, numberOfDaysPerYear, correlationMatrix);
                        spots = fillSpots(data, share);

                        double pricing = getPricingBasketOption(data, share ,basketOption, pricer, volatility, numberOfDaysPerYear, correlationMatrix);

                        /* Fill arrays of optionValue and portfolioValue */
                        optionValue[indexArrays] = pricing;
                        portfolioValue[indexArrays] = portfolio.currentPortfolioValue;
                    }
                    else
                    {
                        double cashRiskFreeBeforeUpdate = portfolio.calculateCashRiskFree(share, spots);

                        DateTime currentDay = data.Date;
                        double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberOfDaysPerYear);

                        spots = fillSpots(data, share);

                        portfolio.currentPortfolioValue = portfolio.productArrayDictionnary(spots, share)
                            + cashRiskFreeBeforeUpdate * freeRate;
                        portfolio.currentDate = currentDay;
                        payoff = basketOption.GetPayoff(data.PriceList);
                    }
                }

                double valuePortfolio = (portfolio.currentPortfolioValue - payoff) / portfolio.firstPortfolioValue;
                Console.WriteLine("Valeur = " + valuePortfolio);

            }
        }


        #region Public Methods

        private double productArrayDictionnary(double[] array, Share[] share)
        {
            if (array.Length != this.portfolioComposition.Count) { throw new ArgumentOutOfRangeException("The array and the dictionnary must contain same size of element"); }
            int size = array.Length;
            double[] returnedArray = new double[size];
            for (int i = 0; i < size; i++)
            {
                returnedArray[i] = this.portfolioComposition[share[i].Id];
            }

            return productScalar(array,returnedArray);
        }

        private double calculateCashRiskFree(Share[] share, double[] spots)
        {
            int numberOfUnderlyingShare = this.portfolioComposition.Count;
            double[] composition = new double[numberOfUnderlyingShare];
            for (int i = 0; i < numberOfUnderlyingShare; i++)
            {
                composition[i] = this.portfolioComposition[share[i].Id];
            }

            return this.currentPortfolioValue - productScalar(composition, spots);
        }

        private static double getPricingBasketOption(DataFeed data, Share[] underlyingShares ,BasketOption basketOption, Pricer pricer, double[] volatility, int numberOfDaysPerYear, double[,] correlationMatrix)
        {
            DateTime currentDay = data.Date;
            double[] spots = fillSpots(data, underlyingShares);
            PricingResults pricingResults = pricer.PriceBasket(basketOption, currentDay, numberOfDaysPerYear, spots, volatility, correlationMatrix);
            return pricingResults.Price;
        }
        

        public void updateValue(double previousSpot, DataFeed data, Pricer pricer, VanillaCall callOption, double volatility, int numberOfDaysPerYear)
        {
            DateTime currentDay = data.Date;
            Share share = callOption.UnderlyingShare;
            double spot = updateSpot(data, share);

            PricingResults pricingResults = pricer.PriceCall(callOption, currentDay, numberOfDaysPerYear, spot, volatility);
            double currentDelta = pricingResults.Deltas[0];
            double cashRisk = currentDelta * spot;
            double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberOfDaysPerYear);
            double cashRiskFree = (this.portfolioComposition[share.Id] - currentDelta) * spot +
                (this.currentPortfolioValue - this.portfolioComposition[share.Id] * previousSpot) * freeRate;

            this.portfolioComposition[share.Id] = currentDelta;

            this.currentPortfolioValue = cashRisk + cashRiskFree;
            this.currentDate = currentDay;
        }

        private void updateValueBasket(DataFeed data, double[] previousSpots, BasketOption basketOption, Share[] underlyingShares,Pricer pricer, double[] volatility, int numberOfDaysPerYear, double[,] correlationMatrix)
        {
            DateTime currentDay = data.Date;

            double[] spots = fillSpots(data, underlyingShares);
            PricingResults pricingResults = pricer.PriceBasket(basketOption, currentDay, numberOfDaysPerYear, spots, volatility, correlationMatrix);
            double[] deltas = pricingResults.Deltas;
            int numberOfUnderlyingShares = underlyingShares.Length;
            double freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberOfDaysPerYear);

            double cashRisk = productScalar(deltas, spots);
            double[] previousDeltas = new double[numberOfUnderlyingShares];
            for (int i = 0; i < numberOfUnderlyingShares; i++)
            {
                previousDeltas[i] = this.portfolioComposition[underlyingShares[i].Id];
            }

            double cashRiskFree = productScalar(minusArrays(previousDeltas, deltas), spots) + 
                (this.currentPortfolioValue - productScalar(previousDeltas,previousSpots)) * freeRate;

            this.currentPortfolioValue = cashRisk + cashRiskFree;

            for(int i = 0; i<numberOfUnderlyingShares; i++)
            {
                this.portfolioComposition[underlyingShares[i].Id] = deltas[i];
            }
            this.currentDate = currentDay;
        }

        public static double[] minusArrays(double[] firstArray, double[] secondArray)
        {
            if (firstArray.Length != secondArray.Length) { throw new ArgumentOutOfRangeException("The arrays must have the same size"); }
            int size = firstArray.Length; 
            double[] newArray = new double[size];
            for (int i = 0; i < size; i++)
            {
                newArray[i] = firstArray[i] - secondArray[i];
            }
            return newArray;
        }

        public static double productScalar(double[] firstArray, double[] secondArray)
        {
            if (firstArray.Length != secondArray.Length) { throw new ArgumentOutOfRangeException("The arrays must have the same size"); }
            int size = firstArray.Length;
            double value = 0;
            for (int i = 0; i < size; i++)
            {
                value += firstArray[i] * secondArray[i];
            }
            return value;
        }

        public static double[] fillSpots(DataFeed data, Share[] shares)
        {
            int size = shares.Length;
            double[] spots = new double[shares.Length];
            for (int i = 0; i < size; i++)
            {
                spots[i] = (double)data.PriceList[shares[i].Id];
            }

            return spots;
        }
        private static double getPricingVanillaCall(DataFeed data, VanillaCall callOption, Pricer pricer, double volatility, int numberOfDaysPerYear)
        {
            DateTime currentDay = data.Date;
            double spot = (double)data.PriceList[callOption.UnderlyingShare.Id];
            PricingResults pricingResults = pricer.PriceCall(callOption, currentDay, numberOfDaysPerYear, spot, volatility);
            return pricingResults.Price;
        }

        public static double updateSpot(DataFeed data, Share share)
        {
            return (double)data.PriceList[share.Id];
        }

         #endregion Public Methods

    }
}
