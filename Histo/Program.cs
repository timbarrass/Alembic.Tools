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
            var bucketWidthSet = false;
            var low = 0d;
            var lowSet = false;
            var high = 100d;
            var highSet = false;

            OptionSet p = new OptionSet()
                .Add("file=", f => fileName = f)
                .Add("bucketWidth=|w=", w =>
                                            {
                                                bucketWidth = Convert.ToDouble(w);
                                                bucketWidthSet = true;
                                            })
                .Add("low=|l=", l =>
                                    {
                                        low = Convert.ToDouble(l);
                                        lowSet = true;
                                    })
                .Add("high=|h=", h =>
                                     {
                                         high = Convert.ToDouble(h);
                                         highSet = true;
                                     });
            var unparsed = p.Parse(args);

            Guard.IsLessThan(high, low, "high", "low", "Your low limit must be less than your high limit.");
            if ((lowSet && !highSet) || (highSet && !lowSet))
            {
                throw new ArgumentException("You must set both of low and high if you set one.");
            }

            var loader = new FileDataLoader(fileName);
            var data = loader.Load();
            var rule = new LinearBucketingRule(data);
            if(! lowSet && ! highSet)
            {
                rule = new LinearBucketingRule(bucketWidth, data);
            }
            else if(lowSet && highSet)
            {
                rule = new LinearBucketingRule(bucketWidth, low, high);
            }

            var histo = new Histogram(rule);

            histo.Build(data);

            Display(rule, histo);
        }

        private static void Display(LinearBucketingRule rule, Histogram histogram)
        {
            var total = 0;
            Console.WriteLine("Low (< " + rule.Min + ") \t" + histogram.Low);
            for(int i = 0; i < histogram.Buckets.Length; i++)
            {
                Console.WriteLine(rule.DetermineValue(i) + "\t" + histogram.Buckets[i]);
                total += histogram.Buckets[i];
            }
            Console.WriteLine("High (>= " + rule.Max + ") \t" + histogram.High);
            total += histogram.Low + histogram.High;
            Console.WriteLine("Total\t" + total);
        }
    }
}
