using System;
using NUnit.Framework;

namespace TimeSeriesTool
{
    [TestFixture]
    public class TimeSeriesTests
    {
        [Test]
        public void TimeSeriesTool_CanBeInstantiated()
        {
            var t = new TimeSeries(new TimeSpan(0,5,0));
        }

        [Test]
        public void TimeSeriesBuilder_BuildsStartAndEndLists()
        {
            var startsAndEnds = new StartAndEndPair[]
                                    {
                                       new StartAndEndPair(
                                           DateTime.Parse("22-08-2011 17:05:34.222"), 
                                           DateTime.Parse("22-08-2011 17:06:34.222")),
                                       new StartAndEndPair(
                                           DateTime.Parse("22-08-2011 17:07:34.222"), 
                                           DateTime.Parse("22-08-2011 17:08:34.222"))
                                    };

            var t = new TimeSeriesBuilder(new TimeSpan(0,5,0));

            DateTime[] starts;
            DateTime[] ends;

            t.BuildSortedStartsAndEnds(startsAndEnds, out starts, out ends);

            Assert.AreEqual(2, starts.Length);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:05:34.222"), starts[0]);
            Assert.AreEqual(2, ends.Length);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:06:34.222"), ends[0]);
        }

        [Test]
        public void TimeSeriesBuilder_BuildsVariableStepTimeSeries_FromSortedStartsAndEnds()
        {
            var starts = new DateTime[] { DateTime.Parse("22-08-2011 17:05:34.222"), DateTime.Parse("22-08-2011 17:07:34.222"), DateTime.Parse("22-08-2011 17:09:34.222") };
            var ends = new DateTime[] { DateTime.Parse("22-08-2011 17:06:34.222"), DateTime.Parse("22-08-2011 17:08:34.222"), DateTime.Parse("22-08-2011 17:15:34.222") };

            var t = new TimeSeriesBuilder(new TimeSpan(0,5,0));
            
            t.BuildVariableStepTimeSeries(starts, ends);

            Assert.AreEqual(6, t.VariableStepTimestamp.Count);
            
        }

        [Test]
        public void TimeSeriesTool_BuildsTimeSeriesFromStartsAndEnds()
        {
            var startsAndEnds = new StartAndEndPair[]
                                    {
                                        new StartAndEndPair(
                                            DateTime.Parse("22-08-2011 17:05:34.222"), 
                                            DateTime.Parse("22-08-2011 17:06:34.222")),
                                        new StartAndEndPair(
                                            DateTime.Parse("22-08-2011 17:07:34.222"), 
                                            DateTime.Parse("22-08-2011 17:08:34.222")),
                                        new StartAndEndPair(
                                            DateTime.Parse("22-08-2011 17:09:34.222"), 
                                            DateTime.Parse("22-08-2011 17:15:34.222"))
                                    };

            var t = new TimeSeries(new TimeSpan(0,5,0));

            t.Build(startsAndEnds);

            Assert.AreEqual(3, t.Timestamps.Length);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:05:34.222"), t.Timestamps[0]);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:05:34.222").AddMinutes(5), t.Timestamps[1]);

            // ToDo: Asserts around values at those timestamps
        }

        [Test]
        [ExpectedException("System.InvalidOperationException")]
        public void TimeSeriesTool_BuildThrowsIfNotEnoughTasks()
        {
            var startsAndEnds = new StartAndEndPair[]
                                    {
                                        new StartAndEndPair(
                                            DateTime.Parse("22-08-2011 17:05:34.222"), 
                                            DateTime.Parse("22-08-2011 17:06:34.222"))
                                    };

            var t = new TimeSeries(new TimeSpan(0,5,0));

            t.Build(startsAndEnds);
        }

        [Test]
        public void TimeSeriesTool_GeneratesSeriesWithCorrectSteps()
        {
            var startsAndEnds = new StartAndEndPair[]
                                    {
                                        new StartAndEndPair(
                                            DateTime.Parse("22-08-2011 17:05:34.222"), 
                                            DateTime.Parse("22-08-2011 17:06:34.222")),
                                        new StartAndEndPair(
                                            DateTime.Parse("22-08-2011 17:07:34.222"), 
                                            DateTime.Parse("22-08-2011 17:08:34.222")),
                                        new StartAndEndPair(
                                            DateTime.Parse("22-08-2011 17:09:34.222"), 
                                            DateTime.Parse("22-08-2011 17:15:34.222"))
                                    };

            var t = new TimeSeries(new TimeSpan(0, 1, 0));

            t.Build(startsAndEnds);

            Assert.AreEqual(11, t.Timestamps.Length);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:06:34.222"), t.Timestamps[1]);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:15:34.222"), t.Timestamps[10]);
        }
    }
}
