using System;
using System.Collections.Generic;

namespace HistogramTool
{
    public class Histogram
    {
        private IBucketingRule _bucketingRule;

        public Histogram() : this(new LinearBucketingRule())
        {
        }

        public Histogram(IBucketingRule bucketingRule)
        {
            _bucketingRule = bucketingRule;
        }

        public void Build(IList<double> values)
        {
            Guard.IsNotNull(values, "values", "Null values ref passed.");

            var bucketCount = _bucketingRule.DetermineBucketCount();
            Buckets = new int[bucketCount];

            foreach (var v in values)
            {
                if (_bucketingRule.IsHigh(v))   // ToDo: looks too fluent, perhaps misleadingly so
                {
                    High++;
                }
                else if (_bucketingRule.IsLow(v))
                {
                    Low++;
                }
                else
                {
                    var bucket = BucketingRule.DetermineBucket(v);
                    Buckets[bucket]++;
                }
            }
        }

        public int[] Buckets { get; set; }

        private IBucketingRule BucketingRule
        {
            get { return _bucketingRule; }
        }

        public int High { get; private set; }

        public int Low { get; private set; }
    }
}