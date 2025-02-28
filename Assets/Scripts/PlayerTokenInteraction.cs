using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerTokenInteraction : MonoBehaviour
{
    public float interactionDistance = 3.0f;
    public KeyCode interactionKey = KeyCode.E;
    public LayerMask tokenLayer; // Set this to the layer your tokens are on
    
    [Header("UI Elements")]
    public GameObject interactionPrompt;
    public TextMeshProUGUI promptText;
    
    private Camera playerCamera;
    private TokenController currentToken;
    
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
        RaycastHit hit;

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
                
            if (interactionPrompt)
            {
                interactionPrompt.SetActive(true);
                    
                if (promptText)
                {
                    promptText.text = $"Press {interactionKey} to collect token";
                }
            }
                
            // Handle token interaction
            if (Input.GetKeyDown(interactionKey) && token.isVisible)
            {
                token.CollectCoin();
                
                // Hide the prompt when collected
                if (interactionPrompt)
                {
                    interactionPrompt.SetActive(false);
                }
            }
        }
        else
        {
            if (interactionPrompt)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}