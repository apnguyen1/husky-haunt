using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDoorInteraction : MonoBehaviour
{
    public float interactionDistance = 3.0f;
    public KeyCode interactionKey = KeyCode.E;
    public LayerMask doorLayer; // Set this to the layer your doors are on
    
    [Header("UI Elements")]
    public GameObject interactionPrompt;
    public TextMeshProUGUI promptText;
    
    private Camera playerCamera;
    private DoorController currentDoor;
    
    void Start()
    {
        // Get the player camera (assuming it's a child of the player)
        playerCamera = GetComponentInChildren<Camera>();
        
        if (!playerCamera)
        {
            Debug.LogError("No camera found as a child of the player object!");
        }
        
        // Hide the interaction prompt initially
        if (interactionPrompt)
        {
            interactionPrompt.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No interaction prompt UI assigned!");
        }
    }
    
    void Update()
    {
        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out var hit, interactionDistance, doorLayer))
        {
            var door = hit.collider.GetComponent<DoorController>();

            if (!door) return;
            currentDoor = door;
                
            if (interactionPrompt)
            {
                interactionPrompt.SetActive(true);
                    
                if (promptText)
                {
                    var doorState = door.isLocked ? "Locked" : (door.isOpen ? "Close" : "Open");
                    promptText.text = $"Press E to {doorState} Door";
                }
            }
                
            // Handle door interaction
            if (!Input.GetKeyDown(interactionKey)) return;
            if (!door.isLocked)
            {
                door.ToggleDoor();
            }
            else
            {
                Debug.Log("This door is locked!");
            }
        }
        else
        {
            if (!currentDoor) return;
            currentDoor = null;
                
            if (interactionPrompt)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}