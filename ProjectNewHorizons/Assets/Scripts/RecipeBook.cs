using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "RecipeBook", menuName = "Scriptable Objects/RecipeBook")]
public class RecipeBook : ScriptableObject
{
    public List<Recipe> Allrecipes;
}
[Serializable]
public class Recipe
{
    [SerializeField]
    public string Name;
    public Ingredient[] recipeIngredientsList;

    public bool IngredientInRecipe(Ingredient ingredientToCompare)
    {
        for (int i = 0; i < recipeIngredientsList.Length; i++)
        {
            if (Ingredient.Equals(ingredientToCompare, recipeIngredientsList[i]))
            {
                return true;
            }
        }
        return false;
    }
}

