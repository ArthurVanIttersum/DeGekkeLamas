using UnityEngine;
using UnityEngine.SceneManagement;
public class UIButtons : MonoBehaviour
{
    public void StartGame()
    {
        //Loads the main scene
        SceneManager.LoadScene("KitchenBlockout");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
