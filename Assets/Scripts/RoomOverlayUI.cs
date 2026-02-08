using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomOverlayUI : MonoBehaviour
{
    public static RoomOverlayUI Instance { get; private set; }

    [Header("Settings")]
    public GameObject RoomButtonPrefab; // Prefab d'un bouton UI
    public Transform CanvasContainer;   // Le Panel/Canvas où mettre les boutons
    public Vector3 UI_Offset = new Vector3(0, 2f, 0); // Décalage vers le haut

    private Dictionary<Room, GameObject> roomButtons = new Dictionary<Room, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateButtonsPosition();
        UpdateButtonsContent();
    }

    // Appelé quand une salle est créée
    public void RegisterRoom(Room room)
    {
        if (RoomButtonPrefab == null || CanvasContainer == null) return;

        // Éviter les doublons
        if (roomButtons.ContainsKey(room)) return;

        GameObject btnObj = Instantiate(RoomButtonPrefab, CanvasContainer);
        Button btn = btnObj.GetComponent<Button>();
        
        if (btn != null)
        {
            btn.onClick.AddListener(() => OnRoomClicked(room));
        }

        roomButtons.Add(room, btnObj);
    }

    private void OnRoomClicked(Room room)
    {
        Debug.Log("Click sur Salle : " + room.RoomName);
        // Ouvrir le menu de sélection de Dweller
        if (DwellerSelectorUI.Instance != null)
        {
            DwellerSelectorUI.Instance.OpenMenu(room);
        }
    }

    private void UpdateButtonsPosition()
    {
        if (Camera.main == null) return;

        foreach (var kvp in roomButtons)
        {
            Room room = kvp.Key;
            GameObject btn = kvp.Value;

            if (room == null || btn == null)
            {
                // Si la salle est détruite, on détruit le bouton (faudra nettoyer la dictionary plus tard)
                if (btn != null) Destroy(btn);
                continue;
            }

            // Positionner le bouton au dessus de la salle
            Vector3 worldPos = room.transform.position + UI_Offset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            
            // Si la salle est derrière la caméra, on cache le bouton
            if (screenPos.z < 0)
            {
                 btn.SetActive(false);
            }
            else
            {
                 btn.SetActive(true);
                 btn.transform.position = screenPos;
            }
        }
    }

    private void UpdateButtonsContent()
    {
        foreach (var kvp in roomButtons)
        {
            Room room = kvp.Key;
            GameObject btn = kvp.Value;

            if (room != null && btn != null)
            {
                Text txt = btn.GetComponentInChildren<Text>();
                if (txt != null)
                {
                    txt.text = $"{room.AssignedDwellers.Count}/{room.MaxWorkers}";
                }
            }
        }
    }
}
