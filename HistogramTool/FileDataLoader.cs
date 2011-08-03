using System.Collections.Generic;
using System.Linq;
using FileHelpers;

namespace HistogramTool
{
    public class FileDataLoader : IHistogramDataLoader
    {
        private string _fileName;

        public FileDataLoader(string fileName)
        {
            _fileName = fileName;
        }

        public IList<double> Load()
        {
            var fh = new FileHelperEngine<SingleValue>();
            var values = fh.ReadFile(_fileName).Select<SingleValue, double>(x => x.Value).ToList<double>();
            return values;
        }
    }
}