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
                //Debug.Log($"Dequeue {current}");
                Console.WriteLine($"Current: {current}, ");
                if (current == goal)
                {
                    break;
                }
                foreach (int next in graph.Neighbors(current))
                {
                    //Debug.Log(cameFrom.Contains(0));
                    //Debug.Log($"{cameFrom[0]}");
                    if (!cameFrom.Contains(next) && !obstacleArrayIndex.Contains(next))
                    {
                        frontier.Enqueue(next);
                        //Debug.Log($"Enqueue {next}");
                        cameFrom[next] = current;
                        //Debug.Log($"{cameFrom[next]}");
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
                //path.AddLast(start);
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
                //Console.Write($"{dataArray[index]}, ");
            }
            //Console.WriteLine();
            return -1;
        }
    }
}