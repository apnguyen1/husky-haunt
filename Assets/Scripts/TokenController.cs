using UnityEngine;

public class TokenController : MonoBehaviour
{
    public bool isVisible = true;
    
    [Header("Visual Feedback")]
    public Material highlightMaterial; // Used when player looks at token
    
    [Header("Simple Highlight")]
    public bool useSpotHighlight = true;
    public Light spotLight;
    public Color spotColor = Color.white;
    public float spotIntensity = 0.1f;
    public float spotRange = 0.1f;
    
    private Material originalMaterial;
    private Renderer tokenRenderer;
    private bool isHighlighted = false;
    
    private void Start()
    {
        tokenRenderer = GetComponent<Renderer>();
        
        if (tokenRenderer)
        {
            originalMaterial = tokenRenderer.material;
        }
        
        // Set up spot highlight if enabled
        if (useSpotHighlight && spotLight == null)
        {
            // Create a child GameObject for the light
            GameObject lightObject = new GameObject("TokenSpotlight");
            lightObject.transform.SetParent(transform);
            lightObject.transform.localPosition = new Vector3(0, 0.5f, 0); // Position slightly above token
            
            // Add and configure the light component
            spotLight = lightObject.AddComponent<Light>();
            spotLight.type = LightType.Point;
            spotLight.color = spotColor;
            spotLight.intensity = spotIntensity;
            spotLight.range = spotRange;
            
            // Set light to render in URP if using that pipeline
            spotLight.renderMode = LightRenderMode.ForcePixel;
        }
    }

    public void SetHighlight(bool highlight)
    {
        if (tokenRenderer && highlightMaterial && highlight != isHighlighted)
        {
            isHighlighted = highlight;
            tokenRenderer.material = highlight ? highlightMaterial : originalMaterial;
        }
    }

    public void CollectCoin()
    {
        if (isVisible)
        {
            // Make the token invisible
            isVisible = false;
            
            // Disable renderer to make it disappear
            if (tokenRenderer)
            {
                tokenRenderer.enabled = false;
            }
            
            // Disable the spotlight if it exists
            if (spotLight)
            {
                spotLight.enabled = false;
            }
            
            // Disable collider so it can't be detected by raycasts anymore
            Collider tokenCollider = GetComponent<Collider>();
            if (tokenCollider)
            {
                tokenCollider.enabled = false;
            }
            
            // Notify the game manager that a token has been collected
            TokenManager.Instance?.TokenCollected();
        }
    }
}