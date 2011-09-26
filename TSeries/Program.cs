using System;
using Mono.Options;

namespace TSeries
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: histo -file=<filename> [-w=<bucket width>] [-l=<low value> -h=<high value>]");
                return;
            }
        }
    }
}
