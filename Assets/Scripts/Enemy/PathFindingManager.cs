using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFindingManager : MonoBehaviour
{
    struct PathRequest
    {
        public Vector3 start;
        public Vector3 goal;
        public Action<bool, Vector3[]> callback;
        public PathRequest(Vector3 start, Vector3 goal, Action<bool, Vector3[]> callback)
        {
            this.start = start;
            this.goal = goal;
            this.callback = callback;
        }

    }

    Queue<PathRequest> pathQueue = new Queue<PathRequest>();
    PathRequest currentPath;
    static PathFindingManager instance;
    bool isProcessingPath;
    Pathfinding pathFinding;


    private void Awake()
    {
        instance = this;
        pathFinding = GetComponent<Pathfinding>();
    }

    private void TryProcessNext()
    {
        if(pathQueue.Count > 0 && !isProcessingPath)
        {
            currentPath = pathQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPath.start, currentPath.goal);
        }
    }

    public void EndProcessingPath(bool success, Vector3[] waypoints)
    {
        currentPath.callback(success, waypoints);
        isProcessingPath = false;
        TryProcessNext();
    }

    public static void RequestPath(Vector3 start, Vector3 goal, Action<bool, Vector3[]> callback)
    {
        PathRequest newPath = new PathRequest(start, goal, callback);
        instance.pathQueue.Enqueue(newPath);
        instance.TryProcessNext();
    }

    public static Vector3 RequestFindPath(Vector3 playerPos, Vector3 enemyPos)
    {
        return instance.pathFinding.FollowForNextPath(playerPos, enemyPos);
    }
    
    public static void ChangeNodeWalkable()
    {
        instance.pathFinding.ChangeStepGoalNodeToBeWalkable();
    }

    public static Vector3 NearLookatVectorGrid(Transform pos, Vector3 enemyPos)
    {
        return instance.pathFinding.FindGridLookatVector(pos, enemyPos);
    }

    public static Vector3 FindFrontVectorGrid(Transform pos)
    {
        return instance.pathFinding.FindGridFrontTiles(pos);
    }
}
