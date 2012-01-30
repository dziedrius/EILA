using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Eila.HttpHandler.Model;
using Eila.HttpHandler.Properties;
using Nustache.Core;

namespace Eila.HttpHandler
{
    public class LogsAnalyserHandler : IHttpHandler
    {
        static LogsAnalyserHandler()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;            
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var resultPath = ValidateConfiguration();
            var sites = Directory.GetDirectories(resultPath);
            sites = FilterSites(sites);

            var site = Path.GetFileName(sites.First());

            if (context.Request.Params.AllKeys.Contains("site"))
            {
                site = context.Request.Params["site"];
            }

            var date = string.Format("{0:yyMMdd}", DateTime.Now.AddDays(-1));

            if (context.Request.Params.AllKeys.Contains("date"))
            {
                date = context.Request.Params["date"];
            }

            var path = string.Format(@"{0}\{1}\{2}", resultPath, site, date);

            if (context.Request.Params.AllKeys.Contains("image"))
            {
                context.Response.ContentType = "image/png";
                byte[] fileBytes = File.ReadAllBytes(Path.Combine(path, context.Request.Params["image"]));
                context.Response.BinaryWrite(fileBytes);
                return;
            }

            if (context.Request.Params.AllKeys.Contains("file"))
            {
                context.Response.ContentType = "text/csv";
                var fileName = context.Request.Params["file"];
                byte[] fileBytes = File.ReadAllBytes(Path.Combine(path, fileName));
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.csv", fileName));
                context.Response.BinaryWrite(fileBytes);
                return;
            }

            var template = Resources.template;

            var resultsView = new ResultsView
                                  {
                                      Sites = GetSitesMenu(sites, context.Request.Url),
                                      DateItem = GetDateItem(resultPath, site, date, context.Request.Url)
                                  };

            var html = Render.StringToString(template, resultsView);
            context.Response.Write(html);         
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var resourceName = "Eila.HttpHandler." + new AssemblyName(args.Name).Name + ".dll";

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                var assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);

                return Assembly.Load(assemblyData);
            }
        }

        private static string[] FilterSites(string[] sites)
        {
            if (!string.IsNullOrEmpty(LogsAnalyserSettings.Settings.SitesToInclude))
            {
                var sitesToInclude = LogsAnalyserSettings.Settings.SitesToInclude.Split(";,".ToCharArray());
                sites = sites.Where(sitesToInclude.Contains).ToArray();
            }
            return sites;
        }

        private static string ValidateConfiguration()
        {
            if (LogsAnalyserSettings.Settings == null)
            {
                throw new ConfigurationErrorsException("Missing LogsAnalyserSettings config section!");
            }

            var resultPath = LogsAnalyserSettings.Settings.ResultPath;
            if (!Directory.Exists(resultPath))
            {
                throw new ConfigurationErrorsException(string.Format("{0} path does not exist.", resultPath));
            }
            return resultPath;
        }

        private DateItemView GetDateItem(string resultPath, string site, string date, Uri requestUrl)
        {
            var dateItemView = new DateItemView
                                   {
                                       Site = site, 
                                       Date = date
                                   };


            var combinedPath = Path.Combine(resultPath, Path.Combine(site, date));

            if (!Directory.Exists(combinedPath))
            {
                return dateItemView;
            }

            var separator = requestUrl.PathAndQuery.Contains('?') ? "&" : "?";

            dateItemView.QueryViews = Directory.GetFiles(combinedPath, "*.png").Select(file => new QueryView
                                                                                              {
                                                                                                  QueryName = Path.GetFileNameWithoutExtension(file),
                                                                                                  ImageFilePath = string.Format("{0}{2}image={1}", requestUrl, Path.GetFileName(file), separator),
                                                                                                  CsvFilePath = string.Format("{0}{2}file={1}.csv", requestUrl, Path.GetFileNameWithoutExtension(file), separator)
                                                                                              }).ToList();

            return dateItemView;
        }

        private List<SiteView> GetSitesMenu(IEnumerable<string> sites, Uri requestUrl)
        {
            var list = new List<SiteView>();
            foreach (var siteDirectory in sites)
            {
                var siteName = Path.GetFileName(siteDirectory);
                Func<string, DateView> selector = date =>
                                    {
                                        var dateName = Path.GetFileName(date);
                                        var link = string.Format("{1}?site={2}&date={0}", dateName, requestUrl.GetLeftPart(UriPartial.Path), siteName);
                                        return new DateView
                                                   {
                                                       Name = dateName, Link = link
                                                   };
                                    };

                var siteView = new SiteView
                                   {
                                       SiteName = siteName,
                                       Dates = Directory.GetDirectories(siteDirectory)
                                           .Select(selector)
                                           .ToList()
                                   };

                list.Add(siteView);
            }

            return list;
        }      
    }
}
