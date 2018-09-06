using System;
using System.Collections.Generic;
using System.Linq;
using PricingLibrary.Utilities.MarketDataFeed;
using System.Runtime.InteropServices;
using PricingLibrary.FinancialProducts;


namespace ProjetNet.Models
{
    internal class ComputingMatrix
    {

        private double[,] getAssetValues(List<DataFeed> dataFeedList)
        {
            int nbDate = dataFeedList.Count;
            int nbUnderlings = dataFeedList[0].PriceList.Count;
            String[] underlyingIds = dataFeedList[0].PriceList.Keys.ToArray();
            double[,] assets = new double[nbDate, nbUnderlings];
            for (int i = 0; i < nbDate; i++)
            {
                for (int j = 0; j < nbUnderlings; j++)
                {
                    assets[i, j] = (double)dataFeedList[i].PriceList[underlyingIds[j]];
                }
            }
            return assets;
        }
        
        // import WRE dlls -Log-
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingLogReturns", CallingConvention = CallingConvention.Cdecl)]

        // declaration
        public static extern int WREmodelingLogReturns(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetsValues,
            ref int horizon,
            double[,] assetsReturns,
            ref int info
        );
        private double[,] computeLogAssets(double[,] assetValues)
        {

            int nbValues = assetValues.GetLength(0);
            int nbAssets = assetValues.GetLength(1);
            int info = 0;
            int res;
            int horizon = 1;
            double[,] assetReturns = new double[nbValues-1, nbAssets];
            res = WREmodelingLogReturns(ref nbValues, ref nbAssets, assetValues, ref horizon, assetReturns, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingCov encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingCov encountred a problem. See info parameter for more details");
            }
            return assetReturns;
        }


        // import WRE dlls -Cov-
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]

        // declaration
        public static extern int WREmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );

        public double[,] computeCovarianceMatrix(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] covMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int res;
            res = WREmodelingCov(ref dataSize, ref nbAssets, returns, covMatrix, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingCov encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingCov encountred a problem. See info parameter for more details");
            }
            return covMatrix;
        }

        public double[,] constructCovarianceMatrix(List<DataFeed> dataFeedList)
        {
            double[,] assetValues = getAssetValues(dataFeedList);
            double[,] logAssests = computeLogAssets(assetValues);
            double[,] covMatrix = computeCovarianceMatrix(logAssests);

            //int size = dailyCovMatrix.GetLength(0);
            //SimulatedDataFeedProvider truc = new SimulatedDataFeedProvider();
            //double[,] covMatrix = new double[size, size];
            //for (int i = 0; i < size; i++)
            //{
            //    for (int j = 0; j < size; j++)
            //    {
            //        covMatrix[i, j] = Math.Sqrt(dailyCovMatrix[i, j] * truc.NumberOfDaysPerYear);
            //    }
            //}
            return covMatrix;
        }

        public double[] constructVarianceTable(List<DataFeed> dataFeedList)
        {
            var covarianceMatrix = constructCovarianceMatrix(dataFeedList);
            int size = covarianceMatrix.GetLength(0);
            double[] varianceTable = new Double[size];
            for (int i = 0; i < size; i++)
            {
                varianceTable[i] = covarianceMatrix[i, i];
            }
            return varianceTable;
        }

        public double[,] constructCorrelationMatrix(List<DataFeed> dataFeedList)
        {
            var covarianceMatrix = constructCovarianceMatrix(dataFeedList);
            double[] varianceTable = constructVarianceTable(dataFeedList);
            int size = covarianceMatrix.GetLength(0);
            double[,] correlationMatrix = new Double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    correlationMatrix[i, j] = covarianceMatrix[i, j] / (Math.Sqrt(varianceTable[i]) * Math.Sqrt(varianceTable[j]));
                }
            }
            return correlationMatrix;
        }


        // import WRE dlls -Corr-
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCorr", CallingConvention = CallingConvention.Cdecl)]

        // declaration
        public static extern int WREmodelingCorr(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetValues,
            double[,] corMatrix,
            ref int info
        );

        public double[,] computeCorrelationMatrix(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] corrMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int res;
            res = WREmodelingCorr(ref dataSize, ref nbAssets, returns, corrMatrix, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingCov encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingCov encountred a problem. See info parameter for more details");
            }
            return corrMatrix;
        }

        public double[,] constructLeurCorrelationMatrix(List<DataFeed> dataFeedList, int horizon)
        {
            double[,] assetValues = getAssetValues(dataFeedList);
            double[,] logAssests = computeLogAssets(assetValues);
            double[,] corrMatrix = computeCorrelationMatrix(logAssests);
            return corrMatrix;
        }
        

        public static void Main(string[] args)
        {
            // header
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
            DateTime maturityBasket = new DateTime(2030, 09, 10);
            IOption optionBasket = new BasketOption(nameBasket, sharesBasket, weights, maturityBasket, strikeBasket);
            List<DataFeed> simulationBasket = simulatedData.GetDataFeeds(optionBasket, from);

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

            var leurcorrelationMatrix = computingMatrix.constructLeurCorrelationMatrix(simulationBasket, 1);
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

            var varianceTable = computingMatrix.constructVarianceTable(simulationBasket);
            int sizevariance = varianceTable.GetLength(0);
            Console.WriteLine("\n \n \n la variance est : \n ");
            for (int i = 0; i < leurSize; i++)
            {
                Console.WriteLine(Math.Sqrt(varianceTable[i]*365));
            }
            Console.ReadKey(true);
        }
    }
}
