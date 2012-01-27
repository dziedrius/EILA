using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Web.Administration;
using MSUtil;

namespace Eila.Framework
{
    public class IISLogParser
    {
        private List<Type> queryTypes;

        public IISLogParser()
        {
            var skipQueriesParameter = ConfigurationManager.AppSettings["SkipQueries"];
            var skipQueries = new string[] { };
            if (!string.IsNullOrEmpty(skipQueriesParameter))
            {
                skipQueries = skipQueriesParameter.Split(":;".ToCharArray());
            }

            var assembly = Assembly.Load("Eila.Queries");

            queryTypes = assembly.GetTypes()
                .Where(x => typeof(QueryBase).IsAssignableFrom(x) && !skipQueries.Contains(x.Name))
                .ToList();

            Console.WriteLine("Loaded query types: {0}", string.Join(", ", queryTypes.Select(x => x.Name)));
        }     

        public virtual List<Type> QueryTypes
        {
            get { return queryTypes; }
            set { queryTypes = value; }
        }       

        public static IEnumerable<LogSource> GetLogSources()
        {
            var skipSiteNamesParameter = ConfigurationManager.AppSettings["SkipSiteNames"];
            var skipSiteNames = new string[] { };

            if (!string.IsNullOrEmpty(skipSiteNamesParameter))
            {
                skipSiteNames = skipSiteNamesParameter.Split(",;".ToCharArray());
            }

            using (var serverManager = new ServerManager())
            {
                return serverManager.Sites
                    .Where(x => !skipSiteNames.Contains(x.Name))
                    .Select(x => new LogSource
                                     {
                                         LogPath = Environment.ExpandEnvironmentVariables(Path.Combine(x.LogFile.Directory, string.Format("W3SVC{0}", x.Id))),
                                         SiteName = x.Name
                                     }).ToList();
            }         
        }

        public void AddAdditionalQueryTypes(List<Type> types)
        {
            queryTypes.AddRange(types);
        }

        public void ParseLog(string query)
        {
            var logParser = new LogQueryClassClass();
            var logContext = new COMW3CInputContextClassClass();
            var outputContext = new COMCSVOutputContextClassClass { oDQuotes = @"AUTO" };
            logParser.ExecuteBatch(query, logContext, outputContext);
        }      
                 
        public void RunAllQueriesForLogs(IEnumerable<LogSource> sources, string resultPath, DateTime date)
        {
            if (!Directory.Exists(resultPath))
            {
                Console.WriteLine("Creating directory for results: {0}", resultPath);
                Directory.CreateDirectory(resultPath);
            }

            foreach (var logSource in sources)
            {
                Console.WriteLine("Processing log source: {0}", logSource.SiteName);
                foreach (var queryType in queryTypes)
                {                    
                    var query = QueryBase.CreateInstance(queryType, date, logSource, resultPath);
                    if (!File.Exists(query.GetLogPath()))
                    {                        
                        continue;
                    }

                    ParseLog(query.GetQueryText());
                    query.GenerateChart();
                }
            }
        }
    }
}
