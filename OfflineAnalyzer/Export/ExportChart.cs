using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DataAnalyzer.Entities;
using System.Windows.Forms.DataVisualization.Charting;

namespace OfflineAnalyzer.Export
{
    class ExportChart
    {
        public static void Export(List<ActivityPerMinute> activities)
        {
            var chart = new Chart();
            var chartArea = new ChartArea();
            chart.Size = new Size(600, 600);


            chartArea.AxisX.LabelStyle.Format = "HH:mm";
            chartArea.AxisX.LabelAutoFitMinFontSize = 17;
            chartArea.AxisY.LabelAutoFitMinFontSize = 17;
            chartArea.AxisX.Title = "Idő [ms]";
            chartArea.AxisY.Title = "Aktivitás";
            chartArea.AxisX.TitleFont = new Font("Arial", 17, FontStyle.Regular);
            chartArea.AxisY.TitleFont = new Font("Arial", 17, FontStyle.Regular);
            chartArea.AxisX.MajorGrid.LineColor = Color.DimGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.DimGray;

            chart.ChartAreas.Add(chartArea);
            var series = new Series
            {
                Name = "Accelerometer",
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.DateTime,
                YValueType = ChartValueType.Double,
                //Color = Color.Red
            };

            foreach (var activity in activities)
            {
                series.Points.AddXY(activity.DateTime, activity.Activity);
            }

            chart.Series.Add(series);
            chart.Invalidate();

            var saveFileDialog = new SaveFileDialog
            {
                FileName = "default.png",
                Filter = "PNG files (*.png)|*.png"
            };

            if(saveFileDialog.ShowDialog() == DialogResult.OK)
                chart.SaveImage(saveFileDialog.FileName, ChartImageFormat.Png);
        }
    }
}
