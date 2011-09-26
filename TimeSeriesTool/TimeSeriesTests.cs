using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;
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
        }
    }



    [DelimitedRecord(",")]
    public class StartAndEndPair
    {
        [FieldConverter(ConverterKind.Date, "dd-MM-yyyy hh24:mi:ss")] 
        public DateTime Start;

        [FieldConverter(ConverterKind.Date, "dd-MM-yyyy hh24:mi:ss")] 
        public DateTime End;

        public StartAndEndPair(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }

    public class TimeSeries
    {
        private DateTime[] _timestamps;
        private double[] _values;
        private double[] _highwater;
        private TimeSpan _fixedStep;

        public TimeSeries(TimeSpan fixedStep)
        {
            FixedStep = fixedStep;
        }

        public void Build(StartAndEndPair[] startsAndEnds)
        {
            using (var builder = new TimeSeriesBuilder(FixedStep))
            {
                builder.Build(startsAndEnds, out _timestamps, out _values, out _highwater);
            }
        }

        public double[] Highwater
        {
            get { return _highwater; }
        }

        public Double[] Values
        {
            get { return _values; }
        }

        public DateTime[] Timestamps
        {
            get { return _timestamps; }
        }

        public TimeSpan FixedStep
        {
            get { return _fixedStep; }
            private set { _fixedStep = value; }
        }
    }

    public class TimeSeriesBuilder : IDisposable
    {
        public TimeSpan FixedStep { get; set; }

        public TimeSeriesBuilder(TimeSpan fixedStep)
        {
            FixedStep = fixedStep;    
        }

        public void BuildSortedStartsAndEnds(StartAndEndPair[] startsAndEnds, out DateTime[] starts, out DateTime[] ends)
        {
            starts = new DateTime[startsAndEnds.Length];
            ends = new DateTime[startsAndEnds.Length];

            int count = 0;
            foreach(var pair in startsAndEnds)
            {
                starts[count] = pair.Start;
                ends[count] = pair.End;
                count++;
            }

            Array.Sort(starts);
            Array.Sort(ends);
        }

        public void BuildVariableStepTimeSeries(DateTime[] starts, DateTime[] ends)
        {
            int currentIndex = 0;
            int otherIndex = 0;
            int runningCount = 0;
            var currentList = starts;
            var otherList = ends;
            var incdec = 1;

            VariableStepTimestamp = new List<DateTime>();
            VariableStepCount = new List<int>();

            while (currentIndex < currentList.Length)
            {
                runningCount += incdec;
                VariableStepTimestamp.Add(currentList[currentIndex]);
                VariableStepCount.Add(runningCount);

                currentIndex++;
                if (currentIndex == currentList.Length || currentList[currentIndex] > otherList[otherIndex])
                {
                    var tempIndex = currentIndex;
                    currentIndex = otherIndex;
                    otherIndex = tempIndex;
                    var tempList = currentList;
                    currentList = otherList;
                    otherList = tempList;

                    incdec *= -1;
                }
            }

            if (VariableStepCount.Count <= 2)
            {
                throw new InvalidOperationException("Can't generate timeseries for 2 or fewer tasks.");
            }
        }

        public IList<int> VariableStepCount { get; set; }

        public IList<DateTime> VariableStepTimestamp { get; set; }

        public void BuildFixedStepTimeSeries(out DateTime[] timestamps, out double[] values, out double[] highwater)
        {
            var index = 0;
            var dt = FixedStep;
            var current = VariableStepTimestamp[0];
            var last = VariableStepTimestamp[VariableStepTimestamp.Count - 1];
            var steps = 1+ (last.Ticks - current.Ticks) / dt.Ticks;

            timestamps = new DateTime[steps];
            values = new double[steps];
            highwater = new double[steps];

            int counter = 0;
            while (current < last)
            {
                var next = current.AddTicks(dt.Ticks);

                var currentHighwater = 0;
                while (index < VariableStepTimestamp.Count - 1
                       && VariableStepTimestamp[index + 1] < next)
                {
                    index++;
                    if (currentHighwater < VariableStepCount[index])
                        currentHighwater = VariableStepCount[index];
                }

                timestamps[counter] = current;
                values[counter] = VariableStepCount[index];
                highwater[counter] = currentHighwater;

                current = next;
                counter++;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if(disposing)
            {
                VariableStepTimestamp.Clear();
                VariableStepCount.Clear();
            }
        }

        ~TimeSeriesBuilder()
        {
            Dispose(false);
        }

        public void Build(StartAndEndPair[] startsAndEnds, out DateTime[] timestamps, out double[] values, out double[] highwater)
        {
            DateTime[] starts;
            DateTime[] ends;
            BuildSortedStartsAndEnds(startsAndEnds, out starts, out ends);

            BuildVariableStepTimeSeries(starts, ends);

            BuildFixedStepTimeSeries(out timestamps, out values, out highwater);
        }
    }
}
