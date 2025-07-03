using UnityEngine;
using UnityEngine.SceneManagement;
public class UIButtons : MonoBehaviour
{
    public void StartGame()
    {
        //Loads the main scene
        GridActivator.isPlayingMatch3 = default;
        GridActivator.dishActive = default;
        MatchingDetection.isPaused = default;
        SceneManager.LoadScene("KitchenBlockout");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void MainMenuReturn()
    {
        SceneManager.LoadScene("Start Menu Scene");
    }
}
