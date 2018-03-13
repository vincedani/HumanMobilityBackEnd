using HumanMobility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAnalyzer.Entities;
using HumanMobility.Models.Services;
using Chart = System.Web.Helpers.Chart;

namespace HumanMobility.Helpers
{
    public static class ReportHelper
    {
        #region Location

        public static string GetData(
            ResearchViewModel model, IService service, string requestedUserId, string exportedUserId)
        {
            service.LogExort(requestedUserId, exportedUserId, model.From, model.To);
            var locations = service.GetLocations(exportedUserId, model.From, model.To).ToList();

            List<Location> result;
            switch (model.ExportType)
            {
                case ExportType.CoherentData:
                    result = MakeCohorentResult(locations);
                    break;
                case ExportType.MergedData:
                    var cohorentData = MakeCohorentResult(locations);
                    result = MakeMergedResult(cohorentData, model.Fill);
                    break;
                case ExportType.Database:
                    result = locations;
                    break;
                default:
                    service.WriteBugReport(requestedUserId,
                        string.Concat("Search request failed. Undefined export type was used. ", DateTime.Now));
                    return null;
            }

            return MakeReport(result).ToString();
        }

        public static string  GetSummary(IService service, int ellapsedDays, DateTime from)
        {
            var builder = new StringBuilder();
            int samplesNeeded = ellapsedDays * 1440 + Convert.ToInt32(DateTime.Now.TimeOfDay.TotalMinutes);

            int toleranceLimit = Convert.ToInt32(samplesNeeded * 0.8);
            int minimumSamples = Convert.ToInt32(samplesNeeded * 0.5);
            var userNames = service.GetUserNames().OrderBy(s => s);

            foreach (string userName in userNames)
            {
                string userId = service.GetUserId(userName);
                var ienumerableData = service.GetLocations(userId, from, null).ToList();
                int count = ienumerableData.Count();

                if (count >= toleranceLimit) // 0.8 -
                {
                    builder.AppendFormat(
                        "{0} : {1} of {2} row, within the tolerance {3}",
                        userName,
                        count,
                        samplesNeeded,
                        Environment.NewLine);
                }
                else if (count < minimumSamples) // 0 - 0.5
                {
                    builder.AppendFormat(
                        "{0} : {1} of {2} row, out of tolerance {3}",
                        userName,
                        count,
                        samplesNeeded,
                        Environment.NewLine);
                }
                else // 0.5 - 0.8
                {
                    var lastKnownLocation = new Location();
                    bool isError = false;
                    bool sameTheTwoWall = true;

                    foreach (var location in ienumerableData)
                    {
                        if (!location.Error && location.Accuary < 100 &&
                            !DataManipulationHelper.IsTwoLocationSame(lastKnownLocation, location))
                        {
                            if (isError)
                                if (DataManipulationHelper.DistanceBetweenLocations(lastKnownLocation, location) > 500)
                                    sameTheTwoWall = false;

                            lastKnownLocation = location;
                            isError = false;
                        }
                        else
                            isError = true;
                    }

                    builder.AppendFormat(
                        sameTheTwoWall
                            ? "{0} : {1} of {2} row, can we fill it {3}"
                            : "{0} : {1} of {2} row, need to interpolate {3}",
                        userName,
                        count,
                        samplesNeeded,
                        Environment.NewLine);
                }
            }

            return builder.ToString();
        }

        private static StringBuilder MakeReport(List<Location> data)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Latitude;Longitude;Accuary;Error;DetectionTime;SaveTime;Flag;");
            var lastKnownLocation = data.FirstOrDefault();

            foreach (var location in data)
            {
                var flag = Flags.K;

                if (location.Error)
                    flag = Flags.E;

                if (location.Accuary > 100)
                    flag = Flags.U;

                if (DataManipulationHelper.IsTwoLocationSame(lastKnownLocation, location))
                    flag = Flags.F;

                int miss = (location.SaveTime - lastKnownLocation.SaveTime).Minutes - 1;

                if (miss >= 1)
                    flag = Flags.M;

                if (DataManipulationHelper.DistanceBetweenLocations(lastKnownLocation, location) > 500)
                    flag = Flags.J;

                string strFlag = flag +
                              (miss >= 1 ? miss.ToString() : string.Empty);

                builder.AppendFormat(
                    "{0};{1};{2};{3};{4:yyyy-MM-dd HH:mm:ss};{5:yyyy-MM-dd HH:mm};{6};{7}",
                    location.Latitude,
                    location.Longitude,
                    location.Accuary,
                    location.Error,
                    location.DetectionTime,
                    location.SaveTime,
                    strFlag,
                    Environment.NewLine);

                lastKnownLocation = location;
            }
            return builder;
        }

        private static List<Location> MakeCohorentResult(List<Location> data)
        {
            data.RemoveAll(m => m.Error || m.Accuary > 100);
            // TODO: Konzultálni Gergővel és Antival, hogy itt mit kellene kivenni még.
            return data;
        }

        private static List<Location> MakeMergedResult(List<Location> data, Fill fillType)
        {
            List<Location> result;
            switch (fillType)
            {
                case Fill.LastData:
                    result = DataManipulationHelper.FillErrorsWithLastData(data);
                    break;
                case Fill.LinearInterpolation:
                    result = DataManipulationHelper.FillErrorsWithLinearInterpolation(data);
                    break;
                default:
                    result = data;
                    break;
            }
            return result;
        }

        #endregion

        #region Accelerometer

        public static string GetAccelerometerData(List<Activity> activities)
        {
            var builder = new StringBuilder();
            builder.AppendLine("SaveTime;X;Y;Z;");

            foreach (var activity in activities)
            {
                builder.AppendFormat(
                    "{0:yyyy-MM-dd HH:mm:ss.fff};{1};{2};{3};{4}",
                    activity.SaveTime,
                    activity.X,
                    activity.Y,
                    activity.Z,
                    Environment.NewLine);
            }

            return builder.ToString();
        }

        private static string GetProcessedAccelerometerData(List<ActivityPerMinute> activities)
        {
            var builder = new StringBuilder();
            builder.AppendLine("DateTime;Activity");

            foreach (var activity in activities)
            {
                builder.AppendFormat("{0:yyyy-MM-dd HH:mm};{1}{2}",
                    activity.DateTime, activity.Activity, Environment.NewLine);
            }

            return builder.ToString();
        }

        public static string ExportProcessedData(List<ActivityPerMinute> data)
        {
            return GetProcessedAccelerometerData(data);
        }

        public static string ExportStdDev(double devX, double devY, double devZ)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Standard Deviation");
            builder.AppendFormat("stdDev X: {0}{1}", devX, Environment.NewLine);
            builder.AppendFormat("stdDev Y: {0}{1}", devY, Environment.NewLine);
            builder.AppendFormat("stdDev Z: {0}{1}", devZ, Environment.NewLine);

            return builder.ToString();
        }

        public static Chart CreateChart(List<ActivityPerMinute> data)
        {
            var chart = new Chart(width: 1920, height: 1080)
                .AddTitle("Activity")
                .AddSeries(
                    name: "Accelerometer",
                    chartType: "Column",
                    xValue: data.Select(m => m.DateTime).ToList(),
                    yValues: data.Select(m => m.Activity).ToList()
                )
                .Write();
            return chart;
        }

        #endregion
    }
}
