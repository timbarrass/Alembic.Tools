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
            var linearBucketingRule = new LinearBucketingRule();
            linearBucketingRule.BucketWidth = 10;

            H = new Histogram(linearBucketingRule);
        }

        [Test]
        public void Histogram_CanBeInstantiated()
        {
            var h = new Histogram();
        }

        [Test]
        public void Histogram_BuildsFromValueList()
        {
            var values = new List<double>() { 1d, 2d, 3d, 4d, 5d };
            var rule = new LinearBucketingRule();
            rule.BucketWidth = 1d;
            var h = new Histogram(rule);

            h.Build(values);

            Assert.AreEqual(0, h.Buckets[0]);
            Assert.AreEqual(1, h.Buckets[1]);
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