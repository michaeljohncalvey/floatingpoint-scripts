using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndustrialTracker : ItemTracker {
    // Manages individual stats of each industrial building.

    [Tooltip("The amount each worker must produce to be happy")]
    public int requiredProduction = 5; // How much each individual must produce to be satisfied
    System.Random rand;

    GameObject markerPrefab;
    Marker marker;
    List<IndustrialComponent> components;
    List<float> sales; // List of recent sales counted in goodsSold
    List<ResidentialTracker> employees;

    public float productionHappiness; // Happiness from reaching sales targets

    public int visitors;
    public int lifetimeVisitors;

    public float goodsCapacity;  // max amount of goods storeable
    public float goodsProduced;  // Goods produced in last economic tick
    public static float allGoods; // Base goods tracking figure for each economic tick

    public float sellPrice;  // Base sell price
    public float sellAmount;  // Base maximum amount sold per economy tick

    // Multipliers from components
    public float productionMulti;
    public float goodsCapacityMulti;
    public float sellPriceMulti;
    public float sellAmountMulti;

    float goodsSold;  // Number of goods sold last week
    bool checkEnable;


    void Awake()
    {
        rand = new System.Random(); //reuse this if generating many

        employees = new List<ResidentialTracker>();
        sales = new List<float>();
        components = new List<IndustrialComponent>();
        goodsSold = 0;
        productionMulti = 1;
        goodsCapacityMulti = 1;
        sellPriceMulti = 1;
        sellAmountMulti = 1;
        StartCoroutine("CheckEnable");
    }

    new void Start()
    {
        base.Start();
        markerPrefab = GameObject.Find("MarkerPrefab");
    }

    void Update()
    {
        if (!updateStarted && usable)
        {
            updateStarted = true;
            EconomyManager.ecoTick += UpdateSecond;
            GameObject.Find("Managers").GetComponent<ItemManager>().addIndustrial(capacity, gameObject);
        }
        else if (updateStarted && !usable)
        {
            updateStarted = false;
            EconomyManager.ecoTick -= UpdateSecond;
        }
        else if (!usable && !updateStarted)
        {
            checkEnable = true;
        }
    }

    void ProduceGoods()
    {
        goodsProduced = users * productionMulti * longtermHappiness / 20;

        income = goodsProduced * sellPrice * sellPriceMulti * (1 + (landValue * 0.01f));
        income -= baseCost;
        allGoods += goodsProduced;
    }

    public void Apply(float applicantLandValue, ResidentialTracker applicantTracker)
    {
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log((float)u1)) *
                     Mathf.Sin(2.0f * Mathf.PI * (float)u2); //random normal(0,1)
        double randNormal =
                     landValue - 5 + 5 * randStdNormal; //random normal(mean,stdDev^2)
        if (usable && users < capacity) // TODO: ADD LAND VALUE CHECKS BACK IN
        {
            AcceptApplication(applicantTracker);
        }
        else RejectApplication(applicantTracker);
    }

    void AcceptApplication(ResidentialTracker applicantTracker)
    {
        if (applicantTracker.unemployedPopulation >= 1)
        {
            AddUsers(1);
            employees.Add(applicantTracker);
            applicantTracker.AcceptApplication();
        }
        else RejectApplication(applicantTracker);
    }

    void RejectApplication(ResidentialTracker applicantTracker)
    {
        // TODO:
        Debug.Log("Applicant rejected!!!");
    }

    public void RemoveAllUsers()
    {
        foreach (ResidentialTracker res in employees)
        {
            res.unemployedPopulation++;
        }
    }

    public void AddMarker()
    {
        marker = Instantiate(markerPrefab, transform.position + new Vector3(0f, 2f, 0f), transform.rotation, transform).GetComponent<Marker>();
        marker.transform.localScale *= 10;
        marker.StartRotation();
    }

    public void LinkComponent(IndustrialComponent component)
    // Sent from component, completes link
    {
        Debug.Log("Gettin linked eh@ " + gameObject.name);
        components.Add(component);
        RecalculateComponents();
    }

    void RecalculateComponents()
    {
        for(int i = 0; i < components.Count; i++)
        {
            if (components[i].type == "sellPrice" || components[i].type == "productionAmount" || components[i].type == "goodsCapacity" || components[i].type == "sellAmount")
            {
                AddBonus(components[i]);
            }
        }
    }

    void AddBonus(IndustrialComponent component)
    {
        sellPriceMulti += component.sellPriceMulti;
        productionMulti += component.productionMulti;
        goodsCapacityMulti += component.goodsCapacityMulti;
        sellAmountMulti += component.sellAmountMulti;
    }

    void UpdateProductionHappiness()
    {
        if (capacity != 0 && requiredProduction != 0)
        {
            productionHappiness = (goodsProduced / capacity * requiredProduction) * 40;
            if (productionHappiness > 40)
            {
                productionHappiness = 40; // Capped at 40
            }
        }
        else productionHappiness = 0;
    }

    void UpdateSecond()
    // Updates values once per second, economic tick
    {
        updateStarted = true;
        if (!usable || !validPosition)
        {
            return;
        }
        UpdateLocalHappiness();
        UpdateProductionHappiness();
        UpdateHappiness();
        UpdateLandValue();
        UpdateTransportationValue();
        ProduceGoods();
        totalIndustrialIncome += income;
    }

    void UpdateHappiness()
    // Performs all necessary final happiness calculations, including longterm
    {
        currentHappiness = localHappiness + productionHappiness + fillRateHappiness;
        CalculateLongtermHappiness();
        CalculateHappinessState();
    }

    public string ValidPosition()
    {
        if (validPosition)
        {
            return "Active";
        }
        else return "Inactive";
    }

    public string FancyIncome()
    {
        return "Income: $" + Mathf.Round(income * 100) / 100 + "/w";
    }

    public string FancyCapacity()
    {
        return "Workers: " + users + " / " + capacity;
    }

    public int FancyHappiness()
    {
        return happinessState;
    }
    
    public string FancyTitle()
    {
        // TODO: NICE TITLES
        return ValidPosition();
    }

    public string FancyGoods()
    {
        return "Goods sold: " + goodsSold.ToString();
    }

    public string FancyLandValue()
    {
        return "Land Value: $" + landValue;
    }
    public void MarkWithComponent()
    {
        StartCoroutine("MarkWithComponent");
    }

    IEnumerator CheckEnable()
    {
        while (true)
        {
            if (checkEnable)
            {
                checkEnable = false;
                if (transform.parent == null)
                {
                    usable = true;
                }
            }
            yield return new WaitForSeconds(5);
        }
    }
}
