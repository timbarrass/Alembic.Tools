using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace HistogramTool
{
    [TestFixture]
    public class HistogramTests
    {
        private Histogram H;

        [SetUp]
        public void SetUp()
        {
            var bucketCountTestData = new List<double>() { 1d, 2.5d, 3d, 4.5d, 6d, 700d, 8d };
            var basicHistoData = new List<double>() { 1.5d, 2d, 4d, 5d, 6d, 4.3d, 0d, 124235d };

            var stubLoader = MockRepository.GenerateStub<IHistogramDataLoader>();
            stubLoader.Expect(x => x.LoadSingleValuedFile("bucketCountTestData")).Return(bucketCountTestData).Repeat.Any();
            stubLoader.Expect(x => x.LoadSingleValuedFile("basicHistoData.txt")).Return(basicHistoData).Repeat.Any();

            var linearBucketingRule = new LinearBucketingRule();
            linearBucketingRule.BucketWidth = 10;

            H = new Histogram(stubLoader, linearBucketingRule);
        }

        [Test]
        public void Histogram_CanBeInstantiated()
        {
            var h = new Histogram();
        }

        [Test]
        public void Histogram_LoadsBasicTextFile()
        {
            var h = new Histogram();

            h.LoadSingleValuedFile("basicHistoData.txt");

            Assert.AreEqual(8, h.DataCount);
        }

        // TODO: max,min are associated with the value set. The more I think about it the more it seems the Histo should be related to the value set by aggregation, not composition
        [Test]
        public void Histogram_ReturnsMinimumValue()
        {
            H.LoadSingleValuedFile("basicHistoData.txt");

            Assert.AreEqual(0d, H.Min());
        }

        [Test]
        public void Histogram_ReturnsMaximumValue()
        {
            H.LoadSingleValuedFile("basicHistoData.txt");

            Assert.AreEqual(124235d, H.Max());
        }

        [Test]
        public void Histogram_BuildsWithNoConstraints()
        {
            H.LoadSingleValuedFile("basicHistoData.txt");

            H.Build();

            Assert.AreEqual(7, H.Buckets[0]);
            Assert.AreEqual(1, H.Buckets[12423]);
        }

        [Test]
        public void Histogram_DeterminesCorrectNumberOfBuckets()
        {
            H.LoadSingleValuedFile("bucketCountTestData");

            Assert.AreEqual(71, H.BucketCount);
        }

        [Test]
        public void Histogram_CanBuildFromAggregateValueList()
        {
            var rule = new LinearBucketingRule();
            rule.BucketWidth = 1d;
            var h = new Histogram(rule);
            h.Values = new List<double> { 1d, 2d, 3d, 4d };

            h.Build();

            Assert.AreEqual(5, h.BucketCount);
            Assert.AreEqual(0, h.Buckets[0]);
            Assert.AreEqual(1, h.Buckets[1]);
            Assert.AreEqual(1, h.Buckets[4]);
        }

        /// <summary>
        /// Complement with negative tests -- what if value presented is outside of
        /// Min/Max range?
        /// </summary>
        [Test]
        public void LinearBucketingRule_DeterminesCorrectBucket()
        {
            var rule = new LinearBucketingRule();
            rule.BucketWidth = 10d;

            Assert.AreEqual(0, rule.DetermineBucket(5));
            Assert.AreEqual(0, rule.DetermineBucket(9));
            Assert.AreEqual(1, rule.DetermineBucket(10));
            Assert.AreEqual(125, rule.DetermineBucket(1251));
        }

        [Test]
        public void LinearBucketingRule_DeterminesCorrectValue()
        {
            var rule = new LinearBucketingRule();
            rule.BucketWidth = 10d;

            Assert.AreEqual(0d, rule.DetermineValue(0));
            Assert.AreEqual(10d, rule.DetermineValue(1));
            Assert.AreEqual(100d, rule.DetermineValue(10));
        }
    }
}