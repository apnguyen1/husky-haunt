using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameLoseCondition : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winScreen;
    public TextMeshProUGUI winMessageText;
    public TextMeshProUGUI winDetailsText;
    public Button restartButton;
    public Button quitButton;
    public Light spotlight;
    public Light entranceLight;
    public Transform enemy; // Assign the enemy in the Inspector

    [Header("Audio")]
    public AudioClip jump_scare_intro;
    public AudioClip jump_scare_scream;
    [Range(0f, 1f)]
    public float volume = 0.5f; // Lower volume for escape sound

    public Transform Camera;
    public Transform Capsule;
    public Transform Flashlight;
    private Animator animator;
    private AudioManager audioManager;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            print("Collided with the Enemy!");
            animator = other.gameObject.GetComponent<Animator>();
            animator.SetBool("isScreaming", true);
            StartCoroutine(PlayLoseSequence());
        }
    }

    private IEnumerator PlayLoseSequence()
    {

        spotlight.enabled = false;
        entranceLight.enabled = false;

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

        // Wait a bit before showing the win screen
        audioManager.PlaySFXWithVolume(jump_scare_intro, volume);
        yield return new WaitForSeconds(6.3f);

        StartCoroutine(AnimateCamera(Camera, Capsule, Flashlight, enemy));

        yield return new WaitForSeconds(2f);

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
                winMessageText.text = "You are caught!";
            }

            // Set win details
            if (winDetailsText)
            {
                winDetailsText.text = "You Lose!";
            }
        }

        // Unlock cursor for button interactions
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator AnimateCamera(Transform camera, Transform body, Transform light, Transform enemy)
    {
        // Calculate the new camera position in front of the enemy
        Vector3 newPosition = enemy.position + enemy.forward * 1.2f;

        // Move the camera
        camera.position = new Vector3(newPosition.x, newPosition.y + 1.1f, newPosition.z);
        body.position = newPosition + enemy.forward * 40f;
        light.position = new Vector3(newPosition.x, newPosition.y + 1.1f, newPosition.z);

        // Rotate the camera to look at the enemy
        camera.LookAt(enemy.position + Vector3.up * 1f);
        light.LookAt(enemy.position + Vector3.up * 1f);

        spotlight.enabled = true;
        audioManager.PlaySFXWithVolume(jump_scare_scream, volume);


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
