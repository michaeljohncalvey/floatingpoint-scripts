﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeTeleport : MonoBehaviour
{

    public bool atOffice;
    public GameObject interiorTarget;
    public GameObject exteriorTarget;

    private void Start()
    {
        atOffice = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "[MayorsOffice]")
        {
            atOffice = true;
            Vector3 difference = interiorTarget.transform.position - gameObject.transform.position;
            GameObject.Find("[CameraRig]").transform.position += new Vector3(difference.x, -110f, difference.z);
        }
        else if (other.gameObject.name == "OfficeDoor")
        {
            atOffice = false;
            Vector3 difference = exteriorTarget.transform.position - gameObject.transform.position;
            GameObject.Find("[CameraRig]").transform.position += new Vector3(difference.x, 110f, difference.z);
        }
    }
}
