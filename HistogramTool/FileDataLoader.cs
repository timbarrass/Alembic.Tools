using System.Collections.Generic;
using System.Linq;
using FileHelpers;

namespace HistogramTool
{
    public class FileDataLoader : IHistogramDataLoader
    {
        public IList<double> LoadSingleValuedFile(string fileName)
        {
            var fh = new FileHelperEngine<SingleValue>();
            var values = fh.ReadFile(fileName).Select<SingleValue, double>(x => x.Value).ToList<double>();
            return values;
        }
    }
}