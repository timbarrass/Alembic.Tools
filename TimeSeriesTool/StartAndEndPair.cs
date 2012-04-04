using System;
using System.Globalization;
using FileHelpers;

namespace TimeSeriesTool
{
    [DelimitedRecord(",")]
    public class StartAndEndPair
    {
        [FieldConverter(typeof(CustomDateTimeConverter))]
        public DateTime Start;

        [FieldConverter(typeof(CustomEndTimeConverter))]
        public DateTime End;

        public StartAndEndPair()
        {
        }

        public StartAndEndPair(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }

    internal class CustomDateTimeConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            return DateTime.Parse(from);
        }
    }

    internal class CustomEndTimeConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            if(string.IsNullOrEmpty(from))
            {
                return DateTime.MaxValue;
            }

            return DateTime.Parse(from);
        }
    }

    [DelimitedRecord(",")]
    public class CommentedStartAndEndPair
    {
        //[FieldConverter(ConverterKind.Date, "dd-MM-yyyy HH:mm:ss.fffff")]
        [FieldConverter(typeof(CustomDateTimeConverter))]
        public DateTime Start;

        //[FieldConverter(ConverterKind.Date, "dd-MM-yyyy HH:mm:ss.fffff")]
        [FieldConverter(typeof(CustomDateTimeConverter))]
        public DateTime End;

        public string Comment;

        public CommentedStartAndEndPair()
        {
        }

        public CommentedStartAndEndPair(DateTime start, DateTime end, string comment)
        {
            Start = start;
            End = end;
            Comment = comment;
        }
    }
}