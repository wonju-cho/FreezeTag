using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Node parent;

    public int gCost;
    public int hCost;
    public bool canWalk;
    public Vector3 worldPos;

    public int grid_x;
    public int grid_y;

    int heapIndex;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(bool canWalk, Vector3 worldPos, int grid_x, int grid_y)
    {
        this.canWalk = canWalk;
        this.worldPos = worldPos;
        this.grid_x = grid_x;
        this.grid_y = grid_y;
    }

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

    public int CompareTo(Node nodeToCompare)
    {
        if(fCost < nodeToCompare.fCost)
        {
            return 1;
        }
        else if(fCost == nodeToCompare.fCost)
        {
            if(hCost <= nodeToCompare.hCost)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        
        return -1;
    }
}
