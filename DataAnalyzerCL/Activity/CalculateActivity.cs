using System;
using System.Collections.Generic;
using DataAnalyzer.Entities;

namespace DataAnalyzer.Activity
{
    public class CalculateActivity
    {
        public static List<ActivityPerMinute> BeginAnalyzation(AcceptableActivityOptions option, 
            List<RowType> rows)
        {
            List<ActivityPerMinute> analyzedActivities;
            switch (option)
            {
                case AcceptableActivityOptions.EnergyExpenditure:
                    analyzedActivities = EnergyExpenditure.ProcessRows(rows);
                    break;

                case AcceptableActivityOptions.ZeroCrossing:
                    analyzedActivities = ZeroCrossing.ProcessRows(rows);
                    break;

                case AcceptableActivityOptions.ActivityIndex:
                    analyzedActivities = ActivityIndex.ProcessRows(rows);
                    break;

                case AcceptableActivityOptions.TAT:
                    analyzedActivities = TimeAboveThreshold.ProcessRows(rows);
                    break;

                case AcceptableActivityOptions.Int:
                    analyzedActivities = Integral.ProcessRows(rows);
                    break;

                case AcceptableActivityOptions.Corr:
                    Correlation.CalculateCorr(rows);
                    analyzedActivities = Integral.ProcessRows(rows); // Needed.
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }

            NormalizeActivities(analyzedActivities);
            return analyzedActivities;
        }

        public static void CorrelationBetweenDistanceAndActivity(List<ActivityPerMinute> distances,
            List<ActivityPerMinute> activities)
        {
            Correlation.CalculateCorr(distances, activities);
        }
        private static void NormalizeActivities(IEnumerable<ActivityPerMinute> activities)
        {
            foreach (var activity in activities)
            {
                activity.Normalize();
            }
        }
    }
}
