using System;
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

        public Histogram(IBucketingRule bucketingRule) : this(new FileDataLoader(), bucketingRule)
        {
        }

        public void LoadSingleValuedFile(string fileName)
        {
            Values = DataLoader.LoadSingleValuedFile(fileName);
        }

        public IList<double> Values
        {
            get
            {
                Guard.IsNotNull(_values, "values", "No histogram data has been loaded yet.");

                return _values;
            }
            set
            {
                _values = value;
            }
        }

        public int DataCount
        {
            get
            {      
                return Values.Count;
            }
        }

        public void Build()
        {
            Guard.IsNotNull(_values, "values", "No histogram data has been loaded yet.");
            
            Buckets = new int[BucketCount];

            foreach(var v in _values)
            {
                var bucket = BucketingRule.DetermineBucket(v);
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

        public int BucketCount
        {
            get { return (int)(Max() / BucketingRule.BucketWidth) + 1; }
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