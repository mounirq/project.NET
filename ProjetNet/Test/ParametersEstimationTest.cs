using System;
using System.Collections.Generic;
using PricingLibrary.Utilities.MarketDataFeed;
using ProjetNet.Models;
using PricingLibrary.FinancialProducts;

namespace ProjetNet.Test
{
    class ParametersEstimationTest
    {
        public static void Main(string[] args)
        {
            var simulatedData = new SimulatedDataProvider();
            DateTime from = new DateTime(2010, 01, 01);
            Share share1 = new Share("vod.l", "vod.l");
            Share share2 = new Share("ftse", "ftse");
            Share share3 = new Share("sfr", "sfr");
            Share share4 = new Share("sx5e", "sx5e");
            string nameBasket = "Basket";
            double strikeBasket = 7000;
            Share[] sharesBasket = { share1, share2, share3, share4 };
            Double[] weights = { 0.2, 0.5, 0.2, 0.1 };
            DateTime maturityBasket = new DateTime(2151, 01, 20);
            IOption optionBasket = new BasketOption(nameBasket, sharesBasket, weights, maturityBasket, strikeBasket);
            List<DataFeed> simulationBasket = simulatedData.GetDataFeeds(optionBasket, from);


            ParametersEstimation parameters = new ParametersEstimation(simulationBasket, new DateTime(2150, 01, 20),15000);
            var correlationMatrix = parameters.Correlation;
            int Size = correlationMatrix.GetLength(0);
            Console.WriteLine("\n \n \n La matrice de correlation est : \n ");
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Console.Write(correlationMatrix[i, j] + new string(' ', 30 - correlationMatrix[i, j].ToString().Length));
                }
                Console.Write("\n");
            }

            var volatilityTable = parameters.Volatility;
            Console.WriteLine("\n \n \n la volatilite est : \n ");
            for (int i = 0; i < Size; i++)
            {
                Console.WriteLine(volatilityTable[i]);
            }
            Console.ReadKey(true);
        }
    }
}
