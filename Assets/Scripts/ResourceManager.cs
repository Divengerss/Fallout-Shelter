using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Resources")]
    public int Power = 50;
    public int Food = 50;
    public int Water = 50;
    public int Tomatoes = 0; 
    public int Caps = 500;

    [Header("Limits")]
    public int MaxPower = 100;
    public int MaxFood = 100;
    public int MaxWater = 100;
    public int MaxTomatoes = 50; 

    public event Action OnResourcesChanged;

    [Header("Consommation")]

    public float ConsumptionInterval = 20.0f; 
    public int DwellerPowerConsumption = 1; 
    public int DwellerFoodConsumption = 1; 
    public int DwellerWaterConsumption = 1; 
    public int RoomPowerConsumption = 1;    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (OnResourcesChanged != null) OnResourcesChanged.Invoke();

        StartCoroutine(ConsumptionRoutine());
    }

    private void Update()
    {
        if (Food <= 0 || Water <= 0)
        {
            Debug.LogError("GAME OVER ! (Plus de nourriture ou d'eau)");
        }
    }

    private System.Collections.IEnumerator ConsumptionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(ConsumptionInterval);

            CalculateConsumption();
        }
    }

    private void CalculateConsumption()
    {
        int powerCost = 0;
        int foodCost = 0;
        int waterCost = 0;

        if (DwellerManager.Instance != null)
        {
            Dweller[] allDwellers = FindObjectsByType<Dweller>(FindObjectsSortMode.None);
            int count = allDwellers.Length;

            foodCost += count * DwellerFoodConsumption;
            waterCost += count * DwellerWaterConsumption;
        }

        if (RoomManager.Instance != null)
        {
            powerCost += RoomManager.Instance.AllRooms.Count * RoomPowerConsumption;
        }

        if (powerCost > 0)
        {
            Power = Mathf.Clamp(Power - powerCost, 0, MaxPower);
        }

        if (foodCost > 0) Food = Mathf.Clamp(Food - foodCost, 0, MaxFood);
        if (waterCost > 0) Water = Mathf.Clamp(Water - waterCost, 0, MaxWater);

        if (powerCost > 0 || foodCost > 0 || waterCost > 0)
        {
            Debug.Log($"Consommation : Power -{powerCost}, Food -{foodCost}, Water -{waterCost}");
            if (OnResourcesChanged != null) OnResourcesChanged.Invoke();
        }
    }

    public void AddResource(string type, int amount)
    {
        switch (type)
        {
            case "Power":
                int potentialPower = Power + amount;
                if (potentialPower > MaxPower)
                {
                    int surplus = potentialPower - MaxPower;
                    int capsEarned = surplus; 
                    Caps += capsEarned;
                    Debug.Log($"Surplus d'énergie vendue ! (+{capsEarned} Caps)");
                    
                    Power = MaxPower;
                }
                else
                {
                    Power = potentialPower;
                }
                break;
            case "Food":
                Food = Mathf.Clamp(Food + amount, 0, MaxFood);
                break;
            case "Water":
                Water = Mathf.Clamp(Water + amount, 0, MaxWater);
                break;
            case "Tomatoes":
                Tomatoes = Mathf.Clamp(Tomatoes + amount, 0, MaxTomatoes);
                break;
            case "Caps":
                Caps += amount;
                break;
        }
        if (OnResourcesChanged != null) OnResourcesChanged.Invoke();
    }

    public bool TryConsume(string type, int amount)
    {
        if (type == "Caps" && Caps >= amount)
        {
            Caps -= amount;
            if (OnResourcesChanged != null) OnResourcesChanged.Invoke();
            return true;
        }
        else if (type == "Tomatoes" && Tomatoes >= amount)
        {
            Tomatoes -= amount;
            if (OnResourcesChanged != null) OnResourcesChanged.Invoke();
            return true;
        }
        
        return false; 
    }
}
