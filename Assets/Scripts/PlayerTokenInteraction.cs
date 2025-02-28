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
    
    private Camera _camera;
    private TokenController _controller;
    
    void Start()
    {
        // Get the player camera (assuming it's a child of the player)
        _camera = GetComponentInChildren<Camera>();
        
        if (!_camera)
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
        var ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out var hit, interactionDistance, tokenLayer))
        {
            var token = hit.collider.GetComponent<TokenController>();

            if (!token) return;
            _controller = token;
                
            if (interactionPrompt)
            {
                interactionPrompt.SetActive(true);
                    
                if (promptText)
                {
                    promptText.text = $"Press E to collect coin";
                }
            }
                
            // Handle door interaction
            if (!Input.GetKeyDown(interactionKey)) return;
            if (token.isVisible)
            {
                token.CollectCoin();
            }
            else
            {
                Debug.Log("This door is locked!");
            }
        }
        else
        {
            if (!_controller) return;
            _controller = null;
                
            if (interactionPrompt)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}