using System.Collections.Generic;
using System.Linq;

namespace HistogramTool
{
    public class Histogram
    {
        private IList<double> _values;
        private double _bucketWidth;
        private int[] _buckets;
        private IHistogramDataLoader _dataLoader;
        private IBucketingRule _bucketingRule;

        public Histogram() : this(new FileDataLoader(), new LinearBucketingRule())
        {
        }

        public Histogram(IHistogramDataLoader dataLoader, IBucketingRule bucketingRule)
        {
            _dataLoader = dataLoader;
            _bucketingRule = bucketingRule;
        }

        public void LoadSingleValuedFile(string fileName)
        {
            _values = DataLoader.LoadSingleValuedFile(fileName);
        }

        public int DataCount
        {
            get
            {
                Guard.IsNotNull(_values, "values", "No histogram data has been loaded yet.");
         
                return _values.Count;
            }
        }

        public void Build()
        {
            Guard.IsNotNull(_values, "values", "No histogram data has been loaded yet.");
            Guard.IsNotZero(BucketWidth, "BucketWidth", "Zero bucket width");

            Buckets = new int[BucketCount];

            foreach(var v in _values)
            {
                var bucket = BucketingRule.DetermineBucket(v, BucketWidth);
                Buckets[bucket]++;
            }
        }

        public int[] Buckets { get; set; }

        public double Max()
        {
            Guard.IsNotNull(_values, "values", "No histogram data has been loaded yet.");

            return _values.Max<double>();
        }

        public double Min()
        {
            Guard.IsNotNull(_values, "values", "No histogram data has been loaded yet.");

            return _values.Min<double>();
        }

        public double BucketWidth
        {
            get { return _bucketWidth; }
            set { _bucketWidth = value; }
        }

        public int BucketCount
        {
            get { return (int)(Max() / BucketWidth) + 1; }
        }

        private IHistogramDataLoader DataLoader
        {
            get { return _dataLoader; }
        }

        private IBucketingRule BucketingRule
        {
            get { return _bucketingRule; }
        }
    }
}