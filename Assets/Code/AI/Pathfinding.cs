using System.IO;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Pathfinding
/// 
/// The pathfinding class
/// </summary>



static class Pathfinding
{
    public static bool CanWeReachSomewhere(Vector3 start, Vector3 target)
    {
        NavMeshPath path = new();

        bool found = NavMesh.CalculatePath(start, target, NavMesh.AllAreas, path);

        if (found)
        {
            if (path.status == NavMeshPathStatus.PathComplete)
                return false;
        }

        return true; 
    }

    public static NavMeshPath CalculatePath(Vector3 start, Vector3 target, int areaMask = NavMesh.AllAreas)
    {
        NavMeshPath path = new();
        bool found = NavMesh.CalculatePath(start, target, areaMask, path);

        if (!found)
            return null;
        else
            return path;
    }
}