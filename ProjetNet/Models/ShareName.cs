using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetNet.Models
{
    internal class ShareName
    {
        public static String GetShareName(String id)
        {
            using (DataBaseAccessDataContext asdc = new DataBaseAccessDataContext())
            {
                string name = asdc.ShareNames.First(el => (el.id.Trim() == id.Trim())).name.ToString();
                return name;
            }
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
