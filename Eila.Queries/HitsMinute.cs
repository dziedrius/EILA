using System.Windows.Forms.DataVisualization.Charting;
using Eila.Framework;
using Eila.Framework.Charts;

namespace Eila.Queries
{
    public class HitsMinute : QueryBase
    {
        public override string GetQueryText()
        {
            return string.Format(
                @"SELECT  
                    QUANTIZE(TO_LOCALTIME(TO_TIMESTAMP(date, time)), 60) AS Minute, 
                    COUNT(*) AS Hits  
                 INTO {0}.csv
                 FROM {1}                 
                 Group By Minute",
                GetResultPath(),
                GetLogPath());
        }

        public override void GenerateChart()
        {
            ChartGenerator.GenerateChart(ChartParamterers.Default.SetChartType(SeriesChartType.Spline), GetResultPath());
        }
    }
}