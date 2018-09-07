using System;
using System.Collections.Generic;
using PricingLibrary.Utilities.MarketDataFeed;
using ProjetNet.Models;
using PricingLibrary.FinancialProducts;

namespace ProjetNet.Test
{
    class ComputingMatrixTest
    {
        public static void Main(string[] args)
        {
            var simulatedData = new SimulatedDataProvider();
            var computingMatrix = new ComputingMatrix();
            DateTime from = new DateTime(2018, 09, 04);
            Share share1 = new Share("vod.l", "vod.l");
            Share share2 = new Share("ftse", "ftse");
            Share share3 = new Share("sfr", "sfr");
            Share share4 = new Share("sx5e", "sx5e");
            string nameBasket = "Basket";
            double strikeBasket = 7000;
            Share[] sharesBasket = { share1, share2, share3, share4 };
            Double[] weights = { 0.2, 0.5, 0.2, 0.1 };
            DateTime maturityBasket = new DateTime(2500, 09, 10);
            IOption optionBasket = new BasketOption(nameBasket, sharesBasket, weights, maturityBasket, strikeBasket);
            List<DataFeed> simulationBasket = simulatedData.GetDataFeeds(optionBasket, from);


            // Avec notre calcul
            var correlationMatrix = computingMatrix.constructCorrelationMatrix(simulationBasket);
            int size = correlationMatrix.GetLength(0);
            Console.WriteLine("notre matrice de correlation est : ");
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(correlationMatrix[i, j] + new string(' ', 30 - correlationMatrix[i, j].ToString().Length));
                }
                Console.Write("\n");
            }


            // Avec WRE calcul
            var leurcorrelationMatrix = computingMatrix.constructWRECorrelationMatrix(simulationBasket, 1);
            int leurSize = leurcorrelationMatrix.GetLength(0);
            Console.WriteLine("\n \n \n WRE matrice de correlation est : \n ");
            for (int i = 0; i < leurSize; i++)
            {
                for (int j = 0; j < leurSize; j++)
                {
                    Console.Write(leurcorrelationMatrix[i, j] + new string(' ', 30 - leurcorrelationMatrix[i, j].ToString().Length));
                }
                Console.Write("\n");
            }

            var volatilityTable = computingMatrix.constructVolatilityTable(simulationBasket);
            int sizevariance = volatilityTable.GetLength(0);
            Console.WriteLine("\n \n \n la volatilite est : \n ");
            for (int i = 0; i < leurSize; i++)
            {
                Console.WriteLine(volatilityTable[i]);
            }
            Console.ReadKey(true);
        }
    }
}
