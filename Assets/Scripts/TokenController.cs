using UnityEngine;

public class TokenController : MonoBehaviour
{
    public bool isVisible = true;

    public void CollectCoin()
    {
        if (isVisible)
        {
            enabled=false;
            isVisible = false;
        }
    }
}