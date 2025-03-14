using UnityEngine;
using TMPro;
using System.Collections;

public class TokenManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static TokenManager Instance { get; private set; }
    
    [Header("Token Information")]
    public int totalTokens = 4;
    private int collectedTokens = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI tokenCounterText;
    
    [Header("Game References")]
    public DoorController entranceDoor;
    public AudioClip unlockSound;
    
    private AudioManager audioManager;
    private bool allTokensCollected = false;
    
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        UpdateTokenUI();
        
    }
    
    public void TokenCollected()
    {
        collectedTokens++;
        UpdateTokenUI();
        
        // Check if all tokens have been collected
        if (collectedTokens >= totalTokens && !allTokensCollected)
        {
            allTokensCollected = true;
            AllTokensCollected();
        }
        
        // Optional: Add effects, sounds, or additional logic when a token is collected
        Debug.Log($"Token collected! {collectedTokens}/{totalTokens}");
    }
    
    private void AllTokensCollected()
    {
        Debug.Log("All tokens collected! The entrance door is now unlocked.");
        
        // Unlock the entrance door
        if (entranceDoor != null)
        {
            entranceDoor.SetLocked(false);
            
            // Play unlock sound
            if (audioManager != null && unlockSound != null)
            {
                audioManager.PlaySFX(unlockSound);
            }
        }
    }
    
    private void UpdateTokenUI()
    {
        if (tokenCounterText)
        {
            if (collectedTokens >= totalTokens)
            {
                tokenCounterText.text = "Escape to the Entrance Door!";
            }
            else
            {
                tokenCounterText.text = $"Tokens: {collectedTokens}/{totalTokens}";
            }
        }
        else
        {
            Debug.LogWarning("Token counter UI text is not assigned!");
        }
    }
    
    // Public method to check if all tokens have been collected
    public bool IsAllTokensCollected()
    {
        return allTokensCollected;
    }
    
    // Call this method to check if the entrance door has been unlocked
    public bool IsEntranceDoorUnlocked()
    {
        return entranceDoor != null && !entranceDoor.isLocked;
    }
}