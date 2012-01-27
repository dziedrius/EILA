using System;
using Eila.Framework.Charts;

namespace Eila.Framework
{
    public abstract class QueryBase
    {
        private DateTime date;
        private LogSource source;
        private string resultPath;        

        private string logFileNameFormat = @"{0}\u_ex{1:yyMMdd}.log";
        private string resultFileNameWithoutExtensionFormatWithoutExtension = @"{0}\{1}\{3:yyMMdd}\{2}";

        public string LogFileNameFormat
        {
            get { return logFileNameFormat; }
            set { logFileNameFormat = value; }
        }
        
        public string ResultFileNameFormatWithoutExtension
        {
            get { return resultFileNameWithoutExtensionFormatWithoutExtension; }
            set { resultFileNameWithoutExtensionFormatWithoutExtension = value; }
        }

        public static QueryBase CreateInstance(Type type, DateTime date, LogSource source, string resultPath)
        {
            var instance = (QueryBase)Activator.CreateInstance(type);
            instance.Initialize(date, source, resultPath);
            return instance;
        }

        public abstract string GetQueryText();

        public virtual void GenerateChart()
        {
            ChartGenerator.GenerateChart(ChartParamterers.Default, GetResultPath());
        }

        public virtual string GetLogPath()
        {
            return string.Format(logFileNameFormat, source.LogPath, date);
        }

        protected virtual void Initialize(DateTime date, LogSource source, string resultPath)
        {
            this.date = date;
            this.source = source;
            this.resultPath = resultPath;
        }            

        protected virtual string GetResultPath()
        {
            return string.Format(ResultFileNameFormatWithoutExtension, resultPath, source.SiteName, GetType().Name, date);
        }
    }
}