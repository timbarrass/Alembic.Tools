using System.Collections.Generic;

namespace HistogramTool
{
    public interface IHistogramDataLoader
    {
        IList<double> LoadSingleValuedFile(string fileName);
    }
}