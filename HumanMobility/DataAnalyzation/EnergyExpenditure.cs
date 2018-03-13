using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyzer.Entities;
using HumanMobility.Models;

namespace HumanMobility.DataAnalyzation
{
    internal class EnergyExpenditure
    {
        public static List<ActivityPerMinute> ProcessRows(List<Activity> rows)
        {
            var activities = new List<ActivityPerMinute>();

            if (rows.Count == 0)
                return activities;

            var lastKnownTime = rows.First().SaveTime.AddMinutes(-5);
            ActivityPerMinute activity = null;

            for (int index = 0; index < rows.Count; index++)
            {
                var row = rows[index];
                var currentDate = new DateTime(row.SaveTime.Year, row.SaveTime.Month, row.SaveTime.Day,
                    row.SaveTime.Hour, row.SaveTime.Minute, 0);

                double resultant = Math.Sqrt(row.X * row.X + row.Y * row.Y + row.Z * row.Z);

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
            return activities;
        }
    }
}
