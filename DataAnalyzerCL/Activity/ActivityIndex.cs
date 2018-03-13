using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyzer.Entities;

namespace DataAnalyzer.Activity
{
    class ActivityIndex
    {
        public static List<ActivityPerMinute> ProcessRows(List<RowType> rows)
        {
            var activities = new List<ActivityPerMinute>();

            double devX = StandardDeviation.CalculateStdDev(rows.Select(m => (double)m.X).ToList());
            double devY = StandardDeviation.CalculateStdDev(rows.Select(m => (double)m.Y).ToList());
            double devZ = StandardDeviation.CalculateStdDev(rows.Select(m => (double)m.Z).ToList());
            double sigma = devX + devY + devZ;

            var cmRows = new List<RowType>(300);
            int currentMinute = rows.First().DateTime.Minute;

            for (int index = 0; index < rows.Count; index++)
            {
                var row = rows[index];
                if (row.DateTime.Minute != currentMinute || index == rows.Count - 1)
                {
                    currentMinute = row.DateTime.Minute;
                    double localDevX = StandardDeviation.CalculateStdDev(cmRows.Select(m => (double) m.X).ToList());
                    double localDevY = StandardDeviation.CalculateStdDev(cmRows.Select(m => (double) m.Y).ToList());
                    double localDevZ = StandardDeviation.CalculateStdDev(cmRows.Select(m => (double) m.Z).ToList());
                    double localSigma = localDevX + localDevY + localDevZ;

                    double brackets = (localSigma - sigma) / 3.0;
                    double max = Math.Max(brackets, 0);
                    double activityIndex = Math.Sqrt(max);

                    var tmp = cmRows.First();
                    var currentDate = new DateTime(tmp.DateTime.Year, tmp.DateTime.Month, tmp.DateTime.Day,
                        tmp.DateTime.Hour, tmp.DateTime.Minute, 0);

                    activities.Add(new ActivityPerMinute(currentDate, activityIndex));
                    cmRows.Clear();
                }
                cmRows.Add(row);
            }

            return activities;
        }
    }
}
