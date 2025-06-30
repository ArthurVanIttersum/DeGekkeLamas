using UnityEngine;
using UnityEngine.UI;

public class UIBook : MonoBehaviour
{
    public Sprite[] pages;
    public Image theImageRenderer;
    public int currentPage = 0;
    
    public void TurnPageRight()
    {
        if (currentPage != pages.Length)
        {
            currentPage++;
            SetPage();
        }
    }
    public void TurnPageLeft()
    {
        if (currentPage != 0)
        {
            currentPage--;
            SetPage();
        }
    }

    private void SetPage()
    {
        theImageRenderer.sprite = pages[currentPage];
    }
}
