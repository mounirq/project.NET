using System;
using System.Collections.Generic;
using System.Linq;
using PricingLibrary.FinancialProducts;

namespace ProjetNet.Models
{
    internal class ShareTools
    {
        public static String GetShareName(String id)
        {
            using (DataBaseAccessDataContext asdc = new DataBaseAccessDataContext())
            {
                string name = asdc.ShareNames.First(el => (el.id.Trim() == id.Trim())).name.ToString();
                return name;
            }
        }

        public static Share[] GenerateShares(string[] UnderlyingShareIds)
        {
            Share[] shares = new Share[UnderlyingShareIds.Length];
            int k = 0;
            foreach (string underlyingId in UnderlyingShareIds)
            {
                String underlyingName = ShareTools.GetShareName(underlyingId);
                shares[k] = new Share(underlyingName, underlyingId);
                k++;
            }
            return shares;
        }

        public static List<String> GetAllShareIds()
        {
            
            using (DataBaseAccessDataContext asdc = new DataBaseAccessDataContext())
            {
                return asdc.ShareNames.Select(ligne => ligne.id).ToList();
            }
        }

    }

}
