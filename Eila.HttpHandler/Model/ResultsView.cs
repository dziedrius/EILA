using System.Collections.Generic;

namespace Eila.HttpHandler.Model
{
    public class ResultsView
    {
        public List<SiteView> Sites { get; set; }
        public DateItemView DateItem { get; set; }
    }
}