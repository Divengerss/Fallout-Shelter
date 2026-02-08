using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro; // Si tu utilises TMP, sinon 'using UnityEngine.UI;' suffira pour Text

public class DwellerAssignmentUI : MonoBehaviour
{
    public static DwellerAssignmentUI Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject Panel;
    public Transform ButtonContainer;
    public GameObject ButtonPrefab;
    public Text HeaderText;

    private Dweller currentDweller;
    private bool isSelectingRoom = false; // Mode "Visée"
    private Room lastHighlightedRoom = null;

    private void Awake()
    {
        Instance = this;
        CloseMenu();
    }

    private void Update()
    {
        // Gestion du Mode de Sélection (Targeting)
        if (isSelectingRoom)
        {
            HandleSelectionMode();
            return;
        }
    }

    public void OpenMenu(Dweller dweller)
    {
        currentDweller = dweller;
        Panel.SetActive(true);
        isSelectingRoom = false;
        
        if (HeaderText != null) HeaderText.text = "Gérer " + dweller.DwellerName;

        // Nettoyer les vieux boutons
        foreach (Transform child in ButtonContainer) Destroy(child.gameObject);

        // Créer le bouton "DÉPLACER" (Move)
        GameObject btnObj = Instantiate(ButtonPrefab, ButtonContainer);
        Text txt = btnObj.GetComponentInChildren<Text>();
        if (txt != null) txt.text = "DÉPLACER / ASSIGNER";
        
        Button btn = btnObj.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(StartRoomSelection);
    }

    public void StartRoomSelection()
    {
        // On ferme le menu UI pour voir le jeu
        Panel.SetActive(false);
        isSelectingRoom = true;
        Debug.Log("Mode Sélection de Salle Activé ! Clique sur une salle.");
    }

    private void HandleSelectionMode()
    {
        // 1. Annulation (Clic Droit)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelSelection();
            return;
        }

        // 2. Raycast pour Highlight
        if (Mouse.current == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        
        // On cherche une salle
        Room hoveredRoom = null;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hoveredRoom = hit.collider.GetComponent<Room>();
        }

        // Gestion du Highlight visuel
        if (hoveredRoom != lastHighlightedRoom)
        {
            if (lastHighlightedRoom != null) lastHighlightedRoom.SetHighlight(false);
            if (hoveredRoom != null) hoveredRoom.SetHighlight(true);
            lastHighlightedRoom = hoveredRoom;
        }

        // 3. Validation (Clic Gauche)
        if (Mouse.current.leftButton.wasPressedThisFrame && hoveredRoom != null)
        {
            AssignToRoom(hoveredRoom);
        }
    }

    private void AssignToRoom(Room room)
    {
        if (currentDweller != null)
        {
            if (room.TryAddDweller(currentDweller))
            {
                Debug.Log($"Assignation réussie : {currentDweller.DwellerName} -> {room.RoomName}");
            }
            else
            {
                Debug.Log("Salle pleine !");
            }
        }
        CancelSelection(); // Fin du mode
    }

    private void CancelSelection()
    {
        isSelectingRoom = false;
        if (lastHighlightedRoom != null)
        {
            lastHighlightedRoom.SetHighlight(false);
            lastHighlightedRoom = null;
        }
        currentDweller = null;
        // On pourrait rouvrir le menu ici si on voulait, mais fermer c'est bien aussi
    }

    public void CloseMenu()
    {
        Panel.SetActive(false);
        currentDweller = null;
        isSelectingRoom = false;
    }
}
