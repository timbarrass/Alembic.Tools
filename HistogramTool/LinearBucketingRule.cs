using System;

namespace HistogramTool
{
    public class LinearBucketingRule : IBucketingRule
    {
        public LinearBucketingRule() : this(3.14d)
        {
        }

        public LinearBucketingRule(double bucketWidth) : this(bucketWidth, Double.MinValue, Double.MaxValue)
        {
        }

        public LinearBucketingRule(double bucketWidth, double minimum, double maximum)
        {
            Min = minimum;
            Max = maximum;
            BucketWidth = bucketWidth;
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
            double c = (Max / BucketWidth) + 1l;
            if (c > long.MaxValue)
                throw new ArithmeticException("Your linear bucketing rule settings generate too many buckets");
            return (long)c;
        }
    }
}