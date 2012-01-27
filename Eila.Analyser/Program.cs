using System;
using System.Linq;
using Eila.Framework;

namespace Eila.Analyser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1 || args[0] == "/?" || args[0] == "/help" || args[0] == "--help")
            {
                Console.WriteLine("Usage: LogsAnalyser [path] [daysBeforeToday]");
                Console.WriteLine("[path] - path where to store processed charts and csv files");
                Console.WriteLine("[daysBeforeToday] - logs will be processed of a day, that is [daysBeforeToday], for example 1 means yesterday. If none supplied, yesterday will be used.");
                return;
            }

            var parser = new IISLogParser();
            int dayCount = 1;
            var hasSpecifiedDayCount = args.Length == 2 && int.TryParse(args[1], out dayCount);
            var logSources = IISLogParser.GetLogSources().ToList();

            Console.WriteLine("Loaded log sources: {0}", string.Join(", ", logSources.Select(x => x.SiteName)));

            parser.RunAllQueriesForLogs(logSources, args[0], DateTime.Today.AddDays(-1 * (hasSpecifiedDayCount ? dayCount : 1)));

            Console.ReadKey();
        }
    }
}
