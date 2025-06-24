using UnityEngine;

/// <summary>
/// Means to be put on the stove, playercontroller activates this when clicked
/// </summary>
public class GridActivator : MonoBehaviour
{
    public static bool isPlayingMatch3;
    public static bool dishActive;

    public void ToggleGame()
    {
        if (dishActive)
        {
            isPlayingMatch3 = !isPlayingMatch3;
            MatchGridSystem.instance.ToggleUI();
        }
    }
}
