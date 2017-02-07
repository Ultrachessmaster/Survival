using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Priority_Queue;

namespace Simulation
{
    class AStar
    {
        private class Node : FastPriorityQueueNode
        {
            public XY pos;
            public bool walkable;
            public Node parent;
            public int cost;
            public Node (XY pos, bool walkable)
            {
                this.pos = pos;
                this.walkable = walkable;
            }
        }
        Node pos;
        Node dest;
        bool[,] map;

        public AStar(XY pos, XY dest, bool[,] map)
        {
            this.pos = new Node(pos, true);
            this.pos.cost = 0;
            this.dest = new Node(dest, true);
            this.map = map;
            Assert.IsTrue(dest.X >= 0);
            Assert.IsTrue(dest.Y >= 0);
            Assert.IsTrue(pos.X >= 0);
            Assert.IsTrue(pos.Y >= 0);
            Assert.IsTrue(dest.X <= map.GetUpperBound(0));
            Assert.IsTrue(dest.Y <= map.GetUpperBound(1));
            Assert.IsTrue(pos.X <= map.GetUpperBound(0));
            Assert.IsTrue(pos.Y <= map.GetUpperBound(1));
        }

        public XY[] CalculatePath()
        {
            if (pos.pos.Equals(dest.pos))
                return new XY[0];
            FastPriorityQueue<Node> frontier = new FastPriorityQueue<Node>((map.GetUpperBound(0) + 1) * (map.GetUpperBound(1) + 1));
            List<Node> beenthere = new List<Node>();
            frontier.Enqueue(pos, 0);
            bool atend = false;
            int i = 0;
            while (frontier.Count != 0)
            {
                i++;
                var current = frontier.Dequeue();
                var neighbors = Neighbors(map, current);
                for (int j = 0; j < neighbors.Length; j++)
                {
                    var next = neighbors[j];
                    var cost = current.cost + 1;
                    if (next.pos.Equals(dest.pos))
                    {
                        atend = true;
                        dest.parent = current;
                    }

                    if (!next.walkable || atend)
                        continue;

                    bool partofbeenthere = false;
                    for (int bf = 0; bf < beenthere.Count; bf++)
                    {
                        var b = beenthere[bf];
                        if (b.pos.Equals(next.pos))
                            partofbeenthere = true;
                    }
                    if (!partofbeenthere || cost < current.cost)
                    {
                        next.cost = cost;
                        next.parent = current;
                        var minpriority = cost + ManhattanDistance(next, dest);
                        frontier.Enqueue(next, minpriority);
                        if (!partofbeenthere)
                        {
                            beenthere.Add(next);
                        }
                    }
                }
                if (i > 250)
                    return null;
            }

            Node n = dest;
            List<XY> path = new List<XY>();
            while (!n.Equals(pos))
            {
                path.Add(n.pos);
                n = n.parent;
            }
            path.Reverse();

            return path.ToArray();
        }

        private int ManhattanDistance(Node node, Node goal)
        {
            return Math.Abs(goal.pos.X - node.pos.X) + Math.Abs(goal.pos.Y - node.pos.Y);
        }

        private Node[] Neighbors(bool[,] map, Node n)
        {
            List<Node> neighbors = new List<Node>();
            int[,] offsets = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
            for(int i = 0; i < 4; i++)
            {
                XY p = new XY(n.pos.X + offsets[i, 0], n.pos.Y + offsets[i, 1]);
                if (p.X >= 0 && p.Y >= 0 && p.X < map.GetUpperBound(0) + 1 && p.Y < map.GetUpperBound(1) + 1)
                {
                    bool walkable = map[p.X, p.Y];
                    neighbors.Add(new Node(p, walkable));
                }
            }
            return neighbors.ToArray();
        }
    }

    
}
