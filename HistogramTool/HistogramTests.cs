using System;
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
            var linearBucketingRule = new LinearBucketingRule(10d, 0d, Double.MaxValue);

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
            var rule = new LinearBucketingRule(1d, 0d, 5d);
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
            var rule = new LinearBucketingRule(10d, 0d, 1000d);

            Assert.AreEqual(0, rule.DetermineBucket(5d));
            Assert.AreEqual(0, rule.DetermineBucket(9d));
            Assert.AreEqual(1, rule.DetermineBucket(10d));
            Assert.AreEqual(125, rule.DetermineBucket(1251d));
        }

        [Test]
        public void LinearBucketingRule_DeterminesCorrectValue()
        {
            var rule = new LinearBucketingRule(10d, 0d, 1000d);

            Assert.AreEqual(0d, rule.DetermineValue(0));
            Assert.AreEqual(10d, rule.DetermineValue(1));
            Assert.AreEqual(100d, rule.DetermineValue(10));
        }

        [Test]
        public void LinearBucketingrule_ObservesMaximumValue()
        {
            var rule = new LinearBucketingRule(10d, 0d, 50d);

            Assert.IsTrue(rule.IsHigh(51d));
            Assert.IsFalse(rule.IsHigh(49d));
        }

        [Test]
        public void LinearBucketingRule_ObservesMinimumValue()
        {
            var rule = new LinearBucketingRule(10d, 10d, 50d);

            Assert.IsTrue(rule.IsLow(9d));
            Assert.IsFalse(rule.IsLow(11d));
        }

        [Test]
        public void LinearBucketingRule_CalculatesBucketCount()
        {
            var rule = new LinearBucketingRule(10d, 0d, 100d);

            Assert.AreEqual(11, rule.DetermineBucketCount());
        }

        [Test]
        public void Histogram_BucketsHighOnBuild()
        {
            var values = new List<double>() { 1d, 2d, 3d, 4d, 5d };
            var rule = new LinearBucketingRule(1d, 0d, 5d);
            var h = new Histogram(rule);

            h.Build(values);

            Assert.AreEqual(1, h.High);            
        }

        [Test]
        public void Histogram_BucketsLowOnBuild()
        {
            var values = new List<double>() { 1d, 2d, 3d, 4d, 5d };
            var rule = new LinearBucketingRule(1d, 2d, 6d);
            var h = new Histogram(rule);

            h.Build(values);

            Assert.AreEqual(1, h.Low);
        }    
    }
}