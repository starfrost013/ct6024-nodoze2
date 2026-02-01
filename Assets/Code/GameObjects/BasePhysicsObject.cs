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