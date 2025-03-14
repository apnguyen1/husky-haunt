using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float delayBeforeSpawn = 5f; // Delay in seconds before spawning
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
        }
        
        // Wait a moment after the knocking sound
        yield return new WaitForSeconds(8f);
        
        // Activate the enemy
        SetEnemyActive(true);
    }
    
    private void SetEnemyActive(bool active)
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
}