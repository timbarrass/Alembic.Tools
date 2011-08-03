namespace HistogramTool
{
    public class LinearBucketingRule : IBucketingRule
    {
        public LinearBucketingRule()
        {
            BucketWidth = 3.14d;
        }

        public LinearBucketingRule(double bucketWidth)
        {
            BucketWidth = bucketWidth;
        }

        public int DetermineBucket(double value)
        {
            Guard.IsNotZero(BucketWidth, "BucketWidth", "Zero bucket width");

            var bucket = (int)(value / BucketWidth);

            return bucket;
        }

        public double DetermineValue(int bucket)
        {
            Guard.IsNotZero(BucketWidth, "BucketWidth", "Zero bucket width");

            var value = bucket * BucketWidth;

            return value;
        }

        public double BucketWidth { get; set; }
    }
}