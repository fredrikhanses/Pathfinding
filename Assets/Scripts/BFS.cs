using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

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