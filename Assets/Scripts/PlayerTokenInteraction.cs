using UnityEngine;

public class PlayerTokenInteraction : MonoBehaviour
{
    public float interactionDistance = 2.0f;
    public KeyCode interactionKey = KeyCode.E;
    public LayerMask tokenLayer; // Set this to the layer your tokens are on
    
    private Camera playerCamera;
    private TokenController currentToken;
    private InteractionManager interactionManager;
    private bool isShowingPrompt = false;
    
    void Start()
    {
        // Get the player camera (assuming it's a child of the player)
        playerCamera = GetComponentInChildren<Camera>();
        
        if (!playerCamera)
        {
            Debug.LogError("No camera found as a child of the player object!");
        }
        
        interactionManager = InteractionManager.Instance;
        
        if (interactionManager == null)
        {
            Debug.LogError("InteractionManager not found in the scene!");
        }
    }
    
    void Update()
    {
        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        // Reset prompt state
        isShowingPrompt = false;

        // Clear highlight on previous token if we're not looking at it anymore
        if (currentToken != null)
        {
            currentToken.SetHighlight(false);
            currentToken = null;
        }

        if (Physics.Raycast(ray, out hit, interactionDistance, tokenLayer))
        {
            var token = hit.collider.GetComponent<TokenController>();

            if (!token || !token.isVisible) return;
            
            currentToken = token;
            currentToken.SetHighlight(true);
            
            // Show interaction prompt
            if (interactionManager != null)
            {
                interactionManager.ShowPrompt($"Press {interactionKey} to collect token");
                isShowingPrompt = true;
            }
                
            // Handle token interaction
            if (Input.GetKeyDown(interactionKey) && token.isVisible)
            {
                token.CollectCoin();
                
                // Hide the prompt when collected
                if (interactionManager != null)
                {
                    interactionManager.HidePrompt();
                    isShowingPrompt = false;
                }
            }
        }
        else if (currentToken == null && !isShowingPrompt)
        {
            // Only hide the prompt if we're not interacting with a door
            bool hidePrompt = true;
            
            // Check if there's a DoorInteraction component and it's showing a prompt
            PlayerDoorInteraction doorInteraction = GetComponent<PlayerDoorInteraction>();
            if (doorInteraction != null && doorInteraction.IsShowingPrompt())
            {
                hidePrompt = false;
            }
            
            if (hidePrompt && interactionManager != null)
            {
                interactionManager.HidePrompt();
            }
        }
    }
    
    // Method to check if this component is currently showing a prompt
    public bool IsShowingPrompt()
    {
        return isShowingPrompt;
    }
}