using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using StarterAssets;

public class GameLoseCondition : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loseScreen; // Renamed from winScreen to loseScreen for clarity
    public TextMeshProUGUI loseMessageText; // Renamed from winMessageText
    public TextMeshProUGUI loseDetailsText; // Renamed from winDetailsText
    public Button restartButton;
    public Button quitButton;

    [Header("Scene References")]
    public Light spotlight;
    public Light entranceLight;
    public Transform enemy; // Assign the enemy in the Inspector

    [Header("Audio")]
    public AudioClip jump_scare_intro;
    public AudioClip jump_scare_scream;
    [Range(0f, 1f)]
    public float volume = 0.5f; // Volume for scare sounds

    [Header("Camera Animation")]
    public Transform Camera;
    public Transform Capsule;
    public Transform Flashlight;

    private Animator animator;
    private AudioManager audioManager;
    private GameWinManager gameWinManager;
    private bool hasLost = false;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        gameWinManager = FindObjectOfType<GameWinManager>();

        // Hide the lose screen initially
        if (loseScreen)
        {
            loseScreen.SetActive(false);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !hasLost)
        {
            hasLost = true;
            Debug.Log("Collided with the Enemy!");

            // Try to get the animator component
            animator = other.gameObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isScreaming", true);
            }

            StartCoroutine(PlayLoseSequence());
        }
    }

    private IEnumerator PlayLoseSequence()
    {
        // Turn off lights for scary effect
        if (spotlight != null) spotlight.enabled = false;
        if (entranceLight != null) entranceLight.enabled = false;

        // Disable player movement
        var playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (playerController)
        {
            playerController.enabled = false;
        }

        // Disable mouse look script (Starter Assets)
        var playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();
        if (playerInput)
        {
            playerInput.cursorInputForLook = false; // Stops mouse look processing
        }

        // Play intro horror sound
        if (audioManager != null && jump_scare_intro != null)
        {
            audioManager.PlaySFXWithVolume(jump_scare_intro, volume);
        }

        // Short wait for dramatic effect
        yield return new WaitForSeconds(1f);

        // Animate camera to look at enemy
        if (Camera != null && enemy != null)
        {
            StartCoroutine(AnimateCamera(Camera, Capsule, Flashlight, enemy));
        }

        // Wait for animation
        yield return new WaitForSeconds(2f);

        // Show lose screen with fade in
        if (loseScreen)
        {
            loseScreen.SetActive(true);

            // Fade in the UI elements
            foreach (var graphic in loseScreen.GetComponentsInChildren<Graphic>())
            {
                StartCoroutine(FadeInGraphic(graphic));
            }

            // Set lose message
            if (loseMessageText)
            {
                loseMessageText.text = "You've been caught!";
            }

            // Set lose details
            if (loseDetailsText)
            {
                loseDetailsText.text = "The Husky has claimed another victim. The asylum keeps its secrets.";
            }
        }

        // Enable the restart options via the game win manager if it exists
        if (gameWinManager != null)
        {
            gameWinManager.EnableRestartOptions();
        }

        // Unlock cursor for button interactions
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator AnimateCamera(Transform camera, Transform body, Transform light, Transform enemy)
    {
        // Calculate the new camera position in front of the enemy
        Vector3 newPosition = enemy.position + enemy.forward * 1.3f;

        // Move the camera
        camera.position = new Vector3(newPosition.x, newPosition.y + 1.1f, newPosition.z);
        body.position = newPosition + enemy.forward * 40f;
        light.position = new Vector3(newPosition.x, newPosition.y + 1.1f, newPosition.z);

        // Rotate the camera to look at the enemy
        camera.LookAt(enemy.position + Vector3.up * 1f);
        light.LookAt(enemy.position + Vector3.up * 1f);

        // Turn the spotlight back on for dramatic effect
        if (spotlight != null) spotlight.enabled = true;

        // Play the jump scare scream
        if (audioManager != null && jump_scare_scream != null)
        {
            audioManager.PlaySFXWithVolume(jump_scare_scream, volume);
        }

        yield return null;
    }

    private IEnumerator FadeInGraphic(Graphic graphic)
    {
        graphic.canvasRenderer.SetAlpha(0f);
        graphic.CrossFadeAlpha(1f, 1.5f, true);
        yield return null;
    }

    public void RestartGame()
    {
        // Do not reset lore status - we want it to stay shown on restarts
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Cursor.visible = false;
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