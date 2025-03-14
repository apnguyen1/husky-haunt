using UnityEngine;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    
    [Header("UI Elements")]
    public GameObject interactionPrompt;
    public TextMeshProUGUI promptText;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
    }
    
    private void Start()
    {
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
    
    public void ShowPrompt(string message)
    {
        if (interactionPrompt && promptText)
        {
            interactionPrompt.SetActive(true);
            promptText.text = message;
        }
    }
    
    public void HidePrompt()
    {
        if (interactionPrompt)
        {
            interactionPrompt.SetActive(false);
        }
    }
}