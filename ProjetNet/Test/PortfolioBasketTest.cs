using System;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities;
using ProjetNet.Models;

namespace ProjetNet.Test
{
    class PortfolioBasketTest
    {
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
