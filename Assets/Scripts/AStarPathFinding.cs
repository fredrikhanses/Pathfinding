using System.Collections.Generic;
using UnityEngine;

namespace AStarLogic
{
    public class Node
    {
        public readonly int positionX;
        public readonly int positionY;
        public int cost;
        public int goalEstimate;
        public int oldPositionX;
        public int oldPositionY;

        public Node(int positionX, int positionY, int oldPositionX, int oldPositionY, int cost, int goalEstimate)
        {
            this.positionX = positionX;
            this.positionY = positionY;
            this.oldPositionX = oldPositionX;
            this.oldPositionY = oldPositionY;
            this.cost = cost;
            this.goalEstimate = goalEstimate; 
        }
    }


    public class AStarPathFinding : MonoBehaviour
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        public int startPositionX = -2;
        public int startPositionY = 4;

        int costToMove = 10;
        int costToMoveDiagonally = 14;

        private void Start()
        {

        }

        private void MoveFromTo()
        {
            Node startingPosition = new Node(startPositionX, startPositionY, 0, 0, 0, 0);
            openList.Add(startingPosition);
            bool finished = false;

            while(!finished && openList.Count > 0)
            {

            }
        }

        private int Distance(ref Node current, ref Node goal)
        {
            int deltaX = Mathf.Abs(current.positionX - goal.positionX);
            int deltaY = Mathf.Abs(current.positionY - goal.positionY);
            return costToMove * (deltaX + deltaY) + (costToMoveDiagonally - 2 * costToMove) * Mathf.Min(deltaX, deltaY);
        }

        private int MoveCost(ref Node current, ref Node goal)
        {
            int deltaX = Mathf.Abs(current.positionX - goal.positionX);
            int deltaY = Mathf.Abs(current.positionY - goal.positionY);
            if((deltaX + deltaY) == 2)
            {
                return 14;
            }
            if((deltaX + deltaY) == 1)
            {
                return 10;
            }
            return 0;
        }
    }
}

