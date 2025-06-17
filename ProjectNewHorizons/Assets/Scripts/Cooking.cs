using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Cooking : MonoBehaviour
{
    private List <Ingredient> currentIngredients = new();
    public Recipe currentlyMaking;
    public DishStats currentStats;
    public struct DishStats
    {
        public int points;
        public int amountOfCorrectIngredients;
        public int amountOfInCorrectIngredients;
        public int amountOfMissingIngredients;
    }



    private bool IngredientInPan(Ingredient ingredientToCompare)
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
    private void TestDishInPan()
    {
        for (int i = 0; i < currentIngredients.Count; i++)//looking at the pan
        {
            if (currentlyMaking.IngredientInRecipe(currentIngredients[i]))
            {
                currentStats.amountOfCorrectIngredients++;
            }
            else
            {
                currentStats.amountOfInCorrectIngredients++;
            }

        }
        for (int i = 0; i < currentlyMaking.recipeIngredientsList.Length; i++)//looking at the recipe
        {
            if (IngredientInPan(currentlyMaking.recipeIngredientsList[i]))
            {
                
            }
            else
            {
                currentStats.amountOfMissingIngredients++;
            }
        }
    }

    public void AddIngredient(Ingredient IngredientToAdd)
    {
        if (IngredientInPan(IngredientToAdd))
        {
            currentStats.points += 1000;
        }
        else
        {
            currentIngredients.Add(IngredientToAdd);
        }
    }

    public void CompleteDish()
    {
        TestDishInPan();
        //use currentStats to award points
        InitializeDish();
    }

    public void InitializeDish()
    {
        DishStats initialize = new DishStats();
        currentStats = initialize;
    }

}
