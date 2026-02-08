using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Réglages")]
    public float GridSize = 40.0f; 
    public Transform RoomContainer; 
    public Room EntrancePrefab; 
    public int MaxRooms = 100; 
    public Vector3 StartOrigin = Vector3.zero; 
    public Vector3 RoomRotation = new Vector3(0, 180, 0); 
    
    public Room EntranceInstance { get; private set; }
    
    public List<Room> AllRooms = new List<Room>();

    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

    private Room pendingRoomPrefab;
    private GameObject ghostObject;
    private bool isPlacing = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (RoomContainer != null) RoomContainer.position = Vector3.zero;
        StartOrigin = Vector3.zero;

        if (EntrancePrefab != null)
        {
            EntranceInstance = Instantiate(EntrancePrefab, StartOrigin, Quaternion.Euler(RoomRotation), RoomContainer);
            AllRooms.Add(EntranceInstance); 
            
            if (RoomOverlayUI.Instance != null) RoomOverlayUI.Instance.RegisterRoom(EntranceInstance);

            Debug.Log($"Entrée construite en {StartOrigin}. Si elle est loin, c'est le Prefab qui est mal centré !");
        }
        else
        {
            Debug.LogWarning("Attention: Pas de Prefab 'Entrance' assigné dans RoomManager !");
        }

        occupiedPositions.Add(Vector2.zero);
    }

    private void Update()
    {
        if (isPlacing)
        {
            UpdatePlacement();
        }
    }

    public void StartPlacingRoom(Room roomPrefab)
    {
        if (occupiedPositions.Count >= MaxRooms)
        {
            Debug.Log("Max Salles atteint ! Impossible de construire plus.");
            return;
        }

        if (isPlacing) CancelPlacement();

        pendingRoomPrefab = roomPrefab;
        isPlacing = true;

        ghostObject = Instantiate(roomPrefab.gameObject, Vector3.zero, Quaternion.Euler(RoomRotation));
        
        Destroy(ghostObject.GetComponent<Room>());
    }

    public void CancelPlacement()
    {
        isPlacing = false;
        pendingRoomPrefab = null;
        if (ghostObject != null) Destroy(ghostObject);
    }

    private void UpdatePlacement()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Plane plane = new Plane(Vector3.back, Vector3.zero); 
        
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);

            int x = Mathf.RoundToInt((worldPoint.x - StartOrigin.x) / GridSize);
            int y = Mathf.RoundToInt((worldPoint.y - StartOrigin.y) / GridSize);
            Vector2 gridPos = new Vector2(x, y);

            ghostObject.transform.position = StartOrigin + new Vector3(x * GridSize, y * GridSize, 0);

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                RoomRotation.y += 90f;
                ghostObject.transform.rotation = Quaternion.Euler(RoomRotation);
            }
            ghostObject.transform.rotation = Quaternion.Euler(RoomRotation);

            bool isValid = IsValidPosition(gridPos);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (isValid)
                {
                    TryBuild(gridPos);
                }
                else
                {
                    Debug.Log("Position Invalide !");
                }
            }
            
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelPlacement();
            }
        }
    }

    private bool IsValidPosition(Vector2 pos)
    {
        if (occupiedPositions.Contains(pos)) return false;

        Vector2 leftNeighbor = new Vector2(pos.x - 1, pos.y);
        if (occupiedPositions.Contains(leftNeighbor)) return true;

        Vector2 topNeighbor = new Vector2(pos.x, pos.y + 1);
        if (occupiedPositions.Contains(topNeighbor)) return true;

        return false;
    }

    private void TryBuild(Vector2 gridPos)
    {
        int cost = pendingRoomPrefab.Cost;
        if (!ResourceManager.Instance.TryConsume("Caps", cost))
        {
            Debug.Log("Pas assez de Caps !");
            return;
        }

        Vector3 worldPos = StartOrigin + new Vector3(gridPos.x * GridSize, gridPos.y * GridSize, 0);
        Room newRoom = Instantiate(pendingRoomPrefab, worldPos, Quaternion.Euler(RoomRotation), RoomContainer);
        AllRooms.Add(newRoom); 

        if (RoomOverlayUI.Instance != null) RoomOverlayUI.Instance.RegisterRoom(newRoom);
        
        occupiedPositions.Add(gridPos);
        
        CancelPlacement();
        Debug.Log("Salle construite !");
    }
}
