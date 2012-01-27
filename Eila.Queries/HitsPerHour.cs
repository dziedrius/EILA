using Eila.Framework;
using Eila.Framework.Charts;

namespace Eila.Queries
{
    public class HitsPerHour : QueryBase
    {
        public override string GetQueryText()
        {
            return string.Format(
                @"SELECT  
                    QUANTIZE(TO_LOCALTIME(TO_TIMESTAMP(date, time)), 3600) AS Hour, 
                    COUNT(*) AS Hits  
                 INTO {0}.csv
                 FROM {1}                 
                 Group By Hour",
                GetResultPath(),
                GetLogPath());
        }

        public override void GenerateChart()
        {
            ChartGenerator.GenerateChart(ChartParamterers.Default.SetAllValuesInXInterval(true), GetResultPath());
        }
    }
}