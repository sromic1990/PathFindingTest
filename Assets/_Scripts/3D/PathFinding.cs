using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Sourav.Utilities.Scripts.DataStructures;

[RequireComponent(typeof(Grid))]
public class PathFinding : MonoBehaviour
{
    public Transform Seeker;
    public Transform Target;

    private Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        FindPath(Seeker.position, Target.position);
        //if(Input.GetButtonDown("Jump"))
        //{
        //}
    }

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start(); 
        Node startNode = grid.GetNodeFromWorldPosition(startPos);
        Node endNode = grid.GetNodeFromWorldPosition(endPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirstItem();
            closedSet.Add(currentNode);

            if(currentNode == endNode)
            {
                sw.Stop();
                Debug.Log("Path found in = "+sw.ElapsedMilliseconds+" ms");
                RetracePath(startNode, endNode);
                return;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, endNode);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> Path = new List<Node>();
        Node currentNode = endNode;
        while(currentNode != startNode)
        {
            Path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Path.Reverse();

        grid.Path = Path;
    }

    int GetDistance(Node NodeA, Node NodeB)
    {
        int distanceX = Mathf.Abs(NodeA.GridX - NodeB.GridX);
        int distanceY = Mathf.Abs(NodeA.GridY - NodeB.GridY);

        if(distanceX > distanceY)
        {
            return (14 * distanceY + 10 * (distanceX - distanceY));
        }
        else
        {
            return (14 * distanceX + 10 * (distanceY - distanceX));
        }
    }
}
