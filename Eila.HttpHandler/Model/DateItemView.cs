using System.Collections.Generic;

namespace Eila.HttpHandler.Model
{
    public class DateItemView
    {
        public string Date { get; set; }
        public string Site { get; set; }
        public List<QueryView> QueryViews { get; set; }
    }
}