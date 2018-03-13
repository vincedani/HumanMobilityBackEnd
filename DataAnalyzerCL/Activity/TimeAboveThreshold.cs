using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyzer.Entities;

namespace DataAnalyzer.Activity
{
    class TimeAboveThreshold
    {
        private const float Threshold = 0.1f;

        public static List<ActivityPerMinute> ProcessRows(List<RowType> rows)
        {
            var activities = new List<ActivityPerMinute>();
            var lastKnownTime = rows.First().DateTime.AddMinutes(-5);
            ActivityPerMinute activity = null;

            for (int index = 0; index < rows.Count; index++)
            {
                var row = rows[index];
                var currentDate = new DateTime(row.DateTime.Year, row.DateTime.Month, row.DateTime.Day,
                    row.DateTime.Hour, row.DateTime.Minute, 0);

                double resultant = Math.Abs(row.X) >= Threshold ? 1.0 : 0;
                resultant += Math.Abs(row.Y) >= Threshold ? 1.0 : 0;
                resultant += Math.Abs(row.Z) >= Threshold ? 1.0 : 0;

                if (!currentDate.Minute.Equals(lastKnownTime.Minute))
                {
                    if (activity != null)
                        activities.Add(activity);

                    activity = new ActivityPerMinute(currentDate, resultant);
                }
                else
                {
                    activity?.AddActivity(resultant);
                }

                lastKnownTime = currentDate;

                if (index == rows.Count - 1 && !activities.Contains(activity))
                    activities.Add(activity);
            }
            return activities.ToList();
        }
    }
}
