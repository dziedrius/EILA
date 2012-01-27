using Eila.Framework;

namespace Eila.Queries
{
    public class ResponseTimesByUrl : QueryBase
    {
        public override string GetQueryText()
        {
            return string.Format(
                @"SELECT 
                    QUANTIZE(time-taken, 25) AS t, 
                    cs-uri-stem AS Url,
                    COUNT(*) as count
                INTO {0}.csv
                FROM {1}
                GROUP BY t, Url
                ORDER BY t
                ",
                GetResultPath(),
                GetLogPath());
        }

        public override void GenerateChart()
        {            
        }
    }
}