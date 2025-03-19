using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float delayBeforeSpawn = 19f; // Default delay in seconds before spawning
    public Transform spawnPoint; // Optional specific spawn location
    
    [Header("Audio")]
    public bool playKnockingSound = true;
    
    private NavMeshAgent navMeshAgent;
    private AudioManager audioManager;
    private bool isActive = false;
    private Renderer[] renderers;
    private Collider[] colliders;
    
    void Start()
    {
        // Get components
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        
        // Get renderers and colliders
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        
        // Disable enemy initially
        SetEnemyActive(false);
        
        // LoreManager will adjust this delay in its Start method
        // If missing LoreManager, adjust delay here based on static property
        LoreManager loreManager = FindObjectOfType<LoreManager>();
        if (loreManager == null && LoreManager.HasLoreBeenShown)
        {
            delayBeforeSpawn = 5f; // Shorter delay for subsequent playthroughs
        }
        
        // Start delayed spawn
        StartCoroutine(SpawnAfterDelay());
    }
    
    void Update()
    {
        // Only chase the player if the enemy is active
        if (isActive && player != null && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
    
    private IEnumerator SpawnAfterDelay()
    {
        
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeSpawn);
        
        // If we have a specific spawn point, move the enemy there
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }
        
        // Play knocking sound
        if (playKnockingSound && audioManager != null)
        {
            audioManager.PlaySFX(audioManager.knocking);
            Debug.Log("Knocking sound played");
        }
        
        // Wait a moment after the knocking sound
        yield return new WaitForSeconds(8f);
        
        // Activate the enemy
        SetEnemyActive(true);
        Debug.Log("Enemy activated");
    }
    
    public void SetEnemyActive(bool active)
    {
        isActive = active;
        
        // Enable/disable renderers to make the enemy visible/invisible
        foreach (var renderer in renderers)
        {
            renderer.enabled = active;
        }
        
        // Enable/disable colliders
        foreach (var collider in colliders)
        {
            collider.enabled = active;
        }
        
        // Enable/disable NavMeshAgent
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = active;
        }
    }
    
    // Method to force-activate the enemy (can be called from other scripts)
    public void ActivateEnemy()
    {
        StopAllCoroutines();
        
        // If we have a specific spawn point, move the enemy there
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }
        
        SetEnemyActive(true);
        Debug.Log("Enemy forcibly activated");
    }
    
    // Method to reset the enemy (for restarting)
    public void ResetEnemy()
    {
        StopAllCoroutines();
        SetEnemyActive(false);
        StartCoroutine(SpawnAfterDelay());
    }
}