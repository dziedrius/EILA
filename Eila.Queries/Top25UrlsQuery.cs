using Eila.Framework;
using Eila.Framework.Charts;

namespace Eila.Queries
{
    public class Top25UrlsQuery : QueryBase
    {
        public override string GetQueryText()
        {
            return string.Format(
                @"SELECT TOP 25  
                    cs-uri-stem as Url,  
                    COUNT(*) As Hits  
                INTO {0}.csv
                FROM {1}  
                GROUP BY cs-uri-stem  
                ORDER By Hits DESC",
                GetResultPath(),
                GetLogPath());
        }

        public override void GenerateChart()
        {
            ChartGenerator.GenerateChart(ChartParamterers.Default.SetAllValuesInXInterval(true), GetResultPath());
        }
    }
}