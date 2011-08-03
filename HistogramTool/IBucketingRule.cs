namespace HistogramTool
{
    public interface IBucketingRule
    {
        long DetermineBucket(double value);

        long DetermineBucketCount();

        double DetermineValue(int bucket);

        double BucketWidth { get; set; }

        bool IsHigh(double value);

        bool IsLow(double value);
    }
}