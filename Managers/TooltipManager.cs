using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Autelia.Serialization;

public class TooltipManager : MonoBehaviour {

    public delegate void UpdateTooltips();
    public UpdateTooltips updateTooltips;

    List<GameObject> nearestBuildings;
    GameObject headset;
    GameObject rightController;
    Transform stareat;
    [Tooltip("Tooltips spawn from here if no controller present")]
    public GameObject testTooltipObject;

    public static bool pressed;

	// Use this for initialization
	void Start () {if (Serializer.IsDeserializing)	return;if (Serializer.IsLoading)	return;
        headset = GameObject.Find("Headset");
        if(!testTooltipObject)
            testTooltipObject = GameObject.Find("TestSphere");
        rightController = GameObject.Find("RightController");
        rightController.GetComponent<VRTK_ControllerEvents>().ButtonOnePressed += new ControllerInteractionEventHandler(EnableObjectTooltip);
        rightController.GetComponent<VRTK_ControllerEvents>().ButtonOneReleased += new ControllerInteractionEventHandler(DisableObjectTooltip);
    }

    private void Update()
    {
        if (ReferenceManager.instance.tick % 45 == 0 && updateTooltips != null)
        {
            updateTooltips();
        }
    }

    void EnableObjectTooltip(object sender, ControllerInteractionEventArgs e)
    {
        StartTooltips();
    }

    void DisableObjectTooltip(object sender, ControllerInteractionEventArgs e)
    {
        DisableTooltips();
    }

    public void StartTooltips()
    // Used in conjunction with test runner to create tooltips without headset and controllers
    {
        pressed = true;
        if(!testTooltipObject)
        // Used during play
        {
            stareat = rightController.transform;
            nearestBuildings = U.FindNearestBuildings(rightController.transform.position, 10f);
        }
        else
        // For testing
        {
            stareat = testTooltipObject.transform;
            nearestBuildings = U.FindNearestBuildings(testTooltipObject.transform.position, 10f);
        }
        foreach (GameObject building in nearestBuildings)
        {
            EnableTooltips(building);
        }
    }

    void EnableTooltips(GameObject building)
    {
        if(building.tag == "residential")
        {
            building.GetComponent<ResidentialTooltip>().EnableObjectTooltip();
        }
        if(building.tag == "commercial")
        {
            building.GetComponent<CommercialTooltip>().EnableObjectTooltip();
        }
        if (building.tag == "industrial")
        {
            building.GetComponent<IndustrialTooltip>().EnableObjectTooltip();
        }
        if (building.tag == "service")
        {
            building.GetComponent<TooltipBase>().EnableTooltip(stareat);
        }
    }

    public void DisableTooltips()
    // Used with test runner to disable tooltips without controllerss
    {
        pressed = false;
        foreach (GameObject building in nearestBuildings)
        {
            if (building.tag == "residential")
            {
                building.GetComponent<ResidentialTooltip>().DisableObjectTooltip();
            }
            if (building.tag == "commercial")
            {
                building.GetComponent<CommercialTooltip>().DisableObjectTooltip();
            }
            if (building.tag == "industrial")
            {
                building.GetComponent<IndustrialTooltip>().DisableObjectTooltip();
            }
            if (building.tag == "service")
            {
                building.GetComponent<TooltipBase>().DisableTooltip();
            }
        }
    }
}
