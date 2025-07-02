using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBook : MonoBehaviour
{
    [Serializable]
    public struct PageData
    {
        public Sprite customerSprite;
        public string name;
        public string description;
    }
    public PageData[] pages;
    public Image theImageRenderer;
    public Text theNameText;
    public Text theDescriptionText;
    public int currentPage = 0;
    
    public void TurnPageRight()
    {
        if (currentPage != pages.Length - 1)
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
        theImageRenderer.sprite = pages[currentPage].customerSprite;
        theNameText.text = pages[currentPage].name;
        theDescriptionText.text = pages[currentPage].description;
    }
}
