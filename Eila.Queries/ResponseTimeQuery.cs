using Eila.Framework;

namespace Eila.Queries
{
    public class ResponseTimeQuery : QueryBase
    {
        public override string GetQueryText()
        {
            return string.Format(
                @"SELECT 
                    QUANTIZE(time-taken, 25) AS t, 
                    COUNT(*) as count
                INTO {0}.csv
                FROM {1}
                GROUP BY t
                ORDER BY t
                ",
                GetResultPath(),
                GetLogPath());
        }        
    }
}