using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AI; // Required for NavMeshAgent

public class GameWinManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winScreen;
    public TextMeshProUGUI winMessageText;
    public TextMeshProUGUI winDetailsText;
    public Button restartButton;
    public Button quitButton;

    [Header("Animation Settings")]
    public float fadeInTime = 1.5f;
    public float cameraZoomOutSpeed = 0.5f;
    public float cameraRotationSpeed = 30f;

    [Header("Audio")]
    public AudioClip escapeSound;
    [Range(0f, 1f)]
    public float escapeSoundVolume = 0.5f; // Lower volume for escape sound
    
    [Header("Enemy")]
    public GameObject enemy; // Assign the enemy GameObject in the Inspector

    private AudioManager audioManager;
    private bool gameCompleted = false;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        // Hide the win screen initially
        if (winScreen)
        {
            winScreen.SetActive(false);
        }

        // Setup button listeners
        if (restartButton)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (quitButton)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void TriggerWin()
    {
        if (!gameCompleted)
        {
            gameCompleted = true;
            StartCoroutine(PlayWinSequence());
        }
    }

    // This method can be called from the GameLoseCondition to enable UI elements
    public void EnableRestartOptions()
    {
        // Make sure buttons are interactable
        if (restartButton)
        {
            restartButton.interactable = true;
        }
        
        if (quitButton)
        {
            quitButton.interactable = true;
        }
    }

    private IEnumerator PlayWinSequence()
    {
        // Stop the enemy if it exists
        if (enemy != null)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true; // Stops the enemy from moving
                agent.velocity = Vector3.zero; // Ensures it doesn't slide
            }
        }

        // Stop background music
        if (audioManager != null)
        {
            // Stop the music by setting volume to 0 (more graceful than stopping)
            if (audioManager.musicSource != null && audioManager.musicSource.isPlaying)
            {
                // Option 1: Fade out music gradually
                StartCoroutine(FadeOutMusic(audioManager.musicSource, 1.0f));

                // Option 2: Stop music immediately (uncomment if preferred)
                // audioManager.musicSource.Stop();
            }

            // Play victory sound at reduced volume
            if (escapeSound != null)
            {
                // Use this if you want to adjust the volume for just this sound
                audioManager.PlaySFXWithVolume(escapeSound, escapeSoundVolume);
            }
        }

        // Disable player movement
        var playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<UnityEngine.CharacterController>();
        if (playerController)
        {
            playerController.enabled = false;
        }

        // Disable mouse look script (Starter Assets)
        var playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.StarterAssetsInputs>();
        if (playerInput)
        {
            playerInput.cursorInputForLook = false; // Stops mouse look processing
        }

        // Find the player camera for animations
        var playerCamera = Camera.main;
        if (playerCamera)
        {
            // Start a nice camera animation
            StartCoroutine(AnimateCamera(playerCamera));
        }

        // Wait a bit before showing the win screen
        yield return new WaitForSeconds(0.5f);

        // Show win screen with fade in
        if (winScreen)
        {
            winScreen.SetActive(true);

            // Fade in the UI elements
            foreach (var graphic in winScreen.GetComponentsInChildren<Graphic>())
            {
                StartCoroutine(FadeInGraphic(graphic));
            }

            // Set win message
            if (winMessageText)
            {
                winMessageText.text = "You Escaped!";
            }

            // Set win details
            if (winDetailsText)
            {
                winDetailsText.text = "Congratulations! You collected all the tokens and found your way out of Husky Haunt.";
            }
        }

        // Unlock cursor for button interactions
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator FadeOutMusic(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;

        while (Time.time < endTime)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        // Ensure volume is set to 0
        audioSource.volume = 0f;
        // Optional: Stop the audio source after fading out
        audioSource.Stop();
    }

    private IEnumerator AnimateCamera(Camera camera)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = camera.transform.position;
        Quaternion originalRotation = camera.transform.rotation;
        float originalFOV = camera.fieldOfView;

        while (elapsedTime < 5f)
        {
            // Slowly rotate camera
            camera.transform.Rotate(Vector3.up, cameraRotationSpeed * Time.deltaTime);

            // Slowly increase field of view (zoom out effect)
            camera.fieldOfView = Mathf.Lerp(originalFOV, originalFOV + 20f, elapsedTime / 5f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeInGraphic(Graphic graphic)
    {
        graphic.canvasRenderer.SetAlpha(0f);
        graphic.CrossFadeAlpha(1f, fadeInTime, true);
        yield return null;
    }

    public void RestartGame()
    {
        // Do not reset the lore status flag - we want it to remain shown
        // LoreManager.ResetLoreStatus(); // This would make the lore show again
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}