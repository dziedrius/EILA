using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Eila.HttpHandler
{
    public class LogsAnalyserHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
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

            var sites = Directory.GetDirectories(resultPath);

            if (!string.IsNullOrEmpty(LogsAnalyserSettings.Settings.SitesToInclude))
            {
                var sitesToInclude = LogsAnalyserSettings.Settings.SitesToInclude.Split(";,".ToCharArray());
                sites = sites.Where(sitesToInclude.Contains).ToArray();
            }

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

            context.Response.Write("<table border='1'><tr><td style='vertical-align:top'>");
            WriteMenu(context, sites);
            context.Response.Write("</td><td>");           

            WriteAnalysis(context, resultPath, site, date);
            context.Response.Write("</td></tr></table>");

            context.Response.Write("<html><body>");
            
            context.Response.Write("</body></html>");
        }   

        private static void WriteMenu(HttpContext context, IEnumerable<string> sites)
        {
            context.Response.Write("<ul>");

            foreach (var siteDirectory in sites)
            {
                var siteName = Path.GetFileName(siteDirectory);
                context.Response.Write(string.Format("<li>{0}<ul>", siteName));

                foreach (var date in Directory.GetDirectories(siteDirectory))
                {
                    var dateName = Path.GetFileName(date);
                    context.Response.Write(string.Format(
                        "<li><a href='{1}?site={2}&date={0}'>{0}</a></li>",
                        dateName,
                        context.Request.Url.GetLeftPart(UriPartial.Path), 
                        siteName));
                }

                context.Response.Write("</ul></li>");
            }

            context.Response.Write("</ul>");
        }

        private void WriteAnalysis(HttpContext context, string resultPath, string site, string date)
        {
            context.Response.Write(string.Format("<h1>{0}, {1}</h1>", site, date));
            var combine = Path.Combine(resultPath, Path.Combine(site, date));

            if (!Directory.Exists(combine))
            {
                return;
            }

            context.Response.Write("<table>");

            foreach (var file in Directory.GetFiles(combine, "*.png"))
            {
                var separator = context.Request.Url.PathAndQuery.Contains('?') ? "&" : "?";
                context.Response.Write(string.Format("<tr><td><a href='{0}{2}image={1}'><img src='{0}{2}image={1}' style='max-width:300px' /></a></td>", context.Request.Url, Path.GetFileName(file), separator));
                context.Response.Write(string.Format("<td><a href='{0}{2}file={1}.csv'>{1}.csv</a></td></tr>", context.Request.Url, Path.GetFileNameWithoutExtension(file), separator));
            }

            context.Response.Write("</table>");
        }      
    }
}
