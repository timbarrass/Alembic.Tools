using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraphTool
{
    public class GraphConcurrencyCharacterisation
    {
        public void Run(IDictionary<string, Node> allNodes)
        {
            foreach (var root in allNodes.Values)
            {
                //Console.WriteLine("root: " + root.Name);

                IEnumerable<Node> greyList = allNodes.Values;

                GetNextSteps(greyList, root);
            }
        }

        public int MaxConcurrency;

        public List<List<Node>> ConcurrencyChains = new List<List<Node>>();

        private void GetNextSteps(IEnumerable<Node> greyList, Node currentNode)
        {
            _currentChain.Push(currentNode);

            var newGreyList =
                greyList.Except(currentNode.Antecedents()).Except(currentNode.Descendents()).Distinct();

            if (newGreyList.Count() == 0) // end of chain condition -- update state
            {
                //foreach (var n in _currentChain) Console.Write(n.Name + "-");

                //Console.WriteLine("");

                ConcurrencyChains.Add(_currentChain.ToList());

                MaxConcurrency = Math.Max(MaxConcurrency, (int) _currentChain.Count);
            }

            foreach (var newPossible in newGreyList)
            {
                GetNextSteps(newGreyList, newPossible);
            }

            _currentChain.Pop();
        }

        private readonly Stack<Node> _currentChain = new Stack<Node>();
    }
}