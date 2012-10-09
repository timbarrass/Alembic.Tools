using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;
using GraphTool;
using Mono.Options;

namespace Graph
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: graph -file=<filename>");
                return;
            }

            var fileName = string.Empty;

            OptionSet p = new OptionSet()
                .Add("file=", f => fileName = f);

            var unparsed = p.Parse(args);

            var loader = new FileDataLoader(fileName);
            var data = loader.Load();

            var allNodes = new Dictionary<string, Node>();

            foreach(var relationship in data)
            {
                if (! allNodes.ContainsKey(relationship.Key))
                {
                    allNodes.Add(relationship.Key, new Node(relationship.Key));
                }

                var parent = allNodes[relationship.Key];

                if (!allNodes.ContainsKey(relationship.Value))
                {
                    allNodes.Add(relationship.Value, new Node(relationship.Value));
                }

                var child = allNodes[relationship.Value];

                parent.Children.Add(child); child.Parents.Add(parent);
            }

            var algo = new GraphConcurrencyCharacterisation();
            algo.Run(allNodes);

            Console.WriteLine("Max concurrency:   " + algo.MaxConcurrency);
            Console.WriteLine("Concurrent chains: " + algo.ConcurrencyChains.Count);
        }
    }

    internal class FileDataLoader
    {
        private string _fileName;

        public FileDataLoader(string fileName)
        {
            _fileName = fileName;
        }

        public IEnumerable<KeyValuePair<string, string>> Load()
        {
            var fh = new FileHelperEngine<ParentAndChild>();
            var values = fh.ReadFile(_fileName).Select<ParentAndChild, KeyValuePair<string, string>>(x => new KeyValuePair<string, string>(x.Parent, x.Child)).ToList<KeyValuePair<string, string>>();
            return values;
        }
    }

    [DelimitedRecord(",")]
    public class ParentAndChild
    {
        public string Parent { get; set; }

        public string Child { get; set; }
    }
}
