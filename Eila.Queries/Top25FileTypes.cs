using Eila.Framework;

namespace Eila.Queries
{
    public class Top25FileTypes : QueryBase
    {
        public override string GetQueryText()
        {
            return string.Format(
                @"SELECT TOP 25  
                    EXTRACT_EXTENSION(cs-uri-stem) As Extension,  
                    COUNT(*) As Hits  
                 INTO {0}.csv
                 FROM {1}
                 GROUP BY Extension  
                 ORDER BY Hits DESC",
                GetResultPath(),
                GetLogPath());
        }      
    }
}