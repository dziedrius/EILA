using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace Eila.Framework.Charts
{
    public class ChartGenerator
    {
        private static readonly Size ChartSize = new Size(1000, 1000);

        public static void GenerateChart(ChartParamterers parameters, string resultPath)
        {
            var csvPath = string.Format("{0}.csv", resultPath);          

            var records = ReadDataFromFile(csvPath);          

            var myChart = new Chart
            {
                Size = ChartSize
            };

            var myChartArea = new ChartArea();
            myChart.ChartAreas.Add(myChartArea);

            var series = new Series("default")
                             {
                                 ChartType = parameters.ChartType
                             };

            foreach (var thing in records)
            {
                series.Points.AddXY(thing.X.Length > 25 ? thing.X.Substring(0, 25) : thing.X, thing.Y);
            }

            myChart.Series.Add(series);

            if (parameters.AllValuesInXInterval)
            {
                myChart.ChartAreas[0].AxisX.Interval = 1;
            }

            var font = new Font("Arial", 12, FontStyle.Bold);
            var title = new Title
            {
                Text = resultPath,
                Font = font
            };

            myChart.Titles.Add(title);

            var pngPath = string.Format("{0}.png", resultPath);
            myChart.SaveImage(pngPath, ChartImageFormat.Png);
        }

        private static IEnumerable<XYRecord> ReadDataFromFile(string csvPath)
        {
            var csvlines = File.ReadAllLines(csvPath);
            var query = from csvline in csvlines
                        let data = csvline.Split(',')
                        select new XYRecord
                        {
                            X = data[0],
                            Y = data[1]
                        };

            return query.ToList();
        }
    }
}