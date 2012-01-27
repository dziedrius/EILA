using System.Windows.Forms.DataVisualization.Charting;

namespace Eila.Framework.Charts
{
    public class ChartParamterers
    {
        private bool allValuesInXInterval;

        private SeriesChartType chartType;

        public static ChartParamterers Default
        {
            get
            {
                return new ChartParamterers
                {
                    allValuesInXInterval = false,
                    chartType = SeriesChartType.Bar
                };
            }
        }

        public SeriesChartType ChartType
        {
            get { return chartType; }            
        }      

        public bool AllValuesInXInterval
        {
            get
            {
                return allValuesInXInterval;
            }
        }

        public ChartParamterers SetAllValuesInXInterval(bool value)
        {
            allValuesInXInterval = value;
            return this;
        }

        public ChartParamterers SetChartType(SeriesChartType type)
        {
            chartType = type;
            return this;
        }
    }
}