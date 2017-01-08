using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.QualityTools.UnitTestFramework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Simulation
{
    class AStar
    {
        private class Node
        {
            public Tuple<int, int> pos;
            public bool walkable;
            public Node parent;
            public Node (Tuple<int,int> pos, bool walkable)
            {
                this.pos = pos;
                this.walkable = walkable;
            }
        }
        Node pos;
        Node dest;
        bool[,] map;

        public AStar(Tuple<int, int> pos, Tuple<int, int> dest, bool[,] map)
        {
            this.pos = new Node(pos, true);
            this.dest = new Node(dest, true);
            this.map = map;
            Assert.IsTrue(dest.Item1 >= 0);
            Assert.IsTrue(dest.Item2 >= 0);
            Assert.IsTrue(pos.Item1 >= 0);
            Assert.IsTrue(pos.Item2 >= 0);
            Assert.IsTrue(dest.Item1 <= map.GetUpperBound(0));
            Assert.IsTrue(dest.Item2 <= map.GetUpperBound(1));
            Assert.IsTrue(pos.Item1 <= map.GetUpperBound(0));
            Assert.IsTrue(pos.Item2 <= map.GetUpperBound(1));
        }

        public Tuple<int,int>[] CalculatePath()
        {
            if (pos.pos.Equals(dest.pos))
                return new Tuple<int, int>[0];
            List<Node> frontier = new List<Node>();
            frontier.Add(pos);
            bool atend = false;
            for(int i = 0; i < frontier.Count && !atend; i++)
            {
                var current = frontier[i];
                foreach(Node next in Neighbors(map, current))
                {
                    if (next.pos.Equals(dest.pos))
                    {
                        atend = true;
                        next.parent = current;
                        frontier.Add(next);
                    }
                        
                    if (!next.walkable || atend)
                        continue;
                    bool partoffroniter = false;
                    foreach(Node f in frontier)
                    {
                        if (f.pos.Equals(next.pos))
                            partoffroniter = true;
                    }
                    if (!partoffroniter)
                    {
                        next.parent = current;
                        frontier.Add(next);
                    }
                    Assert.IsTrue(frontier.Count <= (map.GetUpperBound(0) + 1) * (map.GetUpperBound(1) + 1));
                }
            }
            
            Assert.IsTrue(frontier.Last().pos.Item1 == dest.pos.Item1);
            Assert.IsTrue(frontier.Last().pos.Item2 == dest.pos.Item2);

            Node n = frontier.Last();
            List<Tuple<int, int>> path = new List<Tuple<int, int>>();
            while (!n.Equals(pos))
            {
                path.Add(n.pos);
                n = n.parent;
            }
            path.Reverse();

            return path.ToArray();
        }

        private Node[] Neighbors(bool[,] map, Node n)
        {
            List<Node> neighbors = new List<Node>();
            int[,] offsets = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
            for(int i = 0; i < 4; i++)
            {
                Tuple<int, int> p = new Tuple<int, int>(n.pos.Item1 + offsets[i, 0], n.pos.Item2 + offsets[i, 1]);
                if (p.Item1 >= 0 && p.Item2 >= 0 && p.Item1 < map.GetUpperBound(0) + 1 && p.Item2 < map.GetUpperBound(1) + 1)
                {
                    bool walkable = map[p.Item1, p.Item2];
                    neighbors.Add(new Node(p, walkable));
                }
            }
            return neighbors.ToArray();
        }
    }

    
}
