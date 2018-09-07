using System;
using System.Collections.Generic;
using System.Linq;
using PricingLibrary.Utilities.MarketDataFeed;
using System.Runtime.InteropServices;


namespace ProjetNet.Models
{
    internal class ParametersEstimation
    {

        #region Private Fields

        private double[,] correlation;
        private double[] volatility;

        #endregion Private Fields

        #region Public Properties

        public double[,] Correlation
        {
            get { return correlation; }
        }

        public double[] Volatility
        {
            get { return volatility; }
        }
        #endregion Private Properties



        #region Public Constructors

        public ParametersEstimation(List<DataFeed> dataFeedList, DateTime rebalancingDate, int estimationWindow)
        {
            List<DataFeed> usefulData = usefulDataFeeds(dataFeedList, rebalancingDate, estimationWindow);
            double[,] assetValues = getAssetValues(usefulData);
            double[,] logAssests = computeWRELogAssets(assetValues);
            double[,] corrMatrix = computeWRECorrelationMatrix(logAssests);
            double[] volTable = computeWREVolatilityTable(logAssests);
            this.correlation = corrMatrix;
            this.volatility = volTable;
        }

        #endregion Public Constructors

        //assetReturns

        private List<DataFeed> usefulDataFeeds(List<DataFeed> dataFeedList, DateTime rebalancingDate, int estimationWindow)
        {
            int indice = 0;
            while(DateTime.Compare(dataFeedList[indice].Date, rebalancingDate) != 0) { indice++;}
            return dataFeedList.GetRange(indice - estimationWindow+1, estimationWindow);
        }

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
        private static extern int WREmodelingLogReturns(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetsValues,
            ref int horizon,
            double[,] assetsReturns,
            ref int info
        );
        private double[,] computeWRELogAssets(double[,] assetValues)
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


        //Correlation

        // import WRE dlls -Corr-
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCorr", CallingConvention = CallingConvention.Cdecl)]

        // declaration
        private static extern int WREmodelingCorr(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetValues,
            double[,] corMatrix,
            ref int info
        );

        public double[,] computeWRECorrelationMatrix(double[,] returns)
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


        //Volatility

        // import WRE dlls -Cov-
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]

        // declaration
        private static extern int WREmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );

        private double[,] computeWRECovarianceMatrix(double[,] returns)
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

        private double[] computeWREVolatilityTable(double[,] returns)
        {
            int numberOfDaysPerYear = 252;
            double[,] covarianceMatrix = computeWRECovarianceMatrix(returns);
            int size = covarianceMatrix.GetLength(0);
            double[] volatilityTable = new Double[size];
            for (int i = 0; i < size; i++)
            {
                volatilityTable[i] = Math.Sqrt(covarianceMatrix[i, i] * numberOfDaysPerYear);
            }
            return volatilityTable;
        }





        //notre calcul
        private double[,] computeLogAssets(double[,] assetValues)
        {
            int nbValues = assetValues.GetLength(0);
            int nbAssets = assetValues.GetLength(1);
            double[,] assetReturns = new double[nbValues - 1, nbAssets];
            for (int i = 0; i < nbValues - 1; i++)
            {
                for (int j = 0; j < nbAssets; j++)
                {
                    assetReturns[i, j] = (double)Math.Log(assetValues[i + 1, j] / assetValues[i, j]);
                }
            }
            return assetReturns;
        }

        private double[,] computeCovarianceMatrix(double[,] returns)
        {
            return Accord.Statistics.Measures.Covariance(returns);
        }


        private double[,] computeCorrelationMatrix(double[,] returns)
        {
            var covarianceMatrix = computeCovarianceMatrix(returns);
            int size = covarianceMatrix.GetLength(0);
            double[,] correlationMatrix = new Double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    correlationMatrix[i, j] = covarianceMatrix[i, j] / (Math.Sqrt(covarianceMatrix[i, i]) * Math.Sqrt(covarianceMatrix[j, j]));
                }
            }
            return correlationMatrix;
        }

        private double[] computeVolatilityTable(double[,] returns)
        {
            var covarianceMatrix = computeCovarianceMatrix(returns);
            int numberOfDaysPerYear = 252;
            int size = covarianceMatrix.GetLength(0);
            double[] volatilityTable = new Double[size];
            for (int i = 0; i < size; i++)
            {
                volatilityTable[i] = Math.Sqrt(covarianceMatrix[i, i] * numberOfDaysPerYear);
            }
            return volatilityTable;
        }

    }
}
