using System;
using System.Collections.Generic;
using System.Linq;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using PricingLibrary.Utilities;
using ProjetNet.Models;

namespace ProjetNet.Test
{
    class PortfolioTest
    {
        public static void Main(string[] args)
        {
            int strike = 9;
            double volatility = 0.4;
            DateTime maturity = new DateTime(2019, 09, 04);
            DateTime beginDate = new DateTime(2018, 09, 04);
            int totalDays = DayCount.CountBusinessDays(beginDate, maturity);

            Share[] share = { new Share("Sfr", "1254798") };
            if (share.Length == 1)
            {
                VanillaCall callOption = new VanillaCall("Vanille", share[0], maturity, strike);

                /* Simulated data feeds : */
                SimulatedDataFeedProvider simulator = new SimulatedDataFeedProvider();
                List<DataFeed> dataFeeds = simulator.GetDataFeed(callOption, beginDate);
                int numberOfDaysPerYear = simulator.NumberOfDaysPerYear;

                /* Portfolio initialisation : */
                Portfolio portfolio = new Portfolio();
                portfolio.PortfolioComposition = new Dictionary<String, double>();

                Pricer pricer = new Pricer();



                double spot = (double)dataFeeds[0].PriceList[share[0].Id];
                PricingResults pricingResults = pricer.PriceCall(callOption, beginDate, totalDays, spot, volatility);
                double callPrice = pricingResults.Price;
                portfolio.FirstPortfolioValue = callPrice;
                portfolio.CurrentPortfolioValue = callPrice;
                portfolio.CurrentDate = beginDate;
                double previousDelta = pricingResults.Deltas[0];

                portfolio.PortfolioComposition.Add(share[0].Id, previousDelta);

                double payoff = 0;
                double[] optionValue = new Double[totalDays];
                double[] portfolioValue = new Double[totalDays];
                optionValue[0] = callPrice;
                portfolioValue[0] = portfolio.CurrentPortfolioValue;

                int indexArrays = 1;
                /* Skip the first day : because it's already initialized*/
                foreach (DataFeed data in dataFeeds.Skip(1))
                {
                    if (data != dataFeeds.Last())
                    {
                        portfolio.updateValue(spot, data, pricer, callOption, volatility, numberOfDaysPerYear);
                        spot = updateSpot(data, share[0]);

                        double pricing = getPricingResult(data, callOption, pricer, volatility, numberOfDaysPerYear);

                        /* Fill arrays of optionValue and portfolioValue */

                        optionValue[indexArrays] = pricing;
                        portfolioValue[indexArrays] = portfolio.CurrentPortfolioValue;

                        Console.WriteLine("Valeur option = " + optionValue[indexArrays]);
                        Console.WriteLine("Valeur portefeuille = " + portfolioValue[indexArrays]);

                    }

                    /* For the last day : */
                    else
                    {
                        portfolio.CurrentDate = data.Date;
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


                //double valuePortfolio = (portfolio.currentPortfolioValue - payoff) / portfolio.firstPortfolioValue;
                //Console.WriteLine("Valeur = " + valuePortfolio);

                Portfolio portefolioTest = new Portfolio();
                double finalportfolioValue = portfolio.updatePortfolio(pricer, callOption, dataFeeds, numberOfDaysPerYear, share[0], totalDays, volatility, beginDate);

                Console.WriteLine("Valeur = " + finalportfolioValue);
            }
        }
    }
}
