using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndustrialTracker : ItemTracker {
    // Manages individual stats of each industrial building.

    GameObject markerPrefab;
    Marker marker;
    List<IndustrialComponent> components;

    public int visitors;
    public int lifetimeVisitors;

    public float goodsCapacity;  // obvious
    public float goodsProduced;  // Goods produced in last economic tick
    public float goodsOwned;  // All goods currently owned by this 
    public static float allGoods; // Base goods tracking figure for each economic tick

    public float productionAmount;  // Production multiplier, used for components
    public float sellPrice;  // Sales price for goods
    public float sellAmount;  // Amount sold per economy tick

    int sellPriceComponents;
    int productionAmountComponents;
    int goodsCapacityComponents;
    int sellAmountComponents;

    void Awake()
    {
        markerPrefab = GameObject.Find("MarkerPrefab");
        components = new List<IndustrialComponent>();
        EconomyManager.ecoTick += UpdateSecond;
    }

    void Update()
    {
        if(!updateStarted)
        {
            StartCoroutine("UpdateSecond");  // Economic update tick
        }
    }

    void ProduceGoods()
    {
        if(goodsOwned < goodsCapacity)
        {
            goodsProduced = users * localHappiness * productionAmount;
            if(goodsProduced + goodsOwned <= goodsCapacity)
            {
                goodsOwned += goodsProduced;
            }
            else
            {
                goodsProduced = goodsCapacity - goodsOwned;
                goodsOwned += goodsProduced;
            }
        }
    }

    public void Apply(float applicantLandValue, int residentID, ResidentialTracker applicantTracker)
    {
        System.Random rand = new System.Random(); //reuse this if generating many
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log((float)u1)) *
                     Mathf.Sin(2.0f * Mathf.PI * (float)u2); //random normal(0,1)
        double randNormal =
                     landValue - 5 + 5 * randStdNormal; //random normal(mean,stdDev^2)
        if (applicantLandValue > randNormal && usable && users < capacity)
        {
            AcceptApplication(residentID, applicantTracker);
        }
        else RejectApplication(residentID, applicantTracker);
    }

    void AcceptApplication(int residentID, ResidentialTracker applicantTracker)
    {
        if (!applicantTracker.IsEmployed(residentID))
        {
            AddUsers(1);
            applicantTracker.AcceptApplication(residentID);
        }
        else RejectApplication(residentID, applicantTracker);
    }

    void RejectApplication(int residentID, ResidentialTracker applicantTracker)
    {
        // TODO:
    }

    void SellGoods()
    // Calculates income from goods sale
    {
        if(goodsOwned > sellAmount)
        {
            goodsOwned -= sellAmount;
            allGoods += sellAmount;
        }
        else
        {
            allGoods += goodsOwned;
            goodsOwned = 0;
        }
        income = goodsProduced * sellPrice;
        income -= baseCost;
        totalIndustrialIncome += goodsProduced;
    }

    public void AddMarker()
    {
        marker = Instantiate(markerPrefab, transform.position + new Vector3(0f, 2f, 0f), transform.rotation, transform).GetComponent<Marker>();
        marker.StartRotation();
    }

    public void LinkComponent(IndustrialComponent component)
    // Sent from component, completes link
    {
        components.Add(component);
        RecalculateComponents();
    }

    void RecalculateComponents()
    {
        for(int i = 0; i < components.Count; i++)
        {
            if (components[i].type == "sellPrice" || components[i].type == "productionAmount" || components[i].type == "goodsCapacity" || components[i].type == "sellAmount" || components[i].type == "capacity")
            {
                AddBonus(components[i]);
            }
        }
    }

    void AddBonus(IndustrialComponent component)
    {
        sellPrice += component.sellPrice;
        productionAmount += component.productionAmount;
        goodsCapacity += component.goodsCapacity;
        sellAmount += component.sellAmount;
        capacity += component.capacity;

    }

    void UpdateSecond()
    // Updates values once per second, economic tick
    {
        updateStarted = true;
        if (!usable || !validPosition)
        {
            return;
        }
        UpdateHappiness();
        //UpdateLandValue();
        UpdateTransportationValue();
        ProduceGoods();
        SellGoods();
        totalIndustrialIncome += income;
    }
}
