using System;
using System.Collections.Generic;
using System.Linq;

namespace HistogramTool
{
    public class LinearBucketingRule : IBucketingRule
    {
        public LinearBucketingRule(double bucketWidth, IList<double> values)
        {
            BucketWidth = bucketWidth;
            Min = values.Min();
            Max = values.Max() + BucketWidth;
        }

        public LinearBucketingRule(double bucketWidth, double minimum, double maximum)
        {
            Min = minimum;
            Max = maximum;
            BucketWidth = bucketWidth;
        }

        public LinearBucketingRule(IList<double> values)
        {
            BucketWidth = (values.Max() - values.Min())/10d;
            Min = values.Min();
            Max = values.Max() + BucketWidth;
        }

        public double Min { get; set; }

        public double Max { get; set; }

        public long DetermineBucket(double value)
        {
            Guard.IsNotZero(BucketWidth, "BucketWidth", "Zero bucket width");

            var bucket = (long)((value - Min) / BucketWidth);

            return bucket;
        }

        public double DetermineValue(int bucket)
        {
            Guard.IsNotZero(BucketWidth, "BucketWidth", "Zero bucket width");

            var value = bucket * BucketWidth + Min;

            return value;
        }

        public double BucketWidth { get; set; }

        public bool IsHigh(double value)
        {
            if (value >= Max)
                return true;
            return false;
        }

        public bool IsLow(double value)
        {
            if (value < Min)
                return true;
            return false;
        }

        public long DetermineBucketCount()
        {
            double c = ((Max - Min)/BucketWidth);// +1L;
            if (c > long.MaxValue)
                throw new ArithmeticException("Your linear bucketing rule settings generate too many buckets");
            return (long)c;
        }
    }
}