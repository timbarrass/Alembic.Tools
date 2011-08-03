using System;
using System.Collections.Generic;
using System.Linq;

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

            var bucketCount = (int)(values.Max() / BucketingRule.BucketWidth) + 1;
            Buckets = new int[bucketCount];

            foreach (var v in values)
            {
                var bucket = BucketingRule.DetermineBucket(v);
                Buckets[bucket]++;
            }
        }

        public int[] Buckets { get; set; }

        private IBucketingRule BucketingRule
        {
            get { return _bucketingRule; }
        }
    }
}