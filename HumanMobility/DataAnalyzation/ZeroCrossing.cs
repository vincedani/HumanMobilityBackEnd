using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyzer.Entities;
using HumanMobility.Models;

namespace HumanMobility.DataAnalyzation
{
    internal class ZeroCrossing
    {
        public static List<ActivityPerMinute> ProcessRows(List<Activity> rows)
        {
            var activities = new List<ActivityPerMinute>();

            if (rows.Count == 0)
                return activities;

            var lastKnownTime = rows.First().SaveTime.AddMinutes(-5);
            ActivityPerMinute activity = null;

            bool wasLastXPositive = rows.First().X > 0;
            bool wasLastYPositive = rows.First().Y > 0;
            bool wasLastZPositive = rows.First().Z > 0;

            for (int index = 0; index < rows.Count; index++)
            {
                var row = rows[index];
                var currentDate = new DateTime(row.SaveTime.Year, row.SaveTime.Month, row.SaveTime.Day,
                    row.SaveTime.Hour, row.SaveTime.Minute, 0);

                bool isXPositive = row.X > 0;
                bool isYPositive = row.Y > 0;
                bool isZPositive = row.Z > 0;

                double zcActivity = 0.0;

                if (wasLastXPositive != isXPositive)
                    zcActivity += 1.0;

                if(wasLastYPositive != isYPositive)
                    zcActivity += 1.0;

                if(wasLastZPositive != isZPositive)
                    zcActivity += 1.0;

                wasLastXPositive = isXPositive;
                wasLastYPositive = isYPositive;
                wasLastZPositive = isZPositive;

                if (!currentDate.Minute.Equals(lastKnownTime.Minute))
                {
                    if (activity != null)
                        activities.Add(activity);

                    activity = new ActivityPerMinute(currentDate, zcActivity);
                }
                else
                {
                    activity?.AddActivity(zcActivity);
                }

                lastKnownTime = currentDate;

                if (index == rows.Count - 1 && !activities.Contains(activity))
                    activities.Add(activity);
            }

            return activities;
        }
    }
}
