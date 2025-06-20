using UnityEngine;

public class ScreenResolutionManager : MonoBehaviour
{
    public RenderTexture renderTexture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderTexture.width = Screen.width;
        renderTexture.height = Screen.height;
    }

    
}
