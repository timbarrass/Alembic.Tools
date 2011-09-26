using System;
using FileHelpers;

namespace TimeSeriesTool
{
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
}