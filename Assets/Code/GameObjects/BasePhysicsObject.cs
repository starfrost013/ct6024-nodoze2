using System;
using UnityEngine;

//
// !Domino
// © 2025-2026
//
internal class BasePhysicsObject : MonoBehaviour
{
    //
    // Unity Rigidbody componetn
    //
    protected Rigidbody thisRigidbody;

    protected void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();

        if (!thisRigidbody)
        {
            Debug.LogWarning("It's probably a good idea to add the rigidbody component to the BasePhysicsObject " + name + ". Adding automatically...");
            thisRigidbody = gameObject.AddComponent<Rigidbody>();
        }


    }


}