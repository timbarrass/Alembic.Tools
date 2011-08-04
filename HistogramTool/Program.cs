using System;
using System.Linq;

using Mono.Options;

namespace HistogramTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = string.Empty;
            var bucketWidth = 10d;

            OptionSet p = new OptionSet()
                .Add("file=", f => fileName = f)
                .Add("bucketWidth=|w=", w => bucketWidth = Convert.ToDouble(w));
            var unparsed = p.Parse(args);
            Console.WriteLine("Processing " + fileName);

            var loader = new FileDataLoader(fileName);
            var data = loader.Load();

            var rule = new LinearBucketingRule(bucketWidth, 0d, data.Max());
            var histo = new Histogram(rule);

            histo.Build(data);

            Display(rule, histo);
        }

        private static void Display(LinearBucketingRule rule, Histogram histogram)
        {
            Console.WriteLine("Low\t" + histogram.Low);
            for(int i = 0; i < histogram.Buckets.Length; i++)
            {
                Console.WriteLine(rule.DetermineValue(i) + "\t" + histogram.Buckets[i]);
            }
            Console.WriteLine("High\t" + histogram.High);
        }
    }
}
