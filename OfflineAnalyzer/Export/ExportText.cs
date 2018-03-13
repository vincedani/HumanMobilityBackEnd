using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DataAnalyzer.Entities;

namespace OfflineAnalyzer.Export
{
    class ExportText
    {
        public static void Export(List<ActivityPerMinute> activities)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save analyzed data",
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "Text Files (*.txt)|*.txt",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK || saveFileDialog.FileName == string.Empty)
                return;

            string fileName = saveFileDialog.FileName;
            string report = GetReport(activities);

            using (var file = File.CreateText(fileName))
            {
                file.Write(report);
            }
        }

        private static string GetReport(IEnumerable<ActivityPerMinute> activities)
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

        public static void ExportRows(List<RowType> rows)
        {
            //rows = rows.OrderBy(m => m.DateTime).ThenBy(m => m.DateTime.Millisecond).ToList();

            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save raw data",
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "Text Files (*.txt)|*.txt",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK || saveFileDialog.FileName == string.Empty)
                return;

            string fileName = saveFileDialog.FileName;

            var builder = new StringBuilder();
            builder.AppendLine("DateTime;X;Y;Z");

            foreach (var activity in rows)
            {
                builder.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff};{1};{2};{3}{4}",
                    activity.DateTime, activity.X, activity.Y, activity.Z, Environment.NewLine);
            }

            string report = builder.ToString();

            using (var file = File.CreateText(fileName))
            {
                file.Write(report);
            }
        }

        public static void ExportStdDev(double X, double Y, double Z)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save stdDev",
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "Text Files (*.txt)|*.txt",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK || saveFileDialog.FileName == string.Empty)
                return;

            string fileName = saveFileDialog.FileName;

            var builder = new StringBuilder();
            builder.AppendLine("Standard Deviation");
            builder.AppendFormat("stdDev X: {0}{1}", X, Environment.NewLine);
            builder.AppendFormat("stdDev Y: {0}{1}", Y, Environment.NewLine);
            builder.AppendFormat("stdDev Z: {0}{1}", Z, Environment.NewLine);

            string report = builder.ToString();

            using (var file = File.CreateText(fileName))
            {
                file.Write(report);
            }
        }
    }
}
