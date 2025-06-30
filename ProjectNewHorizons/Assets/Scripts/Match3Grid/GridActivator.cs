using UnityEngine;

/// <summary>
/// Means to be put on the stove, playercontroller activates this when clicked
/// </summary>
public class GridActivator : MonoBehaviour
{
    public static bool isPlayingMatch3;
    public static bool dishActive;
    public DishType stationType;
    public GameObject physicalObject;

    public void ToggleGame()
    {
        if (dishActive)
        {
            MatchGridSystem.instance.ToggleUI();
        }
    }
}
