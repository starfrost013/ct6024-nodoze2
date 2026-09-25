using System;
using UnityEngine;

//
// ND2 (Nodoze-2) UoG
// Version 1.0 (CT5010): © 2025-2026 NotDominoes Team 
// Version 2.0 (CT6024): © 2025-2027 Connor Hyde (starfrost)
//
// BasePhysicsObject.cs

/// <summary>
/// THis class just ensures that all basephysicsobjects have rigidbody components
/// </summary>
internal class BasePhysicsObject : MonoBehaviour
{
    /// <summary>
    /// Unity Rigidbody componen for car physics
    /// </summary>
    protected Rigidbody physRigidbody;

    protected void Start()
    {
        physRigidbody = GetComponent<Rigidbody>();

        if (!physRigidbody)
        {
            Debug.LogWarning("It's probably a good idea to add the rigidbody component to the BasePhysicsObject " + name + ". Adding automatically...");
            physRigidbody = gameObject.AddComponent<Rigidbody>();
        }
    }
}