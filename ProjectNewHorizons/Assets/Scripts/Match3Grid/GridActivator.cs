using UnityEngine;

public class GridActivator : MonoBehaviour
{
    public GameObject gridObject;
    public static bool isPlayingMatch3;

    public void ToggleGame()
    {
        isPlayingMatch3 = !isPlayingMatch3;
        gridObject.SetActive(isPlayingMatch3);
    }
}
