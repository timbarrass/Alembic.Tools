using System;
using System.Linq;

namespace HistogramTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new FileDataLoader(args[0]);
            var data = loader.Load();

            var rule = new LinearBucketingRule(Convert.ToDouble(args[1]), 0d, data.Max());
            var histo = new Histogram(rule);

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
