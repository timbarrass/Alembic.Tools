using System;
using System.Collections.Generic;

namespace TimeSeriesTool
{
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