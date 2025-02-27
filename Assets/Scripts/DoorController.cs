using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false;
    public bool isLocked = false;
    public float openAngle = 90.0f;
    public float openSpeed = 2.0f;
    
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine animationCoroutine;
    
    void Start()
    {
        // Store the initial rotation as the closed state
        closedRotation = transform.rotation;
        
        // Calculate the open rotation based on the specified angle
        openRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + openAngle,
            transform.eulerAngles.z
        );
    }
    
    public void ToggleDoor()
    {
        if (isLocked) return;
        
        isOpen = !isOpen;
        
        // Stop any existing animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        // Start a new animation
        animationCoroutine = StartCoroutine(AnimateDoor(isOpen ? openRotation : closedRotation));
    }
    
    private IEnumerator AnimateDoor(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0;
        
        while (elapsedTime < 1.0f)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime * openSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure the door reaches the exact target rotation
        transform.rotation = targetRotation;
    }
    
    // Method to set a door's locked state
    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }
}