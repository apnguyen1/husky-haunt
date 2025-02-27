using TMPro;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private int coinCount;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    [Tooltip("")]
    public float collectDistance = 5f;

    private void Start()
    {
        SetCountText();
    }

    private void Update()
    {
        // Look for coins within collection distance
        Collider[] nearbyCoins = Physics.OverlapSphere(transform.position, collectDistance);
        foreach (Collider coin in nearbyCoins)
        {
            if (coin.CompareTag("Coins"))
            {
                coin.gameObject.SetActive(false);
                coinCount += 1;
                SetCountText();
            }
        }
    }


    public void OnTriggerEnter(Collider other)
        {
            // Check if the object that entered the trigger has the "Coin" tag
            if (other.CompareTag("Coins"))
            {
                other.gameObject.SetActive(false);
                coinCount += 1;
                SetCountText();
            }
        }

        void SetCountText()
        {
            // Update the count text with the current count.
            countText.text = "Count: " + coinCount.ToString();
        }
    }
