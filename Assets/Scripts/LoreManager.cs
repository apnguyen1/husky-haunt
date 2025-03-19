using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using StarterAssets;

public class LoreManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loreScreen;
    public TextMeshProUGUI loreTitle;
    public TextMeshProUGUI loreText;

    [Header("Lore Settings")]
    [TextArea(5, 10)]
    public string loreTitleText = "HUSKY HAUNT";
    [TextArea(10, 20)]
    public string storyText = "You wake up in an abandoned asylum with no memory of how you got there. The only thing you remember is the legend of 'The Husky' - a deranged former doctor who still haunts these halls.\n\nAccording to the stories, The Husky collects tokens from his victims, and the only way to escape his territory is to find these tokens before he finds you.\n\nYou've heard his knocking. He knows you're here.\n\nFind all the tokens scattered throughout the asylum and escape through the entrance door before The Husky catches you. Run now!!!";

    [Header("Game References")]
    public EnemyMovement enemyMovement; // Reference to the enemy

    // Static flag to track if lore has been shown in this session
    private static bool hasLoreBeenShown = false;

    private CharacterController playerController;
    private StarterAssetsInputs playerInput;

    // Public getter to check if lore has been shown
    public static bool HasLoreBeenShown { get { return hasLoreBeenShown; } }

    private void Awake()
    {
        // Find the player controller
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        // Only show lore if it hasn't been shown yet in this session
        if (!hasLoreBeenShown)
        {
            ShowLoreScreen();
            hasLoreBeenShown = true;

            // If enemyMovement is assigned, update its delay time
            if (enemyMovement != null)
            {
                // Set longer delay for first playthrough
                enemyMovement.delayBeforeSpawn = 20f;
            }
        }
        else
        {
            // Explicitly hide the lore screen if it is active
            if (loreScreen.activeSelf)
            {
                loreScreen.SetActive(false);
            }

            // If lore has already been shown, use shorter delay
            if (enemyMovement != null)
            {
                enemyMovement.delayBeforeSpawn = 2f;
            }
        }
    }

    public void ShowLoreScreen()
    {
        // Enable the lore screen
        loreScreen.SetActive(true);

        // Set the text content
        loreTitle.text = loreTitleText;
        loreText.text = storyText;

        StartCoroutine(TypewriterEffect(loreText, storyText, 0.04f));

        // Disable player movement and input
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (playerInput != null)
        {
            playerInput.cursorInputForLook = false;
        }

        // Show cursor for button interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseLoreScreen()
    {
        // Hide the lore screen
        loreScreen.SetActive(false);

        // Re-enable player movement and input
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (playerInput != null)
        {
            playerInput.cursorInputForLook = true;
        }

        // Hide and lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Optional typewriter effect coroutine
    private IEnumerator TypewriterEffect(TextMeshProUGUI textComponent, string fullText, float delay)
    {
        textComponent.text = "";
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(delay);
        }

        // Ensure the full text is displayed at the end
        textComponent.text = fullText;

        yield return new WaitForSeconds(1f);

        CloseLoreScreen();
    }

    // Method to reset lore status (for testing or starting new game)
    public static void ResetLoreStatus()
    {
        hasLoreBeenShown = false;
    }
}