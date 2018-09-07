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

        /*public static void Main(string[] args)
        {
            Console.WriteLine("***************************");
            Console.WriteLine("*    Test Get ShareName   *");
            Console.WriteLine("***************************");
            var ShareName = new ShareName();
            Console.WriteLine(ShareName.GetShareName("AC FP"));
            Console.WriteLine("**************Get ShareName****************");
            Console.ReadKey(true);
        }*/

    }

}
