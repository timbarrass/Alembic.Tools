namespace HistogramTool
{
    public interface IBucketingRule
    {
        int DetermineBucket(double value, double bucketWidth);

        double DetermineValue(int bucket, double bucketWidth);
    }
}