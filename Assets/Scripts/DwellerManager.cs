using UnityEngine;

public class DwellerManager : MonoBehaviour
{
    public static DwellerManager Instance { get; private set; }

    [Header("Spawning")]
    public GameObject DwellerPrefab;
    public int MaxDwellers = 200; 

    public int DwellerCost = 200; 

    private int currentDwellers = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DwellerCost = 200;
        
        SpawnDweller();
    }

    public void BuyDweller()
    {
        if (ResourceManager.Instance.TryConsume("Caps", DwellerCost))
        {
            SpawnDweller();
            Debug.Log($"Habitant acheté pour {DwellerCost} Caps !");
        }
        else
        {
            Debug.Log($"Pas assez de Caps ! Coût: {DwellerCost}, Disponibles: {ResourceManager.Instance.Caps}");
        }
    }

    private void SpawnDweller()
    {
        if (DwellerPrefab == null) return;

        Vector3 spawnPos = Vector3.zero;
        if (RoomManager.Instance != null)
        {
            spawnPos = RoomManager.Instance.StartOrigin;
        }

        GameObject newDwellerObj = Instantiate(DwellerPrefab, spawnPos, Quaternion.identity);
        newDwellerObj.name = "Dweller " + (currentDwellers + 1);
        
        Dweller dwellerScript = newDwellerObj.GetComponent<Dweller>();

        if (RoomManager.Instance != null && RoomManager.Instance.EntranceInstance != null)
        {
            Room entrance = RoomManager.Instance.EntranceInstance;
            if (entrance.TryAddDweller(dwellerScript))
            {
                Debug.Log($"Nouvel habitant arrivé et assigné à l'entrée !");
            }
            else
            {
                Debug.LogWarning("L'entrée est pleine ! L'habitant reste dehors (sans salle).");
            }
        }
        
        currentDwellers++;
    }

    public void AutoAssignAllDwellers()
    {
        Dweller[] allDwellers = FindObjectsByType<Dweller>(FindObjectsSortMode.None);
        
        foreach (Dweller dweller in allDwellers)
        {
            AutoAssignDweller(dweller);
        }
    }

    public void AutoAssignDweller(Dweller dweller)
    {
        if (RoomManager.Instance == null) return;

        Room bestRoom = null;
        
        bestRoom = FindRoomWithSpace("Power");
        
        if (bestRoom == null) bestRoom = FindRoomWithSpace("Food");
        
        if (bestRoom == null) bestRoom = FindRoomWithSpace("Water");
        
        if (bestRoom == null)
        {
             foreach (Room room in RoomManager.Instance.AllRooms)
             {
                 if (room.AssignedDwellers.Count < room.MaxWorkers && dweller.CurrentRoom != room)
                 {
                     bestRoom = room;
                     break;
                 }
             }
        }

        if (bestRoom != null)
        {
            if (bestRoom.TryAddDweller(dweller))
            {
                Debug.Log($"Auto-Assign (Universel) : {dweller.DwellerName} -> {bestRoom.RoomName} ({bestRoom.ProductionType})");
            }
        }
    }

    private Room FindRoomWithSpace(string type)
    {
        foreach (Room room in RoomManager.Instance.AllRooms)
        {
            if (room.ProductionType == type)
            {
                if (room.AssignedDwellers.Count < room.MaxWorkers)
                {
                    return room;
                }
            }
        }
        return null;
    }
}
