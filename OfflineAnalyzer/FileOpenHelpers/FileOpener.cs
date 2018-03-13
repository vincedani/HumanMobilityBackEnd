using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DataAnalyzer.Activity;
using DataAnalyzer.Entities;
using HumanMobility.Models;
// ReSharper disable LocalizableElement

namespace OfflineAnalyzer.FileOpenHelpers
{
    class FileOpener
    {
        public static void BeginAnalyzation()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            string fileName = openFileDialog.FileName;
            Console.WriteLine(string.Concat("* Opened file: ", fileName));
            List<RowType> rows;
            List<LocationViewModel> locations;

            if (fileName.Contains(".csv") || fileName.Contains(".txt"))
            {
                rows = TextFileHandler.GetData(fileName);
                locations = new List<LocationViewModel>(0);
            }
            else
            {
                locations = DatabaseHandler.GetLocations(fileName);
                rows = DatabaseHandler.GetData(fileName, out string userName);
                Console.WriteLine(string.Concat("* Analyzed user: ", userName));
            }
            
            var beginDate = new DateTime(2017, 11, 17, 12, 0, 0);
            var endDate = new DateTime(2017, 11, 17, 14, 0, 0);

            rows = rows.Where(m => m.DateTime >= beginDate && m.DateTime < endDate).ToList();

            Console.WriteLine(string.Concat("* Preprocessed acc. rows: ", rows.Count));
            Console.WriteLine(string.Concat("* Preprocessed loc. rows: ", locations.Count));

            rows = rows.OrderBy(m => m.DateTime).ToList();
            //rows = rows.Where((elem, idx) => idx % 2 == 0).ToList();

            Console.WriteLine(
                "* Select a process option: Export | EnergyExpenditure | ZeroCrossing | StandardDeviation | | ActivityIndex | TAT | Int | ...");
            AcceptableActivityOptions option;

            while (!Enum.TryParse(Console.ReadLine(), true, out option))
                Console.WriteLine(
                    "* Select a process option from these: Export | EnergyExpenditure | ZeroCrossing | StandardDeviation | ActivityIndex | TAT | Int | ...");

            if (option != AcceptableActivityOptions.Export || option != AcceptableActivityOptions.Sync)
                RemoveGravity(rows);

            switch (option)
            {
                case AcceptableActivityOptions.Export:
                    Export.ExportText.ExportRows(rows);
                    break;

                case AcceptableActivityOptions.Sync:
                    Sync.SyncLocations.Syncronize(locations.OrderBy(m => m.SaveTime).ToList());
                    break;

                case AcceptableActivityOptions.StandardDeviation:
                    double devX = StandardDeviation.CalculateStdDev(rows.Select(m => (double)m.X).ToList());
                    double devY = StandardDeviation.CalculateStdDev(rows.Select(m => (double)m.Y).ToList());
                    double devZ = StandardDeviation.CalculateStdDev(rows.Select(m => (double)m.Z).ToList());
                    Export.ExportText.ExportStdDev(devX, devY, devZ);
                    break;

                case AcceptableActivityOptions.Dist:
                    var distFileDialog = new OpenFileDialog();
                    distFileDialog.ShowDialog();
                    string distFileName = distFileDialog.FileName;
                    Console.WriteLine(string.Concat("* Opened file: ", distFileName));
                    List<ActivityPerMinute> distances;
                    distances = DistFileHandler.GetData(distFileName);

                    distances = distances.Where(m => m.DateTime >= beginDate && m.DateTime < endDate).ToList();
                    var activities = CalculateActivity.BeginAnalyzation(AcceptableActivityOptions.TAT, rows);
                    CalculateActivity.CorrelationBetweenDistanceAndActivity(distances, activities);
                    //PostProcessTasks(distances);

                    break;
                default:
                    PostProcessTasks(CalculateActivity.BeginAnalyzation(option, rows));
                    break;
            }

            // ReSharper disable once RedundantAssignment
            rows = null;
            GC.Collect();
        }

        public static void RemoveGravity(List<RowType> rows)
        {
            var gravity = new float[3];
            const float alpha = (float) 0.7;

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

        private static void PostProcessTasks(List<ActivityPerMinute> activities)
        {
            Console.WriteLine(activities.First().Activity);
            Console.WriteLine("* Select an export option: text | chart | ...");

            AcceptableExportOptions option;
            while (!Enum.TryParse(Console.ReadLine(), true, out option))
                Console.WriteLine("* Select an option frome these: text | chart | ...");

            switch (option)
            {
                case AcceptableExportOptions.Chart:
                    Export.ExportChart.Export(activities);
                    break;

                case AcceptableExportOptions.Text:
                    Export.ExportText.Export(activities);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
