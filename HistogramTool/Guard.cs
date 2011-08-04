using System;

namespace HistogramTool
{
    public static class Guard
    {
        public static void IsNotNull(object value, string parameter, string message = "")
        {
            if (value == null)
                throw new ArgumentNullException(parameter, message);
        }

        public static void IsNotZero(double value, string parameter, string message = "")
        {
            if (value == 0)
                throw new ArgumentException(message, parameter);
        }

        public static void IsLessThan(double first, double second, string firstParameter, string secondParameter, string message = "")
        {
            if (first < second)
                throw new ArgumentException(message, firstParameter + "-" + secondParameter);
        }
    }
}