using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Dish
{
    
    public Recipe dishType;
    public List<Ingredient> currentIngredients = new();
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
    public void AddIngredient(Ingredient ingredientToAdd)
    {
        ScoreManager SM = ScoreManager.instance;
        // If ingredient doesn't belong to dish, subtract score
        if (dishType.IngredientInRecipe(ingredientToAdd))
        {
            // Check if ingredient is collected already
            if (Ingredient.ContainsName(currentIngredients.ToArray(), ingredientToAdd))
            {
                SM.IncreaseScore(SM.scoreIngredientCorrect);
            }
            else
            {
                SM.IncreaseScore(SM.scoreIngredientCorrect);
                currentIngredients.Add(ingredientToAdd);
            }
        }
        else
        {
            //MonoBehaviour.print($"{ingredientToAdd.name}, {Ingredient.ContainsName(dishType.recipeIngredientsList, ingredientToAdd)}," + 
            //    $"{StringTools.IngredientArrayToString(dishType.recipeIngredientsList)}");
            SM.IncreaseScore(SM.scoreIngredientIncorrect);
        }
    }
}
