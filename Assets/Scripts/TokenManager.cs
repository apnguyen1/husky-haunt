using UnityEngine;
using TMPro;

public class TokenManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static TokenManager Instance { get; private set; }
    
    [Header("Token Information")]
    public int totalTokens = 4;
    private int collectedTokens = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI tokenCounterText;
    
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        UpdateTokenUI();
    }
    
    public void TokenCollected()
    {
        collectedTokens++;
        UpdateTokenUI();
        
        // Optional: Add effects, sounds, or additional logic when a token is collected
        Debug.Log($"Token collected! {collectedTokens}/{totalTokens}");
    }
    
    private void UpdateTokenUI()
    {
        if (tokenCounterText)
        {
            tokenCounterText.text = $"Tokens: {collectedTokens}/{totalTokens}";
        }
        else
        {
            Debug.LogWarning("Token counter UI text is not assigned!");
        }
    }
}