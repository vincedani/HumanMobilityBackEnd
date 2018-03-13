using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyzer.Entities;

namespace DataAnalyzer.Activity
{
    internal class ZeroCrossing
    {
        private const float Threshold = 0.1f;

        public static List<ActivityPerMinute> ProcessRows(List<RowType> rows)
        {
            var activities = new List<ActivityPerMinute>();
            var lastKnownTime = rows.First().DateTime.AddMinutes(-5);
            ActivityPerMinute activity = null;

            bool wasLastXPositive = rows.First().X > 0 && Math.Abs(rows.First().X) >= Threshold;
            bool wasLastYPositive = rows.First().Y > 0 && Math.Abs(rows.First().Y) >= Threshold;
            bool wasLastZPositive = rows.First().Z > 0 && Math.Abs(rows.First().Z) >= Threshold;

            for (int index = 0; index < rows.Count; index++)
            {
                var row = rows[index];
                var currentDate = new DateTime(row.DateTime.Year, row.DateTime.Month, row.DateTime.Day,
                    row.DateTime.Hour, row.DateTime.Minute, 0);

                bool isXPositive = row.X > 0 && Math.Abs(row.X) >= Threshold;
                bool isYPositive = row.Y > 0 && Math.Abs(row.Y) >= Threshold;
                bool isZPositive = row.Z > 0 && Math.Abs(row.Z) >= Threshold;

                double zcActivity = 0.0;

                if (wasLastXPositive != isXPositive)
                    zcActivity += 1.0;

                if (wasLastYPositive != isYPositive)
                    zcActivity += 1.0;

                if (wasLastZPositive != isZPositive)
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
