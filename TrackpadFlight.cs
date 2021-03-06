﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TrackpadFlight : MonoBehaviour {

    VRTK_ControllerEvents events;
    GameObject rig;
    Rigidbody rb;
    GameObject head;
    PlayerScale scale;
    public float speedMultiplier; // DO NOT SET IN EDITOR

    float speed;
    bool stop;
    int forward;

    // Use this for initialization
    void Start() {
        speedMultiplier = 2;
        rig = GameObject.Find("[CameraRig]");
        rb = rig.GetComponent<Rigidbody>();
        scale = GameObject.Find("RightController").GetComponent<PlayerScale>();
        head = GameObject.Find("Camera(head)");
        events = GetComponent<VRTK_ControllerEvents>();
        events.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
        events.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
    }

    void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
    {
        stop = false;
        StartCoroutine("Fly");
    }
    
    void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        stop = true;
    }

    IEnumerator Fly()
    {
        while (!stop)
        {
            SetForward();
            if (events.touchpadPressed)
            {
                speed = 2 * speedMultiplier;
            }
            else
            {
                speed = 1 * speedMultiplier;
            }
            if (!scale.isCameraSmall) // if regular
            {
                rig.transform.position += transform.forward * Time.deltaTime * forward * speed;
            }
            else // if small
            {
                Vector3 newMoveVector = transform.forward;
                newMoveVector.y = 0;
                newMoveVector.Normalize();
                rig.transform.position += (newMoveVector * speed * forward * Time.deltaTime);
            }
            yield return null;
        }
    }

    void SetForward()
    {
        if (events.GetTouchpadAxis().y > 0)
        {
            forward = 1;
        }
        else forward = -1;
    }
}
