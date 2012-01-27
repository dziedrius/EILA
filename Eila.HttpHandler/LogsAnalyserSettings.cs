using System.Configuration;

namespace Eila.HttpHandler
{
    public class LogsAnalyserSettings : ConfigurationSection
    {
        private static readonly LogsAnalyserSettings settings = ConfigurationManager.GetSection("LogsAnalyserSettings") as LogsAnalyserSettings;

        public static LogsAnalyserSettings Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("resultPath", IsRequired = true)]
        public string ResultPath
        {
            get { return (string)this["resultPath"]; }
            set { this["resultPath"] = value; }
        }

        [ConfigurationProperty("SitesToInclude", IsRequired = false)]
        public string SitesToInclude
        {
            get
            {
                if (Properties.Contains("sitesToInclude"))
                {
                    return (string)this["sitesToInclude"];
                }

                return null;
            }

            set
            {
                this["sitesToInclude"] = value;
            }
        }
    }
}