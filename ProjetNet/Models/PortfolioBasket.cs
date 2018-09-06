using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.Computations;
using PricingLibrary.Utilities;

namespace ProjetNet.Models
{
    class PortfolioBasket
    {
        private double portfolioValue;
        private double basketPriceInit;
        private BasketOption basket;

        public PortfolioBasket()
        {

        }

        public PortfolioBasket(double portfolioValue, BasketOption basket)
        {
            this.portfolioValue = portfolioValue;
            this.basketPriceInit = portfolioValue;
            this.basket = basket;
        }


        public double PortfolioValue(BasketOption optionBasket, Share[] sharesBasket, int totalDays, double[] volatility, double[,] correlationMatrix, DateTime beginDate)
        {
            this.basket = optionBasket;
            SimulatedDataFeedProvider simulator = new SimulatedDataFeedProvider();
            List<DataFeed> simulationBasket = simulator.GetDataFeed(optionBasket, beginDate);
            int numberDaysPerYear = simulator.NumberOfDaysPerYear;

            int size = sharesBasket.Length; // Number of Shares
            double[] spotsPrev = new double[size]; // Array that will stock the spots values of the last rebalancing
            spotsPrev = fillSpots(simulationBasket[0], sharesBasket, size); // We initialize the array with the spots of the first day of analysis

            /* Calculation of the option price and the initial composition of the portfolio */
            Pricer pricer = new Pricer();
            PricingResults pricesResults = pricer.PriceBasket(optionBasket, beginDate, numberDaysPerYear, spotsPrev, volatility, correlationMatrix);
            double priceBasket = pricesResults.Price;
            double[] deltaPrev = new double[size];
            deltaPrev = pricesResults.Deltas;
            double cashRiskFreePrev = priceBasket - dotArrays(deltaPrev, spotsPrev, size);

            this.portfolioValue = priceBasket; 
            this.basketPriceInit = priceBasket;

            int index = 0;
            double[] delta = new double[size];
            double[] spots = new double[size];
            double cashRiskFree = 0;
            double variationCashRisk = 0;
            double freeRate = 0;
            double cashRisk = 0;
            DateTime today;
            double[] optionValue = new double[totalDays];
            double[] portfolioValue = new double[totalDays];
            double payoff = 0;

            /* Arrays used for the plot */
            optionValue[0] = pricesResults.Price;
            portfolioValue[0] = this.portfolioValue;

            foreach (DataFeed data in simulationBasket)
            {
                if (index != 0 && index != totalDays)
                {
                    cashRisk = 0;
                    variationCashRisk = 0;
                    today = data.Date;

                    /* Update spots */
                    spots = fillSpots(data, sharesBasket, size);

                    /* Update priceResults */
                    pricesResults = pricer.PriceBasket(optionBasket, today, numberDaysPerYear, spots, volatility, correlationMatrix);

                    /* Update deltas */
                    delta = pricesResults.Deltas;

                    /* Update cashRisk */
                    cashRisk = dotArrays(delta, spots, size);

                    variationCashRisk = dotArrays(minusArrays(deltaPrev, delta, size), spots, size);
                    freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberDaysPerYear);

                    /* Update cashRiskFree */
                    cashRiskFree = variationCashRisk + cashRiskFreePrev * freeRate;

                    /* Update portfolioValue */
                    this.portfolioValue = cashRiskFree + cashRisk;

                    /* Memorize the delta and the cashRiskFree calculated for the next balancing */
                    deltaPrev = delta;
                    cashRiskFreePrev = cashRiskFree;

                    optionValue[index] = pricesResults.Price;
                    portfolioValue[index] = this.portfolioValue;
                }
                else if (index == totalDays)
                {
                    payoff = optionBasket.GetPayoff(data.PriceList);
                }

                index++;
            }

            /* Partie traçage de courbes */
            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"C:\Users\ensimag\Desktop\WriteLinesBasket.txt"))
            {
                for (int i = 0; i < totalDays; i++)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(optionValue[i]);
                    file.WriteLine(portfolioValue[i]);
                }
            }

            return (this.portfolioValue - payoff) / this.basketPriceInit;
        }

        /* Returns the spots array from a DataFeed and an array of shares */ 
        public static double[] fillSpots(DataFeed data, Share[] shares, int size)
        {
            double[] spots = new double[size];
            for (int i=0; i<size; i++)
            {
                spots[i] = (double) data.PriceList[shares[i].Name];
            }

            return spots;
        }

        /* Returns the scalar product of two arrays */
        public static double dotArrays(double[] firstArray, double[] secondArray, int size)
        {
            double value = 0;
            for (int i=0; i<size; i++)
            {
                value +=  firstArray[i] * secondArray[i];
            }
            return value;
        }

        /* Returns an array where each element is the result of the substraction of the elements of both arrays */
        public static double[] minusArrays(double[] firstArray, double[] secondArray, int size)
        {
            double[] newArray = new double[size];
            for (int i = 0; i < size; i++)
            {
                newArray[i] = firstArray[i] - secondArray[i];
            }
            return newArray;
        }

        

        public static void Main(string[] args)
        {
            var simulatedData = new SimulatedDataProvider();
            DateTime from = new DateTime(2018, 09, 04);
            Share share1 = new Share("vod.l", "vod.l");
            Share share2 = new Share("ftse", "ftse");
            string nameBasket = "Basket";
            double strikeBasket = 9;
            Share[] sharesBasket = { share1, share2 };
            Double[] weights = { 0.3, 0.7 };
            DateTime maturityBasket = new DateTime(2019, 09, 04);
            BasketOption optionBasket = new BasketOption(nameBasket, sharesBasket, weights, maturityBasket, strikeBasket);
            PortfolioBasket portfolioBasket = new PortfolioBasket();
            int totalDays = DayCount.CountBusinessDays(from, maturityBasket);
            double[] volatility = new double[2];
            volatility[0] = 0.4;
            volatility[1] = 0.4;
            double[,] correlationMatrix = new double[2, 2];
            correlationMatrix[0, 0] = 1;
            correlationMatrix[1, 1] = 0.1;
            correlationMatrix[0, 1] = 1;
            correlationMatrix[1, 0] = 0.1;
            double valeur = portfolioBasket.PortfolioValue(optionBasket, sharesBasket, totalDays, volatility, correlationMatrix, from);
            Console.WriteLine("Valeur Gain normalisée = " + valeur);
        }

        

    }

    



}
