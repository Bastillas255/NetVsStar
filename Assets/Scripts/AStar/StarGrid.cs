using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGrid : MonoBehaviour
{
    public bool onlyDisplayPathGizmos;
    public bool displayGridGizmos;
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    StarNode[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
    
    private void CreateGrid()
    {
        grid = new StarNode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y/2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter+nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
                grid[x, y] = new StarNode(walkable,worldPoint,x,y);
            }
        }
    }

    public StarNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x =Mathf.RoundToInt( (gridSizeX - 1) * percentX);
        int y =Mathf.RoundToInt( (gridSizeY - 1) * percentY);

        return grid[x, y];

    }

    //loop throught all the neigboring nodes of the current node
    public List<StarNode> GetNeighbours(StarNode node)
    {
        List<StarNode> neighbours = new List<StarNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x==0&&y==0) || (Mathf.Abs(x + y) != 1))//
                { 
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >=0 && checkX<gridSizeX&& checkY>=0&&checkY<gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;

    }

    public List<StarNode> path;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x, gridWorldSize.y,1));
        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (StarNode n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else
        {
            if (grid != null && displayGridGizmos)
            {
                StarNode playerNode = NodeFromWorldPoint(player.position); //"player" on this context is a*
                Debug.Log("Enemy (X,Y) = " + playerNode.worldPosition);
                foreach (StarNode n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (path != null)
                    {
                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    if (playerNode == n)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }

    }
}
