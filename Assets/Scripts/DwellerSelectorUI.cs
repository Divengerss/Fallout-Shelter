using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DwellerSelectorUI : MonoBehaviour
{
    public static DwellerSelectorUI Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject Panel;
    public Transform ListContainer;
    public GameObject DwellerButtonPrefab; // Un bouton avec Text
    public Text HeaderText;

    private Room targetRoom;

    private void Awake()
    {
        Instance = this;
        CloseMenu();
    }

    public void OpenMenu(Room room)
    {
        targetRoom = room;
        Panel.SetActive(true);
        if (HeaderText != null) HeaderText.text = $"Ajouter à {room.RoomName}";

        RefreshList();
    }

    private void RefreshList()
    {
        // Nettoyer
        foreach (Transform child in ListContainer) Destroy(child.gameObject);

        // Trouver tous les Dwellers
        Dweller[] allDwellers = FindObjectsByType<Dweller>(FindObjectsSortMode.None);

        foreach (Dweller d in allDwellers)
        {
            // On n'affiche que ceux qui ne sont PAS DÉJÀ dans CETTE salle
            if (d.CurrentRoom == targetRoom) continue;

            CreateDwellerButton(d);
        }
    }

    private void CreateDwellerButton(Dweller d)
    {
        GameObject btnObj = Instantiate(DwellerButtonPrefab, ListContainer);
        Text txt = btnObj.GetComponentInChildren<Text>();
        
        string status = (d.CurrentRoom == null) ? "Idle" : d.CurrentRoom.RoomName;
        if (txt != null) txt.text = $"{d.DwellerName} ({status})";

        Button btn = btnObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => OnDwellerSelected(d));
        }
    }

    private void OnDwellerSelected(Dweller d)
    {
        if (targetRoom != null)
        {
            // 1. Retirer de l'ancienne salle (géré par SetCurrentRoom/TryAddDweller normalement, mais on force pour être sûr)
            if (d.CurrentRoom != null)
            {
                d.CurrentRoom.RemoveDweller(d);
            }

            // 2. Ajouter à la nouvelle
            if (targetRoom.TryAddDweller(d))
            {
                Debug.Log($"Déménagement réussi : {d.DwellerName} -> {targetRoom.RoomName}");
            }
            else
            {
                Debug.Log("Salle pleine !");
            }
        }
        CloseMenu();
    }

    public void CloseMenu()
    {
        Panel.SetActive(false);
        targetRoom = null;
    }
}
