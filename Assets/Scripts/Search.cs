using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Search
{
    public class Graph<Location>
    {
        public Dictionary<Location, Location[]> edges = new Dictionary<Location, Location[]>();
        public Location[] Neighbors(Location id)
        {
            return edges[id];
        }
    };

    public class BreadthFirstSearch : MonoBehaviour
    {
        static void Print(ref LinkedList<string> path)
        {
            foreach (string data in path)
            {
                Console.Write($"{data}, ");
            }
        }

        public static LinkedList<int> BreadthSearchNumbers(ref Graph<int> graph, ref int[] obstacleArrayIndex, int start, int goal, int mapSize)
        {
            Queue<int> frontier = new Queue<int>();
            LinkedList<int> path = new LinkedList<int>();
            int current = goal;
            frontier.Enqueue(start);
            int[] cameFrom = new int[mapSize];
            for (int i = 0; i < cameFrom.Length; i++)
            {
                cameFrom[i] = mapSize;
            }
            while (frontier.Any())
            {
                current = frontier.Dequeue();
                if (current == goal)
                {
                    break;
                }
                foreach (int next in graph.Neighbors(current))
                {
                    if (!cameFrom.Contains(next) && !obstacleArrayIndex.Contains(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }
            if (current == goal)
            {
                while (current != start)
                {
                    path.AddLast(current);
                    current = cameFrom[current];
                }
                return path;
            }
            else
            {
                return path;
            }
        }
    }

    public class LinearSearch
    {
        public static int LinearSearchSimple(ref Vector3[] dataArray, Vector3 data)
        {
            for (int index = 0; index < dataArray.Length; index++)
            {
                if (dataArray[index] == data)
                {
                    return index;
                }
            }
            return -1;
        }
    }

    // A* needs only a WeightedGraph and a location type L, and does *not*
    // have to be a grid. However, in the example code I am using a grid.
    public interface IWeightedGraph<L>
    {
        int Cost(Location a, Location b);
        IEnumerable<Location> Neighbors(Location id);
    }


    public struct Location
    {
        // Implementation notes: I am using the default Equals but it can
        // be slow. You'll probably want to override both Equals and
        // GetHashCode in a real project.

        public readonly int x, y;
        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }


    public class SquareGrid : IWeightedGraph<Location>
    {
        // Implementation notes: I made the fields public for convenience,
        // but in a real project you'll probably want to follow standard
        // style and make them private.

        public static readonly Location[] directions = new[]
        {
            new Location(1, 0),
            new Location(0, -1),
            new Location(-1, 0),
            new Location(0, 1)
        };

        public int width, height;
        public HashSet<Location> obstacles = new HashSet<Location>();
        public HashSet<Location> slowDowns = new HashSet<Location>();
        public HashSet<Location> pathing = new HashSet<Location>();
        public HashSet<Location> pickUps = new HashSet<Location>();
        public HashSet<Location> AIPathing = new HashSet<Location>();

        public SquareGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool InBounds(Location id)
        {
            if (0 <= id.x && id.x < width && 0 <= id.y && id.y < height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Passable(Location id)
        {
            if (!obstacles.Contains(id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Cost(Location a, Location b)
        {
            if (slowDowns.Contains(b))
            {
                return 3;
            }
            else
            {
                return 1;
            }
        }

        public IEnumerable<Location> Neighbors(Location id)
        {
            foreach (Location dir in directions)
            {
                Location next = new Location(id.x + dir.x, id.y + dir.y);
                if (InBounds(next) && Passable(next))
                {
                    yield return next;
                }
            }
        }
    }


    public class PriorityQueue<data>
    {
        // I'm using an unsorted array for this example, but ideally this
        // would be a binary heap. There's an open issue for adding a binary
        // heap to the standard C# library: https://github.com/dotnet/corefx/issues/574
        //
        // Until then, find a binary heap class:
        // * https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
        // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
        // * http://xfleury.github.io/graphsearch.html
        // * http://stackoverflow.com/questions/102398/priority-queue-in-net

        private List<Tuple<data, int>> elements = new List<Tuple<data, int>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(data item, int priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public data Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                {
                    bestIndex = i;
                }
            }

            data bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }


    /* NOTE about types: in the main article, in the Python code I just
     * use numbers for costs, heuristics, and priorities. In the C++ code
     * I use a typedef for this, because you might want int or double or
     * another type. In this C# code I use double for costs, heuristics,
     * and priorities. You can use an int if you know your values are
     * always integers, and you can use a smaller size number if you know
     * the values are always small. */

    public class AStarSearch
    {
        public Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();
        public Dictionary<Location, int> costSoFar = new Dictionary<Location, int>();

        // Note: a generic version of A* would abstract over Location and
        // also Heuristic
        static public int Heuristic(Location start, Location goal)
        {
            return Math.Abs(start.x - goal.x) + Math.Abs(start.y - goal.y);
        }

        public LinkedList<Location> AStar(IWeightedGraph<Location> graph, Location start, Location goal)
        {
            PriorityQueue<Location> frontier = new PriorityQueue<Location>();
            LinkedList<Location> path = new LinkedList<Location>();
            frontier.Enqueue(start, 0);
            Location current = goal;
            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();

                if (current.Equals(goal))
                {
                    break;
                }

                foreach (Location next in graph.Neighbors(current))
                {
                    int newCost = costSoFar[current] + graph.Cost(current, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        int priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }
            if (current.Equals(goal))
            {
                while (!(current.Equals(start)))
                {
                    path.AddLast(current);
                    current = cameFrom[current];
                }
                return path;
            }
            else
            {
                return path;
            }
        }
    }
}