using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarNode : IHeapItem<StarNode>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public StarNode parent;
    int heapIndex;


    public StarNode(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;

    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    //add-on of interface in heap, seen on "A* Pathfinding (E04: heap optimization)" by Sebastian Lague (3Dw5d7PlcTM)
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(StarNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare==0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
