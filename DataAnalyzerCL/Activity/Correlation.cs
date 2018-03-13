using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyzer.Entities;

namespace DataAnalyzer.Activity
{
    class Correlation
    {
        public static void CalculateCorr(List<RowType> rows)
        {
            var zcm = ZeroCrossing.ProcessRows(rows);
            var tat = TimeAboveThreshold.ProcessRows(rows);
            var integral = Integral.ProcessRows(rows);
            var ai = ActivityIndex.ProcessRows(rows);

            double corrZcmTat = MathNet.Numerics.Statistics.Correlation.Pearson(zcm.Select(m => m.Activity), tat.Select(m => m.Activity));
            double corrZcmInt = MathNet.Numerics.Statistics.Correlation.Pearson(zcm.Select(m => m.Activity), integral.Select(m => m.Activity));
            double corrZcmAi = MathNet.Numerics.Statistics.Correlation.Pearson(zcm.Select(m => m.Activity), ai.Select(m => m.Activity));

            Console.WriteLine("ZCM => TAT: " + corrZcmTat);
            Console.WriteLine("ZCM => Int: " + corrZcmInt);
            Console.WriteLine("ZCM => AI: " + corrZcmAi);


            double corrTatZcm = MathNet.Numerics.Statistics.Correlation.Pearson(tat.Select(m => m.Activity), zcm.Select(m => m.Activity));
            double corrTatInt = MathNet.Numerics.Statistics.Correlation.Pearson(tat.Select(m => m.Activity), integral.Select(m => m.Activity));
            double corrTatAi = MathNet.Numerics.Statistics.Correlation.Pearson(tat.Select(m => m.Activity), ai.Select(m => m.Activity));

            Console.WriteLine("TAT => ZCM: " + corrTatZcm);
            Console.WriteLine("TAT => Int: " + corrTatInt);
            Console.WriteLine("TAT => AI: " + corrTatAi);


            double corrIntZcm = MathNet.Numerics.Statistics.Correlation.Pearson(integral.Select(m => m.Activity), zcm.Select(m => m.Activity));
            double corrIntTat = MathNet.Numerics.Statistics.Correlation.Pearson(integral.Select(m => m.Activity), tat.Select(m => m.Activity));
            double corrIntAi = MathNet.Numerics.Statistics.Correlation.Pearson(integral.Select(m => m.Activity), ai.Select(m => m.Activity));

            Console.WriteLine("Int => ZCM: " + corrIntZcm);
            Console.WriteLine("Int => TAT: " + corrIntTat);
            Console.WriteLine("Int => AI: " + corrIntAi);

            double corrAiZcm = MathNet.Numerics.Statistics.Correlation.Pearson(ai.Select(m => m.Activity), zcm.Select(m => m.Activity));
            double corrAiTat = MathNet.Numerics.Statistics.Correlation.Pearson(ai.Select(m => m.Activity), tat.Select(m => m.Activity));
            double corrAiInt = MathNet.Numerics.Statistics.Correlation.Pearson(ai.Select(m => m.Activity), integral.Select(m => m.Activity));

            Console.WriteLine("AI => ZCM: " + corrAiZcm);
            Console.WriteLine("AI => TAT: " + corrAiTat);
            Console.WriteLine("AI => Int: " + corrAiInt);
        }

        public static void CalculateCorr(List<ActivityPerMinute> distances, List<ActivityPerMinute> activities)
        {
            distances = distances.OrderBy(m => m.DateTime).ToList();
            activities = activities.OrderBy(m => m.DateTime).ToList();

            foreach (var distance in distances)
            {
                if (!activities.Any(m => m.DateTime == distance.DateTime))
                {
                    activities.Add(new ActivityPerMinute(distance.DateTime, 0));
                }
            }

            double correlation = MathNet.Numerics.Statistics.Correlation.Pearson(
                distances.Select(m => m.Activity),
                activities.Select(m => m.Activity));

            Console.WriteLine("Dist => Act: " + correlation);
        }
    }
}
