﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SpawnController : MonoBehaviour {

    SpawnManager spawnManager;  // Reference saved to help spawn in new building on enable of old
    ThumbTracker thumb;

    public int unit;  // Refers to which sphere this is on
    public bool showingBuilding;
    bool selected;  // Used to stop coroutine
    bool selectedStarted;  // Used to tell if coroutine already running- for starting

    GameObject containedBuilding;  // Building currently spawned in sphere. This reference will be used to delete it later on.
    Renderer containedRenderer;  // Used to find size for scale up/ down
    float scaleFactor;
    int containedType;  // contained building type
    int menuSelection;  // What the type should be
    DisplayUI displayUI;

    // Trackers for enabling/ disabling
    ResidentialTracker res;
    CommercialTracker com;
    IndustrialTracker ind;
    CommercialTracker off;
    IndustrialComponent indc;
    FoliageTracker fol;


    private void Start()
    {
        displayUI = GameObject.Find("UI").GetComponent<DisplayUI>();
        thumb = transform.parent.transform.parent.GetComponent<ThumbTracker>();
    }

    private void Update()
    {
        if (selected)
        {
            PerformSelect();
        }
    }

    public GameObject ReturnContainedBuilding()
    {
        return containedBuilding;
    }

    public void EnableBuilding()
    // Call this to spawn the building 
    {
        SizeForPlay();
        DeselectBuilding();
        EnablePhysics();
        if (containedType == 0)
        {
            EnableResidential();
        }
        else if (containedType == 1)
        {
            EnableCommercial();
        }
        else if (containedType == 2)
        {
            EnableIndustrial();
        }
        else if (containedType == 3)
        {
            EnableCommercial();
        }
        else if (containedType == 4)
        {
            EnableComponent();
        }
        else if (containedType == 5)
        {
            EnableFoliage();
        }

        // Spawns new from here!!!!!
        showingBuilding = false;
        spawnManager.SpawnUIBuildings(displayUI.GetSelection(), unit + thumb.angleIncrement);
    }

    public void DeleteBuilding()
    {
        selected = false;
        selectedStarted = false;
        if (showingBuilding)
        {
            Destroy(containedBuilding);
            showingBuilding = false;
        }
    }

    void SelectBuilding()
    // TODO: Display building stats in displayMenu here
    // Start slow building rotation here
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        selected = true;
        PerformSelect();

    }

    void PerformSelect()
    {
        List<string> sendList = new List<string>();
        sendList.Add(FancyType());
        sendList.Add(FancyCapacity());
        sendList.Add(FancyLevel());
        sendList.Add(FancyWeekCost());
        sendList.Add(FancyBuyCost());
        displayUI.SendSelectedText(sendList);
        containedBuilding.transform.Rotate(new Vector3(0, 3f, 0));
    }

    void DeselectBuilding()

    {
        selected = false;
    }

    public void DisableBuilding(SpawnManager sm, GameObject newBuilding)
    {
        if (!spawnManager)
        {
            spawnManager = sm;
        }
        UpdateContainedBuilding(newBuilding);

        DisablePhysics();
        SizeForMenu();

        SetTracker();
        if (unit == 2)
        {
            SelectBuilding();
        }
    }

    public Vector3 GetInstantiatePosition()
    {
        return transform.position - (new Vector3(0f, -0.5f, 0));
    }

    public bool Empty()
    {
        return !showingBuilding;
    }

    void EnableResidential()
    {
        res.usable = true;
    }

    void EnableCommercial()
    {
        com.usable = true;
    }

    void EnableIndustrial()
    {
        ind.usable = true;
    }

    void EnableComponent()
    {
        indc.usable = true;
    }

    void EnableFoliage()
    {
        fol.usable = true;
    }

    void DisablePhysics()
    {
        containedBuilding.transform.parent = this.transform;
        Rigidbody rb = containedBuilding.GetComponent<Rigidbody>();
        containedBuilding.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void EnablePhysics()
    {
        containedBuilding.transform.parent = null;
        Rigidbody rb = containedBuilding.GetComponent<Rigidbody>();
        containedBuilding.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    void UpdateContainedBuilding(GameObject newBuilding)
    {
        menuSelection = displayUI.GetSelection();
        containedBuilding = newBuilding;

        containedRenderer = containedBuilding.GetComponent<Renderer>();
        containedType = menuSelection;
        showingBuilding = true;
    }

    void SetTracker()
    {
        ClearTrackers();
        if (containedType == 0)
        {
            res = containedBuilding.GetComponent<ResidentialTracker>();
            Debug.Log("Res: " + res);
            res.usable = false;
        }
        else if (containedType == 1)
        {
            com = containedBuilding.GetComponent<CommercialTracker>();
            com.usable = false;
        }
        else if (containedType == 2)
        {
            ind = containedBuilding.GetComponent<IndustrialTracker>();
            ind.usable = false;
        }
        else if (containedType == 3)
        {
            off = containedBuilding.GetComponent<CommercialTracker>();
            off.usable = false;
        }
        else if (containedType == 4)
        {
            indc = containedBuilding.GetComponent<IndustrialComponent>();
            indc.usable = false;
        }
        else if (containedType == 5)
        {
            fol = containedBuilding.GetComponent<FoliageTracker>();
            fol.usable = false;
        }
    }

    void ClearTrackers()
    {
        res = null;
        com = null;
        ind = null;
        off = null;
        indc = null;
        fol = null;
    }

    void SizeForMenu()
    {
        Vector3 size = containedRenderer.bounds.size;
        if (size.x > size.y && size.x > size.z)
        {
            // size x 
            scaleFactor = size.x;
        }
        else if (size.y > size.x && size.y > size.z)
        {
            // size y
            scaleFactor = size.y;
        }
        else if (size.z > size.x && size.z > size.y)
        {
            // size z
            scaleFactor = size.z;
        }
        containedBuilding.transform.localScale *= (1 / scaleFactor) * 0.5f;
    }

    void SizeForPlay()
    {
        containedBuilding.transform.localScale *= scaleFactor * 1f / 0.75f;
    }

    string FancyType()
    {
        switch (containedType)
        {
            case 0:
                return "Residential";
            case 1:
                return "Commercial";
            case 2:
                return "Industrial";
            case 3:
                return "Office";
            case 4:
                return "Component";
            case 5:
                return "Foliage";
        }
        return "";
    }

    string FancyCapacity()
    {
        switch (containedType)
        {
            case 0:
                return "Capacity: " + res.capacity;
            case 1:
                return "Capacity: " + com.capacity;
            case 2:
                return "Capacity: " + ind.capacity;
            case 3:
                return "Capacity: " + off.capacity;
            case 4:
                return "Production: " + indc.productionMulti;
            case 5:
                return "";
        }
        return "";
    }

    string FancyLevel()
    {
        switch (containedType)
        {
            case 0:
                return "Level: " + res.level;
            case 1:
                return "Level: " + com.level;
            case 2:
                return "Level: " + ind.level ;
            case 3:
                return "Level: " + off.level;
            case 4:
                return "Level: " + indc.level;
            case 5:
                return "";
        }
        return "";
    }

    string FancyWeekCost()
    {
        switch (containedType)
        {
            case 0:
                return "Weekly cost: " + res.baseCost;
            case 1:
                return "Weekly cost: " + com.baseCost;
            case 2:
                return "Weekly cost: " + ind.baseCost;
            case 3:
                return "Weekly cost: " + off.baseCost;
            case 4:
                return "Weekly cost: " + indc.baseCost;
            case 5:
                return "";
        }
        return "";
    }

    string FancyBuyCost()
    {
        switch (containedType)
        {
            case 0:
                return "Buy Cost: $" + res.buyCost;
            case 1:
                return "Buy Cost: $" + com.buyCost;
            case 2:
                return "Buy Cost: $" + ind.buyCost;
            case 3:
                return "Buy Cost: $" + off.buyCost;
            case 4:
                return "Buy Cost: $" + indc.buyCost;
            case 5:
                return "";
        }
        return "";
    }
}
