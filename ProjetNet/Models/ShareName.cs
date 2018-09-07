using System;
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

    }

}
