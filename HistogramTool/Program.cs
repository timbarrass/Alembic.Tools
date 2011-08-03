namespace HistogramTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new FileDataLoader();
            var rule = new LinearBucketingRule(10d);
            var histo = new Histogram(rule);

            var data = loader.LoadSingleValuedFile("basicHistoData.txt");

            histo.Build(data);
        }
    }
}
