using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetNet.Models
{
    class Tools
    {
        public static DateTime AddBusinessDays(DateTime dt, int nDays)
        {
            int weeks = nDays / 5;
            nDays %= 5;
            while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                dt = dt.AddDays(1);

            while (nDays-- > 0)
            {
                dt = dt.AddDays(1);
                if (dt.DayOfWeek == DayOfWeek.Saturday)
                    dt = dt.AddDays(2);
            }
            return dt.AddDays(weeks * 7);
        }

        public static DateTime MinusBusinessDays(DateTime dt, int nDays)
        {
            int weeks = nDays / 5;
            nDays %= 5;
            while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                dt = dt.AddDays(-1);

            while (nDays-- > 0)
            {
                dt = dt.AddDays(-1);
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                    dt = dt.AddDays(-2);
            }
            return dt.AddDays(- weeks * 7);
        }

        public static DateTime NextBusinessDay(DateTime dt)
        {
            while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday) { dt = dt.AddDays(1); }
            return dt;
        }


        public static double[] minusArrays(double[] firstArray, double[] secondArray)
        {
            if (firstArray.Length != secondArray.Length) { throw new ArgumentOutOfRangeException("The arrays must have the same size"); }
            int size = firstArray.Length;
            double[] newArray = new double[size];
            for (int i = 0; i < size; i++)
            {
                newArray[i] = firstArray[i] - secondArray[i];
            }
            return newArray;
        }

        public static double productScalar(double[] firstArray, double[] secondArray)
        {
            if (firstArray.Length != secondArray.Length) { throw new ArgumentOutOfRangeException("The arrays must have the same size"); }
            int size = firstArray.Length;
            double value = 0;
            for (int i = 0; i < size; i++)
            {
                value += firstArray[i] * secondArray[i];
            }
            return value;
        }
    }
}
