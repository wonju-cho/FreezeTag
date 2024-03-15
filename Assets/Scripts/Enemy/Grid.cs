using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool GizmosMode = false;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask wallMask;
    Node[,] grid;

    int grid_x, grid_y;
    float nodeDiameter;
    public List<Node> path;

    public int MaxSize
    {
        get
        {
            return grid_x * grid_y;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null && GizmosMode == true)
        {
            foreach(Node node in grid)
            {
                if(node.canWalk == true)
                {
                    Gizmos.color = Color.white; 
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                if(path!=null)
                {
                    if(path.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }
                }

                Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        grid_x = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        grid_y = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrids();
    }

    private void CreateGrids()
    {
        Vector3 bottom_left = this.transform.position - (Vector3.forward * gridWorldSize.y / 2) - (Vector3.right * gridWorldSize.x / 2); // x,z
        Vector3 pos;
        Vector3 y_gap = Vector3.forward * nodeDiameter;
        bool canWalk = true;

        grid = new Node[grid_x, grid_y];

        for(int x = 0; x< grid_x; ++x)
        {
            pos = bottom_left + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * nodeRadius;

            for(int y = 0; y< grid_y; ++y)
            {
                if (Physics.CheckSphere(pos, nodeRadius, wallMask))
                {
                    canWalk = false;
                }
                else
                {
                    canWalk = true;
                }

                grid[x, y] = new Node(canWalk, pos, x, y);

                pos += y_gap;
            }
        }

    }

    public Node GetNodePosFromWorldPosition(Vector3 worldPos)
    {
        //from the position of the bottom_left to 0,0 position
        float positive_x = worldPos.x + gridWorldSize.x / 2; 
        float positive_y = worldPos.z + gridWorldSize.y / 2;

        //worldPos : gridWorldSize = "want to know the NodePos" : gridSize
        int x = Mathf.RoundToInt((grid_x - 1) * positive_x / gridWorldSize.x);
        int y = Mathf.RoundToInt((grid_y - 1) * positive_y / gridWorldSize.y);

        if(x<0 || x>=grid_x || y<0 || y>=grid_y)
        {
            return null;
        }
        return grid[x, y];
    }

    public List<Node> GetNeighborNodes(Node node)
    {
        List<Node> neighbor_nodes = new List<Node>();

        int curr_x;
        int curr_y;

        for(int x = -1; x <=1; ++x)
        {
            for(int y = -1; y <= 1;++y)
            {
                //check without parent node.
                if (x == 0 && y == 0)
                    continue;

                curr_x = node.grid_x + x;
                curr_y = node.grid_y + y;


                //check inside the boundary
                if(curr_x >= 0 && curr_x < grid_x)
                {
                    if(curr_y >= 0 && curr_y <grid_y)
                    {
                        //check grid corner is safe
                        if (x == 1)
                        {
                            if (y == 1)
                            {
                                //have to check 0,1 & 1,0
                                if (!grid[curr_x, curr_y - 1].canWalk || !grid[curr_x - 1, curr_y].canWalk)
                                {
                                    continue;
                                }
                            }
                            else if (y == -1)
                            {
                                //have to check 1,0 & 0,-1
                                if (!grid[curr_x, curr_y + 1].canWalk || !grid[curr_x - 1, curr_y].canWalk)
                                {
                                    continue;
                                }
                            }
                        }
                        else if (x == -1)
                        {
                            if (y == 1)
                            {
                                //have to check 0,1 & -1,0
                                if (!grid[curr_x, curr_y - 1].canWalk || !grid[curr_x + 1, curr_y].canWalk)
                                {
                                    continue;
                                }
                            }
                            else if (y == -1)
                            {
                                //have to check -1,0 & 0,-1
                                if (!grid[curr_x, curr_y + 1].canWalk || !grid[curr_x + 1, curr_y].canWalk)
                                {
                                    continue;
                                }
                            }
                        }

                        neighbor_nodes.Add(grid[curr_x, curr_y]);
                    }
                }
            }
        }

        return neighbor_nodes;
    }


}
