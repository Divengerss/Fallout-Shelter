using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Dweller : MonoBehaviour
{
    [Header("Stats (S.P.E.C.I.A.L Simplified)")]
    public string DwellerName = "Bob";
    public int Strength = 1; // Power
    public int Agility = 1;  // Food
    public int Perception = 1; // Water

    [Header("Visuals")]
    public Color IdleColor = Color.red;
    public Color WorkingColor = Color.green;
    private Renderer myRenderer;

    [Header("Movement")]
    public float MoveSpeed = 2f;
    public Vector2 WaitTimeRange = Vector2.zero; // Bouge en continu !
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isWaiting = false;

    [Header("Etat")]
    public Room CurrentRoom;
    private bool isDragging = false;
    private Vector3 originalPosition;
    private float zDepth = 0f; // Profondeur Z pour garder le bon plan

    private void Start()
    {
        // ... (Keep existing Start logic)
        Strength = Random.Range(1, 4);
        Agility = Random.Range(1, 4);
        Perception = Random.Range(1, 4);
        zDepth = transform.position.z;

        myRenderer = GetComponent<Renderer>();
        UpdateColor();

        // Initialiser la position cible à la position actuelle
        targetPosition = transform.position;
    }

    private void Update()
    {
        UpdateColor();
        HandleWandering();

        if (Mouse.current == null) return;

        // Simple Clic Gauche pour ouvrir le menu d'assignation
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckClick();
        }
    }

    private void CheckClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == this.transform)
            {
                // CLIC DÉTECTÉ : On ouvre l'UI
                if (DwellerAssignmentUI.Instance != null)
                {
                    DwellerAssignmentUI.Instance.OpenMenu(this);
                }
                else
                {
                    Debug.LogWarning("Pas de DwellerAssignmentUI dans la scène !");
                }
            }
        }
    }

    // (Ancien code de Drag supprimé pour clarté, si tu veux le garder dis-le moi)
    // Pour l'instant, le clic = Menu, plus de Drag.
    
    private void HandleWandering()
    {
        if (CurrentRoom == null) return; // Pas de salle, pas de balade
        if (isDragging) return;

        // Si on ne bouge pas et qu'on n'attend pas deJA, on décide quoi faire
        if (!isMoving && !isWaiting)
        {
            StartCoroutine(PickNewDestination());
        }

        // Si on a une cible, on y va
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

            // Arrivé ?
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    private System.Collections.IEnumerator PickNewDestination()
    {
        isWaiting = true;
        // On attend un peu
        float waitTime = Random.Range(WaitTimeRange.x, WaitTimeRange.y);
        yield return new WaitForSeconds(waitTime);

        if (CurrentRoom != null && !isDragging)
        {
            // On choisit un point au hasard DANS la salle
            // On suppose que la salle fait GridSize de large (moins une marge)
            float halfWidth = 4f; // Marge (GridSize est a 10, donc 5 de demi-largeur, on prend 4 pour ne pas sortir)
            if (RoomManager.Instance != null) halfWidth = (RoomManager.Instance.GridSize / 2f) - 1f;

            float randomX = Random.Range(-halfWidth, halfWidth);
            Vector3 roomCenter = CurrentRoom.transform.position;
            
            // On vise la position X de la salle (+/- random)
            // ET la position Y de la salle (pour changer d'étage si besoin !)
            targetPosition = new Vector3(roomCenter.x + randomX, roomCenter.y, zDepth);
            isMoving = true;
        }
        isWaiting = false;
    }



    public void SetCurrentRoom(Room room)
    {
        if (CurrentRoom != null && CurrentRoom != room)
        {
            CurrentRoom.RemoveDweller(this);
        }
        CurrentRoom = room;

        // On arrête tout ce qu'on faisait pour aller vers la nouvelle salle
        StopAllCoroutines();
        isMoving = false;
        isWaiting = false;
        // La prochaine update de HandleWandering va lancer PickNewDestination avec la nouvelle CurrentRoom
    }

    private void UpdateColor()
    {
        if (myRenderer == null) return;

        // Si on est dans une salle ET que la salle produit quelque chose (pas l'entrée)
        bool isWorking = (CurrentRoom != null && CurrentRoom.ProductionType != "None");

        myRenderer.material.color = isWorking ? WorkingColor : IdleColor;
    }
}
