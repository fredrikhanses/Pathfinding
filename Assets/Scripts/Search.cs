using System.Collections.Generic;
using System;

namespace Search
{
    public interface IWeightedGraph<L>
    {
        int Cost(Location a, Location b);
        IEnumerable<Location> Neighbors(Location id);
    }

    public struct Location
    {
        public readonly int x, y;
        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class SquareGrid : IWeightedGraph<Location>
    {
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

    public class AStarSearch
    {
        public Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();
        public Dictionary<Location, int> costSoFar = new Dictionary<Location, int>();

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