using UnityEngine;

abstract class SensorResult { }; // abstratc class for result 
abstract class SensorData { }; // abstract class for input

/// <summary>
/// Sensor (October 1, 2026)
/// 
/// A sensor is a sub-type of an agent which has the ability to sense the world in some way. For example 
/// 
/// Maybe later on this will be expanded into an environmental query system. But this seems unnecessary.
/// </summary>
class Sensor : MonoBehaviour
{
    /// <summary>
    /// attached agent
    /// </summary>
    private Agent agent;

    public Sensor(ref Agent agent)
    {
        this.agent = agent; // reference
    } 

    public virtual void Init()
    {
        
    }

    public virtual SensorResult Sense(SensorData data) 
    { 
        Debug.LogAssertion("Sensor::Sense must be overloaded or overridden!");
        return null; 
    } 
}