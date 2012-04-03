using System;
using System.Collections.Generic;
using FileHelpers;
using Mono.Options;
using TimeSeriesTool;

namespace TSeries
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: tseries file|f=<data file> [step|s=<fixed step size, as dd:hh:mm:ss.]");
                return;
            }

            string step = "00:05:00";
            string file = string.Empty;
            string tag = string.Empty;
            bool matchTags = false;
            int initialCount = 0;
            OptionSet p = new OptionSet()
                .Add("file=|f=", f => file = f)
                .Add("step=|s=", s => step = s)
                .Add("tag=|t=", t => { tag = t; matchTags = true; })
                .Add("initial=|i=", i => initialCount = Convert.ToInt32(i));
            var unparsed = p.Parse(args);

            FileDataLoader loader = null;
            if (matchTags)
            {
                loader = new FileDataLoader(file, tag);
            }
            else
            {
                loader = new FileDataLoader(file);
            }
            var startsAndEnds = loader.Load();
            
            TimeSpan fixedStep = TimeSpan.Parse(step);
            TimeSeries series = new TimeSeries(fixedStep);
            series.Build(startsAndEnds);

            var index = 0;
            foreach(var t in series.Timestamps)
            {
                Console.WriteLine("{0}\t{1}\t{2}", t, series.Values[index] + initialCount, series.Highwater[index]);
                index++;
            }
        }
    }

    public class FileDataLoader
    {
        private string _fileName;

        private string _requiredTag;

        private bool _matchRequiredTag;

        public FileDataLoader(string fileName)
        {
            _fileName = fileName;

            _matchRequiredTag = false;
        }

        public FileDataLoader(string fileName, string requiredTag)
        {
            _fileName = fileName;

            _requiredTag = requiredTag;

            _matchRequiredTag = true;
        }

        public StartAndEndPair[] Load()
        {

            var fh = new FileHelperEngine<CommentedStartAndEndPair>();
            var values = fh.ReadFile(_fileName);

            var returnValues = new List<StartAndEndPair>();

            foreach (var value in values)
            {
                if (_matchRequiredTag)
                {
                    if (value.Comment.Contains(_requiredTag))
                    {
                        returnValues.Add(new StartAndEndPair(value.Start, value.End));
                    }
                }
                else
                {
                    returnValues.Add(new StartAndEndPair(value.Start, value.End));
                }
            }

            return returnValues.ToArray();
        }
    }
}
