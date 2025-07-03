using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBookRecipe : MonoBehaviour
{
    [Serializable]
    public struct RecipePageData
    {
        public string name;
        public Sprite dishSprite;
        public Sprite[] ingredientsSprites;
        public string description;
    }
    public RecipePageData[] pages;
    public Image theDishImageRenderer;
    public Image[] theIngredientImageRenderers;
    public TMP_Text theNameText;
    public TMP_Text theDescriptionText;
    public int currentPage = 0;
    public Sprite defaultEmptySprite;

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
        Sprite ingredientSprite;
        for (int i = 0; i < theIngredientImageRenderers.Length; i++)
        {
            if (i >= pages[currentPage].ingredientsSprites.Length)
            {
                ingredientSprite = defaultEmptySprite;
            }
            else
            {
                ingredientSprite = pages[currentPage].ingredientsSprites[i];
            }
            theIngredientImageRenderers[i].sprite = ingredientSprite;
            
        }
        theNameText.text = pages[currentPage].name;
        theDescriptionText.text = pages[currentPage].description;
    }

    private void Start()
    {
        SetPage();
    }
}
