using System;

namespace HistogramTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new FileDataLoader();
            var rule = new LinearBucketingRule(10d);
            var histo = new Histogram(rule);

            var data = loader.LoadSingleValuedFile(args[0]);

            histo.Build(data);

            Display(rule, histo);
        }

        private static void Display(LinearBucketingRule rule, Histogram histogram)
        {
            for(int i = 0; i < histogram.Buckets.Length; i++)
            {
                Console.WriteLine(rule.DetermineValue(i) + "\t" + histogram.Buckets[i]);
            }
        }
    }
}
