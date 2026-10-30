using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

/// <summary>
/// AI Manager
/// Orchestrates and connects all GOAP / Behaviour Tree agent systems
/// </summary>

static class AIManager
{
    /// <summary>
    /// registered by the agent when it is created.
    /// This list is always accessed *by reference*, as c# uses reference types by default: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types
    /// </summary>
    private static List<Agent> agents = new(); 

    public static Blackboard blackboard; 
    
    //
    // METHODS
    //

    public static bool Add(Agent agent)
    {
        if (agent == null)
        {
            Debug.LogError("AIManager::Add had a null agent ?!");
            goto error; 
        }

        agents.Add(agent);

        Debug.Log("AI Manager registered a new agent " + agent.name);
        return true; 
    error:
        return false; 
    }

    public static bool Remove(Agent agent)
    {
        if (agent == null)
        {
            Debug.LogError("AIManager::Remove: passed a null agent ?!");
            goto error;
        }

        if (!agents.Remove(agent))
        {
            Debug.LogError("AIManager::Remove: agent was not actually in the agent list");
            goto error; 
        }

        Debug.Log("AI Manager removed the agent " + agent.name);
        return true; 
    error:
        return false;        
    }

    
};