﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class ResidentialTooltip : MonoBehaviour
{

    GameObject tooltip;
    ResidentialTracker residentialTracker;
    bool referencesUpdated;
    Transform stareat;

    int happiness; // temporary value used to check if update required

    // Text references
    Text titleText;
    Text incomeText;
    Text capacityText;
    Text happinessText;
    Text landValueText;

    // Happiness sprites
    SpriteRenderer dead;
    SpriteRenderer happy;
    SpriteRenderer veryHappy;
    SpriteRenderer passive;
    SpriteRenderer angry;

    // Use this for initialization
    void Start()
    {
        residentialTracker = GetComponent<ResidentialTracker>();
        referencesUpdated = false;
        stareat = GameObject.Find("Camera (eye)").transform;
    }

    public void UpdateValues()
    {
        if (referencesUpdated == false && TooltipManager.pressed == true)
        {
            UpdateReferences();
        }
        else if (referencesUpdated == true && TooltipManager.pressed == true)
        {
            UpdateText();
            UpdateHappiness();
        }
    }

    void UpdateText()
    {
        titleText.text = residentialTracker.FancyTitle();
        incomeText.text = residentialTracker.FancyIncome();
        capacityText.text = residentialTracker.FancyCapacity();
        landValueText.text = residentialTracker.FancyLandValue();
    }

    void UpdateHappiness()
    {
        int newHappiness = residentialTracker.FancyHappiness();
        if (newHappiness != happiness)
        {
            SetHappiness(newHappiness);
        }
    }

    void SetHappiness(int newHappiness)
    {
        happiness = newHappiness;
        if (happiness == 0)
        {
            DisableSprites();
            dead.enabled = true;
        }
        else if (happiness == 1)
        {
            DisableSprites();
            angry.enabled = true;
        }
        else if (happiness == 2)
        {
            DisableSprites();
            passive.enabled = true;
        }
        else if (happiness == 3)
        {
            DisableSprites();
            happy.enabled = true;
        }
        else if (happiness == 4)
        {
            DisableSprites();
            veryHappy.enabled = true;
        }
    }

    void DisableSprites()
    {
        dead.enabled = false;
        happy.enabled = false;
        veryHappy.enabled = false;
        passive.enabled = false;
        angry.enabled = false;
    }

    void UpdateReferences()
    {
        titleText = tooltip.transform.Find("Canvas/TitleText").GetComponent<Text>();
        incomeText = tooltip.transform.Find("Canvas/IncomeText").GetComponent<Text>();
        capacityText = tooltip.transform.Find("Canvas/CapacityText").GetComponent<Text>();
        landValueText = tooltip.transform.Find("Canvas/LandValueText").GetComponent<Text>();

        dead = tooltip.transform.Find("Canvas/Icons/Dead").GetComponent<SpriteRenderer>();
        happy = tooltip.transform.Find("Canvas/Icons/Happy").GetComponent<SpriteRenderer>();
        angry = tooltip.transform.Find("Canvas/Icons/Angry").GetComponent<SpriteRenderer>();
        passive = tooltip.transform.Find("Canvas/Icons/Passive").GetComponent<SpriteRenderer>();
        veryHappy = tooltip.transform.Find("Canvas/Icons/VeryHappy").GetComponent<SpriteRenderer>();

        referencesUpdated = true;
    }

    public void EnableObjectTooltip()
    // Enables, resets position and resets text for object tooltips
    {
        Debug.Log("Residential tooltip attempting to enable");
        if (tooltip != null)
        {
            Destroy(tooltip);
        }
        tooltip = Instantiate(GameObject.Find("ResidentialTooltip"), gameObject.transform);

        tooltip.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        tooltip.transform.position = gameObject.transform.position + new Vector3(0f, 2.5f, 0f);
        tooltip.transform.localScale = new Vector3(10f, 10f, 10f);
        tooltip.transform.LookAt(2 * transform.position - stareat.position);
        GameObject.Find("Managers").GetComponent<TooltipManager>().updateTooltips += UpdateValues;
        UpdateReferences();
        UpdateText();
    }

    public void DisableObjectTooltip()
    // Removes object tooltips
    {
        GameObject.Find("Managers").GetComponent<TooltipManager>().updateTooltips -= UpdateValues;
        Destroy(tooltip.gameObject);
        referencesUpdated = false;
    }

}
