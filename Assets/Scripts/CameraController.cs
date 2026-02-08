using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float PanSpeed = 20f; // Vitesse clavier
    public float DragSpeed = 0.5f; // Vitesse souris
    public float ZoomSpeed = 2.0f;
    public Vector2 MinMaxZ = new Vector2(-100, -10); // Limites de Zoom
    public Vector2 LimitX = new Vector2(0, 20); // Limites largeur
    public Vector2 LimitY = new Vector2(0, 20); // Limites hauteur

    private Vector3 lastMousePosition;
    private bool isDragging = false;

    private void Update()
    {
        if (Mouse.current == null) return;

        HandleMousePan();
        HandleKeyboardPan();
        HandleZoom();
        ClampPosition();
    }

    private void HandleMousePan()
    {
        // Clic Gauche pour Grab
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Astuce : On ne veut pas bouger la caméra si on clique sur un Habitant !
            // Mais on VEUT pouvoir bouger si on clique sur une salle (fond)
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            
            bool hitDweller = false;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.GetComponent<Dweller>() != null)
                {
                    hitDweller = true;
                }
            }

            // Si on n'a PAS touché d'habitant, on peut bouger la caméra
            if (!hitDweller)
            {
                isDragging = true;
                lastMousePosition = mousePos;
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = Mouse.current.position.ReadValue();
            Vector3 delta = lastMousePosition - currentMousePosition; // Inversé pour "tirer" le monde
            
            // Ajuster la vitesse selon le zoom
            float speedMultiplier = Mathf.Abs(transform.position.z) * 0.001f * DragSpeed;
            
            Vector3 move = new Vector3(delta.x * speedMultiplier, delta.y * speedMultiplier, 0);
            transform.Translate(move, Space.World);

            lastMousePosition = currentMousePosition;
        }
    }

    private void HandleKeyboardPan()
    {
        if (Keyboard.current == null) return;

        Vector3 move = Vector3.zero;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) move.x -= 1;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) move.x += 1;
        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) move.y += 1;
        if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) move.y -= 1;

        if (move != Vector3.zero)
        {
            // Vitesse constante
            transform.Translate(move * PanSpeed * Time.deltaTime, Space.World);
        }
    }

    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.y.ReadValue();
        if (scroll != 0)
        {
            // Zoom = Avancer/Reculer en Z
            float zoomAmount = scroll * ZoomSpeed * 0.01f;
            Vector3 newPos = transform.position + transform.forward * zoomAmount;
            
            // On limite le zoom (ne pas traverser le sol ou partir trop loin)
            // On suppose que la caméra regarde vers Z positif ou négatif.
            // Généralement en 2D/3D Side View, on recule en Z négatif.
            
            // Clamp Z
            newPos.z = Mathf.Clamp(newPos.z, MinMaxZ.x, MinMaxZ.y);
            
            transform.position = newPos;
        }
    }

    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, LimitX.x, LimitX.y);
        pos.y = Mathf.Clamp(pos.y, LimitY.x, LimitY.y);
        transform.position = pos;
    }
}
