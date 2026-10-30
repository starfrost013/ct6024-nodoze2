//
// THe vision sensor
// It allows our guy to see when he is going to curbstomp you beyond death (10/30/26)
//


using System.Collections.Generic;
using UnityEngine;

class VisionSensorResult : SensorResult
{
    public bool canSeePosition = false;
};

class VisionSensorData : SensorData // separation of concerns
{
    /// <summary>
    /// the position we are seeing from
    /// </summary>
    public Vector3 currentPosition;

    /// <summary>
    /// the position we are trying to see
    /// </summary>
    public Vector3 targetPosition;   

    /// <summary>
    /// A list of wanted raycast positions for the player to set
    /// </summary>
    private List<Vector3> raycastPositions = new();
    public float basicVisionRadius;

    /// <summary>
    /// the view angle of the sensor
    /// </summary>
    public float viewAngle; 

    public void AddRaycastPosition(Vector3 pos) { raycastPositions.Add(pos); }
    public void RemoveRaycastPosition(Vector3 pos) {raycastPositions.Remove(pos); } 

};

class VisionSensor : Sensor
{

    public VisionSensor(ref Agent agent) : base(ref agent) { }
    
    // this method is hidden
    public VisionSensorResult Sense(VisionSensorData data) 
    { 
        VisionSensorResult result = new();
        RaycastHit hit = new();

        Vector3 deltaPosition = data.targetPosition - data.currentPosition;

        if (deltaPosition.magnitude > data.basicVisionRadius)
            goto done;

        // measure the angle between the player and the enemy
        if (Vector3.Angle(transform.forward, deltaPosition) > data.viewAngle)
            goto done;

        // I wonder how slow this is
        if (Physics.Raycast(transform.position, deltaPosition, out hit, data.basicVisionRadius))
        {
            // use the target position, which is filled in from the position of the agnet that it is targeting
            if (hit.transform.root.position != data.targetPosition) 
                goto done;
        }

        // succesfully completed
        result.canSeePosition = true; 
        
    done:
        return result;
    }
}; 