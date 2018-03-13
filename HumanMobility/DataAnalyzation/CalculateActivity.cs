using System;
using System.Collections.Generic;
using DataAnalyzer.Entities;
using HumanMobility.Models;

namespace HumanMobility.DataAnalyzation
{
    public class CalculateActivity
    {
        public static List<ActivityPerMinute> BeginAnalyzation(AcceptableActivityOptions option, 
            List<Activity> rows)
        {
            RemoveGravity(rows);
            List<ActivityPerMinute> analyzedActivities;
            switch (option)
            {
                case AcceptableActivityOptions.EnergyExpenditure:
                    analyzedActivities = EnergyExpenditure.ProcessRows(rows);
                    break;
                case AcceptableActivityOptions.ZeroCrossing:
                    analyzedActivities = ZeroCrossing.ProcessRows(rows);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }

            NormalizeActivities(analyzedActivities);
            return analyzedActivities;
        }

        private static void NormalizeActivities(IEnumerable<ActivityPerMinute> activities)
        {
            foreach (var activity in activities)
            {
                activity.Normalize();
            }
        }

        private static void RemoveGravity(List<Activity> rows)
        {
            var gravity = new float[3];
            const float alpha = (float)0.8;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < rows.Count; i++)
            {
                // Isolate the force of gravity with the low-pass filter.
                gravity[0] = alpha * gravity[0] + (1 - alpha) * rows[i].X;
                gravity[1] = alpha * gravity[1] + (1 - alpha) * rows[i].Y;
                gravity[2] = alpha * gravity[2] + (1 - alpha) * rows[i].Z;

                // Remove the gravity contribution with the high-pass filter.
                rows[i].X -= gravity[0];
                rows[i].Y -= gravity[1];
                rows[i].Z -= gravity[2];
            }
        }
    }
}
