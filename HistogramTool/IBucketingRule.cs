namespace HistogramTool
{
    public interface IBucketingRule
    {
        int DetermineBucket(double value);

        double DetermineValue(int bucket);

        double BucketWidth { get; set; }
    }
}