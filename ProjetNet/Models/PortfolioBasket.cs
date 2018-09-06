using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.Computations;

namespace ProjetNet.Models
{
    class PortfolioBasket
    {
        private double portfolioValue;
        private double basketPrice;
        private BasketOption basket;

        public PortfolioBasket(double portfolioValue, BasketOption basket)
        {
            this.portfolioValue = portfolioValue;
            this.basketPrice = portfolioValue;
            this.basket = basket;
        }

        public static void Main(string[] args)
        {
            var simulatedData = new SimulatedDataProvider();
            DateTime from = new DateTime(2018, 09, 04);
            Share share1 = new Share("vod.l", "vod.l");
            Share share2 = new Share("ftse", "ftse");
            string nameBasket = "Basket";
            double strikeBasket = 7000;
            Share[] sharesBasket = { share1, share2 };
            Double[] weights = { 0.3, 0.7 };
            DateTime maturityBasket = new DateTime(2019, 09, 04);
            BasketOption optionBasket = new BasketOption(nameBasket, sharesBasket, weights, maturityBasket, strikeBasket);
            var simulationBasket = simulatedData.GetDataFeeds(optionBasket, from);
            double[,] correlationMatrix = new double[2, 2];
            correlationMatrix[0, 0] = 0.1;
            correlationMatrix[1, 1] = 0.1;
            correlationMatrix[0, 1] = 1;
            correlationMatrix[1, 0] = 1;

            SimulatedDataFeedProvider simulation = new SimulatedDataFeedProvider();
            int numberDaysPerYear = simulation.NumberOfDaysPerYear;

            int size = sharesBasket.Length;
            double[] spotsPrev = new double[size];
            double[] volatility = new double[2];
            volatility[0] = 0.4;
            volatility[1] = 0.4;

            for (int i=0; i<size; i++)
            {
                spotsPrev[i] = (double) simulationBasket[0].PriceList[sharesBasket[i].Name];
            }

            Pricer pricer = new Pricer();
            PricingResults pricesResults = pricer.PriceBasket(optionBasket, from, numberDaysPerYear, spotsPrev, volatility, correlationMatrix);

            double priceBasket = pricesResults.Price;
            PortfolioBasket portfolioBasket = new PortfolioBasket(priceBasket, optionBasket);

            double[] deltaPrev = new double[size];
            double cashRiskFreePrev = priceBasket;
            for (int i=0; i<size; i++)
            {
                deltaPrev[i] = pricesResults.Deltas[i];
                cashRiskFreePrev = cashRiskFreePrev - deltaPrev[i] * spotsPrev[i];
            }

            int index = 0;
            double[] delta = new double[size];
            double[] spots = new double[size];
            double cashRiskFree;
            double tmp = 0;
            double freeRate;
            double cashRisk = 0;
            DateTime today;
            foreach (DataFeed data in simulationBasket)
            {
                if( index!= 0)
                {
                    today = data.Date;
                    /* Remplir les spots */
                    for (int i=0; i<size; i++)
                    {
                        spots[i] = (double)simulationBasket[index].PriceList[sharesBasket[i].Name];
                    }
                    pricesResults = pricer.PriceBasket(optionBasket, today, numberDaysPerYear, spots, volatility, correlationMatrix);
                    
                    /* Remplir les deltas */
                    for (int i=0; i<size; i++)
                    {
                        delta[i] = pricesResults.Deltas[i];
                        cashRisk = cashRisk + delta[i] * spots[i];
                        tmp = (deltaPrev[i] - delta[i]) * spots[i];
                    }
                    freeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(1 / numberDaysPerYear);
                    cashRiskFree = tmp + cashRiskFreePrev * freeRate;
                    portfolioBasket.portfolioValue = cashRiskFree + cashRisk;



                } 
                index++;
            }

            Console.WriteLine("Valeur Portefeuille = " + portfolioBasket.portfolioValue);
        }

    }
    


}
