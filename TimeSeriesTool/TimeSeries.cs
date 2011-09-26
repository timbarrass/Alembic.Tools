using System;

namespace TimeSeriesTool
{
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
}