using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyzer.Activity
{
    public class StandardDeviation
    {
        public static double CalculateStdDev(List<double> values)
        {
            double ret = 0;
            if (values.Any())
            {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
    }
}
