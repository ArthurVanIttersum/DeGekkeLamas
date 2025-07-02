using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBookRecipe : MonoBehaviour
{
    [Serializable]
    public struct RecipePageData
    {
        public Sprite dishSprite;
        public Sprite[] ingredientsSprites;
        public string name;
        public string description;
    }
    public RecipePageData[] pages;
    public Image theDishImageRenderer;
    public Image[] theIngredientImageRenderers;
    public TMP_Text theNameText;
    public TMP_Text theDescriptionText;
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
        theDishImageRenderer.sprite = pages[currentPage].dishSprite;
        for (int i = 0; i < pages[currentPage].ingredientsSprites.Length; i++)
        {
            theIngredientImageRenderers[i].sprite = pages[currentPage].ingredientsSprites[i];
        }
        theNameText.text = pages[currentPage].name;
        theDescriptionText.text = pages[currentPage].description;
    }

    private void Start()
    {
        SetPage();
    }
}
