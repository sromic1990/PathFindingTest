using Sourav.Utilities.Scripts.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool OnlyDrawPath;
    public Mode Mode; 

    public LayerMask UnwalkableLayer;
    public Vector2 GridWorldSize;
    public float NodeRadius;

    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    void Awake()
    {
        nodeDiameter = NodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(GridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(GridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = Vector3.zero;
        if (Mode == Mode.ThreeDimensonal)
        {
            worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.forward * GridWorldSize.y / 2;
        }
        else
        {
            worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.up * GridWorldSize.y / 2;
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = Vector3.zero;
                bool walkable = true;

                if(Mode == Mode.ThreeDimensonal)
                {
                    worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + NodeRadius) + Vector3.forward * (y * nodeDiameter + NodeRadius);
                    //Debug.Log("worldPoint = "+worldPoint);
                    walkable = !(Physics.CheckSphere(worldPoint, NodeRadius, UnwalkableLayer));
                }
                else
                {
                    worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + NodeRadius) + Vector3.up * (y * nodeDiameter + NodeRadius);
                    //Debug.Log("worldPoint = "+worldPoint);
                    //walkable = !(Physics.CheckSphere(worldPoint, NodeRadius, UnwalkableLayer));
                    walkable = !(Physics2D.OverlapCircle(new Vector2(worldPoint.x, worldPoint.y), NodeRadius, UnwalkableLayer));
                }
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
        Debug.Log("Grid size = "+grid.Length);
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float percentY = 0.0f;
        if(Mode == Mode.ThreeDimensonal)
        {
            percentY = (worldPosition.z + GridWorldSize.y / 2) / GridWorldSize.y;
        }
        else
        {
            percentY = (worldPosition.y + GridWorldSize.y / 2) / GridWorldSize.y;
        }

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbourNodes = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbourNodes.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbourNodes;
    }

    public List<Node> Path;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if(Mode == Mode.ThreeDimensonal)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, 1, GridWorldSize.y));
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.y, 1));
        }

        if(OnlyDrawPath)
        {
            if(Path != null)
            {
                Debug.Log("Path count = "+Path.Count);
                foreach(Node n in Path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.WorldPoint, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else
        {
            if(grid != null)
            {
                foreach(Node n in grid)
                {
                    Gizmos.color = (n.Walkable) ? Color.white : Color.red;
                    if(Path != null)
                    {
                        if(Path.Contains(n))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawCube(n.WorldPoint, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}

public enum Mode
{
    TwoDimensional,
    ThreeDimensonal
}
