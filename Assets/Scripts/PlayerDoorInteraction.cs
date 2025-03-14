using UnityEngine;

public class PlayerDoorInteraction : MonoBehaviour
{
    public float interactionDistance = 3.0f;
    public KeyCode interactionKey = KeyCode.E;
    public LayerMask doorLayer; // Set this to the layer your doors are on
    
    [Header("Game References")]
    public GameWinManager winManager;
    public DoorController entranceDoor; // Reference to the entrance door specifically
    
    private Camera playerCamera;
    private DoorController currentDoor;
    private TokenManager tokenManager;
    private InteractionManager interactionManager;
    
    void Start()
    {
        // Get the player camera (assuming it's a child of the player)
        playerCamera = GetComponentInChildren<Camera>();
        
        if (!playerCamera)
        {
            Debug.LogError("No camera found as a child of the player object!");
        }
        
        // Get managers references
        tokenManager = TokenManager.Instance;
        interactionManager = InteractionManager.Instance;
        
        if (interactionManager == null)
        {
            Debug.LogError("InteractionManager not found in the scene!");
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
            
            // Check if this is the entrance door and all tokens are collected
            bool isEntranceDoor = (door == entranceDoor);
            bool canEscape = isEntranceDoor && tokenManager != null && tokenManager.IsAllTokensCollected();
            
            // Show appropriate prompt based on door state
            if (door.isLocked)
            {
                // Handle locked door text
                if (isEntranceDoor && tokenManager != null && !tokenManager.IsAllTokensCollected())
                {
                    interactionManager.ShowPrompt("Door is Locked - Find all tokens to unlock");
                }
                else
                {
                    interactionManager.ShowPrompt("Door is Locked");
                }
            }
            else if (canEscape)
            {
                // Special text for escaping
                interactionManager.ShowPrompt("Press E to Escape");
            }
            else
            {
                // Standard door open/close text
                var doorAction = door.isOpen ? "Close" : "Open";
                interactionManager.ShowPrompt($"Press E to {doorAction} Door");
            }
            
            // Handle door interaction when key is pressed
            if (!Input.GetKeyDown(interactionKey)) return;
            
            // Check if this is the entrance door and player has collected all tokens
            if (canEscape)
            {
                // Trigger the win condition!
                if (winManager != null)
                {
                    winManager.TriggerWin();
                }
                else
                {
                    Debug.LogError("WinManager reference not set in PlayerDoorInteraction!");
                }
            }
            else if (!door.isLocked)
            {
                // Normal door operation
                door.ToggleDoor();
            }
            else
            {
                Debug.Log("This door is locked!");
            }
        }
        else
        {
            if (currentDoor)
            {
                currentDoor = null;
                
                // Only hide prompt if we're not interacting with a token
                // This check prevents door interaction from hiding token prompts
                bool hidePrompt = true;
                
                // Check if there's a TokenInteraction component and it's not showing a prompt
                PlayerTokenInteraction tokenInteraction = GetComponent<PlayerTokenInteraction>();
                if (tokenInteraction != null && tokenInteraction.IsShowingPrompt())
                {
                    hidePrompt = false;
                }
                
                if (hidePrompt && interactionManager != null)
                {
                    interactionManager.HidePrompt();
                }
            }
        }
    }
    
    // Method to check if this component is currently showing a prompt
    public bool IsShowingPrompt()
    {
        return currentDoor != null;
    }
}