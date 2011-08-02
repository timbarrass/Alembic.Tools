namespace HistogramTool
{
    public class LinearBucketingRule : IBucketingRule
    {
        public int DetermineBucket(double value, double bucketWidth)
        {
            var bucket = (int)(value / bucketWidth);

            return bucket;
        }

        public double DetermineValue(int bucket, double bucketWidth)
        {
            var value = bucket * bucketWidth;

            return value;
        }
    }
}