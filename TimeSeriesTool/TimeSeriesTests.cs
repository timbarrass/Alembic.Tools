﻿using System;
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
            var t = new TimeSeries();
        }

        [Test]
        public void TimeSeriesTool_BuildsStartAndEndLists()
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

            var t = new TimeSeries();

            DateTime[] starts;
            DateTime[] ends;

            // Remember -- we still want to be a stateless-as-possible values processor ...
            t.BuildSortedStartsAndEnds(startsAndEnds, out starts, out ends);

            Assert.AreEqual(2, starts.Length);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:05:34.222"), starts[0]);
            Assert.AreEqual(2, ends.Length);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:06:34.222"), ends[0]);
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

            var t = new TimeSeries();

            t.Build(startsAndEnds);

            Assert.AreEqual(3, t.Timestamps.Length);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:05:34.222"), t.Timestamps[0]);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:05:34.222").AddMinutes(5), t.Timestamps[0]);
        }

        [Test]
        [ExpectedException("System.InvalidOperationException")]
        public void TimeSeriesTool_BuildThrowsIfNotEnoughTasks()
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

            var t = new TimeSeries();

            t.Build(startsAndEnds);

            Assert.AreEqual(3, t.Timestamps.Length);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:05:34.222"), t.Timestamps[0]);
            Assert.AreEqual(DateTime.Parse("22-08-2011 17:05:34.222").AddMinutes(5), t.Timestamps[0]);
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

        public void Build(StartAndEndPair[] startsAndEnds)
        {
            DateTime[] starts;
            DateTime[] ends;
            
            BuildSortedStartsAndEnds(startsAndEnds, out starts, out ends);

            // ToDo: refactor ... to stateful TimeSeriesBuilder? Requires sorted starts and ends?
            int currentIndex = 0;
            int otherIndex = 0;
            int runningCount = 0;
            var currentList = starts;
            var otherList = ends;
            var incdec = 1;

            var timestamp = new List<DateTime>();
            var value = new List<int>();

            while (currentIndex < currentList.Length - 1)
            {
                runningCount += incdec;
                timestamp.Add(currentList[currentIndex]);
                value.Add(runningCount);

                currentIndex++;
                if (currentList[currentIndex] > otherList[otherIndex])
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

            // ToDo: refactor to stateful builder?
            if(value.Count <= 2)
            {
                throw new InvalidOperationException("Can't generate timeseries for 2 or fewer tasks.");
            }
            
            _timestamps = new DateTime[starts.Length];
            _values = new double[starts.Length];
            _highwater = new double[starts.Length];

            var index = 0;
            // ToDo: make delta configurable
            var dt = new TimeSpan(0, 5, 0);
            var current = timestamp[0];

            int counter = 0;
            while (current < timestamp[timestamp.Count - 2])
            {
                var next = current.AddTicks(dt.Ticks);

                var highwater = 0;
                while (index < timestamp.Count - 1
                    && timestamp[index + 1] < next)
                {
                    index++;
                    if (highwater < value[index])
                        highwater = value[index];
                }

                Timestamps[counter] = current;
                Values[counter] = value[index];
                Highwater[counter] = highwater;
                
                current = next;
                counter++;
            }
        }

        // ToDo: more state for stateful builder?
        public double[] Highwater
        {
            get {
                return _highwater;
            }
            set {
                _highwater = value;
            }
        }

        // ToDo: more state for stateful builder?
        public Double[] Values
        {
            get {
                return _values;
            }
            set {
                _values = value;
            }
        }

        // ToDo: more state for stateful builder?
        public DateTime[] Timestamps
        {
            get {
                return _timestamps;
            }
            set {
                _timestamps = value;
            }
        }
    }
}