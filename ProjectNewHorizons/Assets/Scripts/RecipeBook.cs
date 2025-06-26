
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;



[CreateAssetMenu(fileName = "RecipeBook", menuName = "Scriptable Objects/RecipeBook")]
public class RecipeBook : ScriptableObject
{
    public int randomDishCountRangeMin;
    public int randomDishCountRangeMax;
    public List<Recipe> allrecipes;

    /// <summary>
    /// Returns a random order with a random amount of dishes
    /// </summary>
    public Order GenerateRandomOrder()
    {
        Order newOrder = new();
        int randomDishCount = UnityEngine.Random.Range(randomDishCountRangeMin, randomDishCountRangeMax);
        for (int i = 0; i < randomDishCount; i++)
        {
            int randomDish = UnityEngine.Random.Range(0, allrecipes.Count);
            Dish newDish = new Dish();
            newDish.dishType = allrecipes[randomDish];
            newOrder.dishes.Add(newDish);
        }
        return newOrder;
    }
}
[Serializable]
public class Recipe
{
    [SerializeField]
    public string name;
    public Texture texture;
    public Ingredient[] recipeIngredientsList;
    public Sprite spriteForPopup;

    /// <summary>
    /// Checks if name of ingredient exists in list of ingredients for recipe
    /// </summary>
    public bool IngredientInRecipe(Ingredient ingredientToCompare)
    {
        for (int i = 0; i < recipeIngredientsList.Length; i++)
        {
            if (ingredientToCompare.name == recipeIngredientsList[i].name)
            {
                return true;
            }
        }
        return false;
    }
}

