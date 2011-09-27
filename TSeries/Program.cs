using System;
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
            OptionSet p = new OptionSet()
                .Add("file=|f=", f => file = f)
                .Add("step=|s=", s => step = s);
            var unparsed = p.Parse(args);

            var loader = new FileDataLoader(file);
            var startsAndEnds = loader.Load();

            TimeSpan fixedStep = TimeSpan.Parse(step);
            TimeSeries series = new TimeSeries(fixedStep);
            series.Build(startsAndEnds);

            var index = 0;
            foreach(var t in series.Timestamps)
            {
                Console.WriteLine("{0}\t{1}\t{2}", t, series.Values[index], series.Highwater[index]);
                index++;
            }
        }
    }

    public class FileDataLoader
    {
        private string _fileName;

        public FileDataLoader(string fileName)
        {
            _fileName = fileName;
        }

        public StartAndEndPair[] Load()
        {
            var fh = new FileHelperEngine<StartAndEndPair>();
            var values = fh.ReadFile(_fileName);
            return values;
        }
    }
}
