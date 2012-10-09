using System.Collections.Generic;

namespace GraphTool
{
    public class Node
    {
        public string Name;

        public List<Node> Children = new List<Node>();

        public List<Node> Parents = new List<Node>();

        public Node(string name)
        {
            Name = name;
        }

        public IEnumerable<Node> Descendents()
        {
            var childStack = new Stack<Node>();
            childStack.Push(this);

            while (childStack.Count != 0)
            {
                var child = childStack.Pop();

                yield return child;

                foreach (var node in child.Children)
                {
                    childStack.Push(node);
                }
            }
        }

        public IEnumerable<Node> Antecedents()
        {
            var parentStack = new Stack<Node>();
            parentStack.Push(this);

            while (parentStack.Count != 0)
            {
                var parent = parentStack.Pop();

                yield return parent;

                foreach (var node in parent.Parents)
                {
                    parentStack.Push(node);
                }
            }
        }
    }
}