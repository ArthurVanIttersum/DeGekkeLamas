using UnityEngine;
using UnityEngine.UI;

public class ScreenResolutionManager : MonoBehaviour
{
    public RenderTexture renderTexture;
    public Camera outputCam;
    public RawImage targetTexture;

    Vector2 lastScreensize;
    void Awake()
    {
        lastScreensize = new(Screen.width, Screen.height);
        ResizeTexture();
    }

    private void Update()
    {
        Vector2 currectScreenSize = new(Screen.width, Screen.height);
        if (currectScreenSize != lastScreensize)
        {
            lastScreensize = currectScreenSize;
            ResizeTexture();
            print("Updated resolution");
        }
    }
    void ResizeTexture()
    {
        renderTexture.Release();
        renderTexture.width = Screen.width;
        renderTexture.height = Screen.height;
    }
}
