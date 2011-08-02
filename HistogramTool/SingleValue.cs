using FileHelpers;

namespace HistogramTool
{
    [DelimitedRecord(",")]
    public class SingleValue
    {
        public double Value { get; set; }
    }
}