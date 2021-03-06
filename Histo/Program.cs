﻿using System;
using System.Linq;

using Mono.Options;

namespace HistogramTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: histo -file=<filename> [-w=<bucket width>] [-l=<low value> -h=<high value>]");
                return;
            }

            var fileName = string.Empty;
            var bucketWidth = 10d;
            var bucketWidthSet = false;
            var low = 0d;
            var lowSet = false;
            var high = 100d;
            var highSet = false;
            var cumulative = false;

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
                                     })
                .Add("cumulative|c", c => { cumulative = true; });
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

            Display(rule, histo, cumulative);
        }

        private static void Display(LinearBucketingRule rule, Histogram histogram, bool cumulative)
        {
            var total = 0;
            Console.WriteLine("Low (< " + rule.Min + ") \t" + histogram.Low);
            for(int i = 0; i < histogram.Buckets.Length; i++)
            {
                total += histogram.Buckets[i];
                if (cumulative)
                {
                    Console.WriteLine(rule.DetermineValue(i) + "\t" + total);
                }
                else
                {
                    Console.WriteLine(rule.DetermineValue(i) + "\t" + histogram.Buckets[i]);
                }

            }
            Console.WriteLine("High (>= " + rule.Max + ") \t" + histogram.High);
            total += histogram.Low + histogram.High;
            Console.WriteLine("Total\t" + total);
        }
    }
}
