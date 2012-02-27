using System;
using FileHelpers;

namespace TimeSeriesTool
{
    [DelimitedRecord(",")]
    public class StartAndEndPair
    {
        [FieldConverter(ConverterKind.Date, "dd-MM-yyyy HH:mm:ss.fffff")] 
        public DateTime Start;

        [FieldConverter(ConverterKind.Date, "dd-MM-yyyy HH:mm:ss.fffff")] 
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

    [DelimitedRecord(",")]
    public class CommentedStartAndEndPair
    {
        [FieldConverter(ConverterKind.Date, "dd-MM-yyyy HH:mm:ss.fffff")]
        public DateTime Start;

        [FieldConverter(ConverterKind.Date, "dd-MM-yyyy HH:mm:ss.fffff")]
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