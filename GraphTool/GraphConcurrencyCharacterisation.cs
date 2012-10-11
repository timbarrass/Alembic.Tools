using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphTool
{
    public class GraphConcurrencyCharacterisation
    {
        public int MaxConcurrency;

        public List<List<Node>> ConcurrencyChains = new List<List<Node>>();


        public void Run(IDictionary<string, Node> allNodes)
        {
            foreach (var root in allNodes.Values)
            {
                var greyList = allNodes.Values;

                GetNextSteps(new Stack<Node>(), greyList, root);

                _previouslySeenRoots.Add(root);
            }
        }

        private void GetNextSteps(Stack<Node> currentChain, IEnumerable<Node> greyList, Node currentNode)
        {
            currentChain.Push(currentNode);

            var newGreyList =
                greyList
                    .Except(currentNode.Antecedents())
                    .Except(currentNode.Descendents())
                    .Distinct();

            if (newGreyList.Count() == 0)
            {
                ConcurrencyChains.Add(currentChain.ToList());

                MaxConcurrency = Math.Max(MaxConcurrency, (int) currentChain.Count);
            }

            foreach (var newPossible in newGreyList)
            {
                if (_previouslySeenRoots.Contains(newPossible)) continue;

                GetNextSteps(currentChain, newGreyList, newPossible);
            }

            currentChain.Pop();
        }

        private readonly List<Node> _previouslySeenRoots = new List<Node>(); 
    }
}