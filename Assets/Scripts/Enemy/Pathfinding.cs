using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    PathFindingManager manager;
    Vector3 stepPath;
    Node step_goal_node;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        manager = GetComponent<PathFindingManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 goalPos)
    {
        StartCoroutine(ComputePath(startPos, goalPos));

    }

    public Vector3 FollowForNextPath(Vector3 playerPos, Vector3 enemyPos)
    {
        if(FindPathUsingAStar(playerPos, enemyPos))
        {
            return stepPath;
        }

        return enemyPos;//do not move
    }

    public bool FindPathUsingAStar(Vector3 playerPos, Vector3 enemyPos)
    {
        Node start_node = grid.GetNodePosFromWorldPosition(playerPos);
        Node goal_node = grid.GetNodePosFromWorldPosition(enemyPos);

        Heap<Node> openList = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closeList = new HashSet<Node>();

        //Push Start node onto the Open List.
        openList.Add(start_node);

        Node parent_node;

        //Loop
        while (openList.Count > 0)
        {
            parent_node = openList.RemoveFirst();

            //if node == goal -> path finding.
            if (parent_node == goal_node)
            {
                stepPath = goal_node.parent.worldPos;
                step_goal_node = goal_node.parent;

                return true;
            }

            int currCost;

            foreach (Node child in grid.GetNeighborNodes(parent_node))
            {
                if (closeList.Contains(child) || !child.canWalk)
                {
                    continue;
                }

                currCost = parent_node.gCost + GetDistance(parent_node, child);

                if (currCost < child.gCost || !openList.Contains(child))
                {
                    child.gCost = currCost;
                    child.hCost = GetDistance(child, goal_node);
                    child.parent = parent_node;

                    if (!openList.Contains(child))
                    {
                        openList.Add(child);
                    }
                }
            }

            closeList.Add(parent_node);
        }
        return false;
    }

    public void ChangeStepGoalNodeToBeWalkable()
    {
        step_goal_node.canWalk = true;
    }

    public IEnumerator ComputePath(Vector3 startPos, Vector3 goalPos)
    {
        bool isSuccessFinding = false;
        Vector3[] waypoints = new Vector3[0];

        Node start_node = grid.GetNodePosFromWorldPosition(startPos);
        Node goal_node = grid.GetNodePosFromWorldPosition(goalPos);

        if(start_node.canWalk && goal_node.canWalk)
        {
            Heap<Node> openList = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closeList = new HashSet<Node>();

            //Push Start node onto the Open List.
            openList.Add(start_node);

            Node parent_node;

            //Loop
            while (openList.Count > 0)
            {
                parent_node = openList.RemoveFirst();

                //if node == goal -> path finding.
                if (parent_node == goal_node)
                {
                    isSuccessFinding = true;
                    break;
                }

                int currCost;

                foreach (Node child in grid.GetNeighborNodes(parent_node))
                {
                    if (closeList.Contains(child) || !child.canWalk)
                    {
                        continue;
                    }

                    currCost = parent_node.gCost + GetDistance(parent_node, child);

                    if (currCost < child.gCost || !openList.Contains(child))
                    {
                        child.gCost = currCost;
                        child.hCost = GetDistance(child, goal_node);
                        child.parent = parent_node;

                        if (!openList.Contains(child))
                        {
                            openList.Add(child);
                        }
                    }
                }

                closeList.Add(parent_node);
            }
        }

        yield return null;

        if(isSuccessFinding)
        {
            waypoints = TrackPath(start_node, goal_node);
        }

        manager.EndProcessingPath(isSuccessFinding, waypoints);
    }

    private int GetDistance(Node startNode, Node goalNode)
    {
        int distance;

        int x = Mathf.Abs(startNode.grid_x - goalNode.grid_x);
        int y = Mathf.Abs(startNode.grid_y - goalNode.grid_y);

        if(x < y)
        {
            distance = 14 * x + 10 * (y - x);
        }
        else
        {
            distance = 14 * y + 10 * (x - y);
        }

        return distance;
    }

    private Vector3 FindNextStepPath(Node startNode, Node goalNode)
    {
        List<Node> path = new List<Node>();
        Node curr_node = goalNode;

        while (curr_node != startNode)
        {
            path.Add(curr_node);
            curr_node = curr_node.parent;
        }

        return path[1].worldPos;
    }

    private void TrackPathWithNotReverse(Node start, Node goal)
    {
        List<Node> path = new List<Node>();
        Node curr_node = goal;

        while (curr_node != start)
        {
            path.Add(curr_node);
            curr_node = curr_node.parent;
        }

        grid.path = path;
    }

    private Vector3[] TrackPath(Node startNode, Node goalNode)
    {
        List<Node> path = new List<Node>();
        Node curr_node = goalNode;

        while(curr_node != startNode)
        {
            path.Add(curr_node);
            curr_node = curr_node.parent;
        }

        Vector3[] simplePath = SimplifyPath(path);
        Array.Reverse(simplePath);

        return simplePath;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();

        Vector2 direction_old = Vector2.zero;
        Vector2 direction_new = Vector2.zero;

        for (int i = 0; i < path.Count - 1; ++i)
        {
            direction_new = new Vector2(path[i].grid_x - path[i + 1].grid_x, path[i].grid_y - path[i + 1].grid_y);

            if (direction_new != direction_old)
            {
                wayPoints.Add(path[i + 1].worldPos);
                direction_old = direction_new;
            }
        }

        return wayPoints.ToArray();
    }

    public Vector3 FindGridLookatVector(Transform pos, Vector3 enemyPos)
    {

        Vector3 look_vector = pos.forward - pos.position;
        look_vector.Normalize();

        Vector3 grid_position = pos.position + pos.forward * grid.nodeRadius*2;
        Node grid_node = grid.GetNodePosFromWorldPosition(grid_position);


        Vector3 result_position = 2 * (grid_position - enemyPos);
        Node result_grid_node = grid.GetNodePosFromWorldPosition(result_position);

        if (grid_node != null)
        {
            if (!grid_node.canWalk)
            {
                //grid which is looked at is not available to walk
                return pos.position;
            }
            else
            {
                //can walk to front grid
                if(result_grid_node != null)
                {
                    if(!result_grid_node.canWalk)
                    {
                        //not walk
                        return grid_position;
                    }
                }

                return pos.position;
            }
        }

        return pos.position;

    }

    public Vector3 FindGridFrontTiles(Transform pos)
    {
        Vector3 look_vector = pos.forward - pos.position;
        look_vector.Normalize();

        Vector3 result_position = pos.position;
        Node grid_node = null;

        for(int i = 4; i> 0;--i)
        {
            result_position = pos.position + pos.forward * grid.nodeRadius *2* i;
            grid_node = grid.GetNodePosFromWorldPosition(result_position);

            if(grid_node != null)
            {
                if (grid_node.canWalk)
                {
                    return result_position;
                }
            }
            
        }

        return result_position;
    }
}
