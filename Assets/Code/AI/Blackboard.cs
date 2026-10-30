using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//
// Util classes
//

delegate T BlackboardGetAccessor<T>();
delegate void BlackboardSetAccessor<T>(T value);

class BlackboardObject<T>
{
    public BlackboardObject(string name, BlackboardGetAccessor<T> get, BlackboardSetAccessor<T> set)
    {
        this.name = name;
        this.get = get;
        this.set = set; 
    }

    // blackboard
    internal BlackboardGetAccessor<T> get;
    internal BlackboardSetAccessor<T> set; 

    public string name; // is this a good identiifer for a blackbord item ?? idk. Dictionaries are just hash tables internally
};

/// <summary>
/// Lets agents access certain variables of oth=er 
/// </summary>
class Blackboard
{
    private Dictionary<string, object> blackboard; 

    public void Add<T>(BlackboardObject<T> blackboardObject)
    {
        if (blackboardObject == null)
        {
            Debug.Log("Blackboard::Add - the object does not exist");
            return; 
        }

        blackboard.Add(blackboardObject.name, blackboardObject);
    }

    public void Remove<T>(BlackboardObject<T> blackboardObject)
    {
        if (blackboardObject == null)
        {
            Debug.Log("Blackboard::Remove - the object does not exist");
            return; 
        }
    
        if (!blackboard.Remove(blackboardObject.name)) // no need to return anything
            Debug.Log("Failed to remove blackboard object with name " + blackboardObject.name + " because it was never added in the first place!");
    }

};