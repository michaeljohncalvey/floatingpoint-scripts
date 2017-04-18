﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappinessAffector : MonoBehaviour {

    public float radius;
    public float affectAmount;
    public string affectType;

	// Use this for initialization
	void Start () {
        EconomyManager.ecoTick += StartAffect;	
	}

    public void StartAffect()
    {
        List<GameObject> surroundingBuildings = U.FindNearestBuildings(transform.position, radius);
        //Debug.Log("Happiness affector surrounding buildings: " + surroundingBuildings.Count);

        foreach (GameObject building in surroundingBuildings)
        {
            if (building.tag == "residential")
            {
                if (affectType == "industrialReduce")
                {
                    building.GetComponent<ResidentialTracker>().ModifyHappiness(affectAmount, "industrialReduce");
                }
                else
                {
                    building.GetComponent<ResidentialTracker>().ModifyHappiness(affectAmount, "");
                }
            }
            else if (building.tag == "industrial")
            {
                //Debug.Log(building);
                building.GetComponent<IndustrialTracker>().ModifyHappiness(affectAmount, "");
            }
            else if (building.tag == "commercial")
            {
                building.GetComponent<CommercialTracker>().ModifyHappiness(affectAmount, "");
            }
            else if (building.tag == "leisure")
            {
                building.GetComponent<LeisureTracker>().ModifyHappiness(affectAmount, "");
            }
        }
    }
}