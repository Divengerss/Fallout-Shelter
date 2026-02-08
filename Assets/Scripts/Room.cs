using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Paramètres de la Salle")]
    public string RoomName = "Room";
    public string ProductionType = "None"; 
    public int ProductionAmount = 5; 
    
    [Header("Consommation (Opitonnel)")]
    public string ConsumptionType = "None"; 
    public int ConsumptionAmount = 0; 
    
    public int Cost = 100;
    public float ProductionInterval = 300f; 
    
    [Header("Etat")]
    public List<Dweller> AssignedDwellers = new List<Dweller>();
    public int MaxWorkers = 2; 
    public bool IsActive => AssignedDwellers.Count > 0; 

    private float timer = 0f;
    private bool isPaused = false;

    private void Update()
    {
        if (ProductionType == "None") return;

        if (!IsActive) return;

        timer += Time.deltaTime;

        if (timer >= ProductionInterval)
        {
            if (ProductionType != "Power" && ResourceManager.Instance.Power <= 0)
            {
                if (!isPaused)
                {
                    Debug.LogWarning($"Production arrêtée dans {RoomName} : Plus d'énergie !");
                    isPaused = true;
                }
                return;
            }

            if (isPaused)
            {
                 Debug.Log($"Production reprise dans {RoomName} !");
                 isPaused = false;
            }

            Produce();
            timer = 0f;
        }
    }

    public bool TryAddDweller(Dweller dweller)
    {
        if (AssignedDwellers.Count >= MaxWorkers) return false;

        if (AssignedDwellers.Contains(dweller)) return true;

        AssignedDwellers.Add(dweller);
        
        dweller.transform.position = this.transform.position + new Vector3(0, -0.2f, -0.5f); 
        dweller.SetCurrentRoom(this);
        
        return true;
    }

    private Color originalColor;
    private Renderer roomRenderer;
    private bool isHighlighted = false;

    private void Start()
    {
        roomRenderer = GetComponent<Renderer>();
        if (roomRenderer != null)
        {
            originalColor = roomRenderer.material.color;
        }

        if (ProductionType == "Power")
        {
            ProductionInterval = 20f; 
            ProductionAmount = 10;     
        }
        
        Debug.Log($"Salle {RoomName} Initialisée : Type={ProductionType}, Amount={ProductionAmount}, Interval={ProductionInterval}");
    }

    public void SetHighlight(bool active)
    {
        if (roomRenderer == null) return;

        if (active && !isHighlighted)
        {
            roomRenderer.material.color = Color.yellow; 
            isHighlighted = true;
        }
        else if (!active && isHighlighted)
        {
            roomRenderer.material.color = originalColor;
            isHighlighted = false;
        }
    }

    public void RemoveDweller(Dweller dweller)
    {
        if (AssignedDwellers.Contains(dweller))
        {
            AssignedDwellers.Remove(dweller);
        }
    }

    private void Produce()
    {
        if (ResourceManager.Instance != null)
        {
            if (ConsumptionType != "None" && ConsumptionAmount > 0)
            {
                if (!ResourceManager.Instance.TryConsume(ConsumptionType, ConsumptionAmount))
                {
                   Debug.LogWarning($"{RoomName} n'a pas assez de {ConsumptionType} pour produire !");
                   return; 
                }
            }

            int totalGain = ProductionAmount * AssignedDwellers.Count;
            
            ResourceManager.Instance.AddResource(ProductionType, totalGain);
            Debug.Log($"{RoomName} a produit {totalGain} {ProductionType} ({AssignedDwellers.Count} travailleurs)");
        }
    }
}
