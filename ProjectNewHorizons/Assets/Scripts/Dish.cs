using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dish
{
    
    public Recipe dishType;
    private List<Ingredient> currentIngredients = new();
    public int points;
    public int amountOfCorrectIngredients;
    public int amountOfInCorrectIngredients;
    public int amountOfMissingIngredients;

    public bool IngredientInDish(Ingredient ingredientToCompare)
    {
        for (int i = 0; i < currentIngredients.Count; i++)
        {
            if (Ingredient.Equals(ingredientToCompare, currentIngredients[i]))
            {
                return true;
            }
        }
        return false;
    }
    public void EvaluateDish()
    {
        for (int i = 0; i < currentIngredients.Count; i++)//looking at the plate
        {
            if (dishType.IngredientInRecipe(currentIngredients[i]))
            {
                amountOfCorrectIngredients++;
            }
            else
            {
                amountOfInCorrectIngredients++;
            }

        }
        for (int i = 0; i < dishType.recipeIngredientsList.Length; i++)//looking at the recipe
        {
            if (IngredientInDish(dishType.recipeIngredientsList[i]))
            {

            }
            else
            {
                amountOfMissingIngredients++;
            }
        }
    }
    public void AddIngredient(Ingredient IngredientToAdd)
    {
        if (IngredientInDish(IngredientToAdd))
        {
            points += 1000;
        }
        else
        {
            currentIngredients.Add(IngredientToAdd);
        }
    }
}
