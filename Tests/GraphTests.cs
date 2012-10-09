using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GraphTool;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GraphTests
    {
        [Test]
        public void NodeCanTraceAllDescendents()
        {
            var A = new Node("A");
            var B = new Node("B");
            var C = new Node("C");
            var D = new Node("D");
            var E = new Node("E");
            var F = new Node("F");

            A.Children.Add(B); B.Parents.Add(A);
            A.Children.Add(C); C.Parents.Add(A);
            B.Children.Add(E); E.Parents.Add(B);
            C.Children.Add(D); D.Parents.Add(C);
            D.Children.Add(E); E.Parents.Add(D);
            F.Children.Add(C); C.Parents.Add(F);

            Assert.That(A.Descendents().Distinct().Contains(B));
            Assert.That(A.Descendents().Distinct().Contains(C));
            Assert.That(A.Descendents().Distinct().Contains(D));
            Assert.That(A.Descendents().Distinct().Contains(E));
        }

        [Test]
        public void NodeCanTraceAllAntecedents()
        {
            var A = new Node("A");
            var B = new Node("B");
            var C = new Node("C");
            var D = new Node("D");
            var E = new Node("E");
            var F = new Node("F");

            A.Children.Add(B); B.Parents.Add(A);
            A.Children.Add(C); C.Parents.Add(A);
            B.Children.Add(E); E.Parents.Add(B);
            C.Children.Add(D); D.Parents.Add(C);
            D.Children.Add(E); E.Parents.Add(D);
            F.Children.Add(C); C.Parents.Add(F);

            foreach (var n in E.Antecedents().Distinct())
            {
                Trace.Write(n.Name + " ");
            }
            Trace.WriteLine("");

            Assert.That(E.Antecedents().Distinct().Contains(B));
            Assert.That(E.Antecedents().Distinct().Contains(A));
            Assert.That(E.Antecedents().Distinct().Contains(C));
            Assert.That(E.Antecedents().Distinct().Contains(D));
            Assert.That(E.Antecedents().Distinct().Contains(F));
        }

        [Test]
        public void AlgorithmCanFindAllConcurrencyChains()
        {
            var A = new Node("A");
            var B = new Node("B");
            var C = new Node("C");
            var D = new Node("D");
            var E = new Node("E");
            var F = new Node("F");
            var G = new Node("G");
            var H = new Node("H");
            var I = new Node("I");
            var J = new Node("J");
            var K = new Node("K");
            var L = new Node("L");

            A.Children.Add(B); B.Parents.Add(A);
            A.Children.Add(C); C.Parents.Add(A);
            B.Children.Add(E); E.Parents.Add(B);
            C.Children.Add(D); D.Parents.Add(C);
            D.Children.Add(E); E.Parents.Add(D);
            F.Children.Add(C); C.Parents.Add(F);
            F.Children.Add(G); G.Parents.Add(F);
            G.Children.Add(H); H.Parents.Add(G);
            H.Children.Add(K); K.Parents.Add(H);
            I.Children.Add(G); G.Parents.Add(I);
            I.Children.Add(J); J.Parents.Add(I);
            J.Children.Add(K); K.Parents.Add(J);
            L.Children.Add(J); J.Parents.Add(L);

            var allNodes = new Dictionary<string, Node>
                {
                    {"A", A},
                    {"B", B},
                    {"C", C},
                    {"D", D},
                    {"E", E},
                    {"F", F},
                    {"G", G},
                    {"H", H},
                    {"I", I},
                    {"J", J},
                    {"K", K},
                    {"L", L}
                };

            var algo = new GraphConcurrencyCharacterisation();
            algo.Run(allNodes);

            Trace.WriteLine("Max concurrency: " + algo.MaxConcurrency);
            Trace.WriteLine("Chains:          " + algo.ConcurrencyChains.Count);
        }
    }
}
