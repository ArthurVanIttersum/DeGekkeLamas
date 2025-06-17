using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Order
{
    public List<Dish> dishes = new();
    
    [HideInInspector] public int points = 0;
    [HideInInspector] public int amountOfCorrectIngredients = 0;
    [HideInInspector] public int amountOfInCorrectIngredients = 0;
    [HideInInspector] public int amountOfMissingIngredients = 0;
    

    public void EvaluateOrder()
    {
        for (int i = 0; i < dishes.Count; i++)
        {
            points += dishes[i].points;
            amountOfCorrectIngredients += dishes[i].amountOfCorrectIngredients;
            amountOfInCorrectIngredients += dishes[i].amountOfInCorrectIngredients;
            amountOfMissingIngredients += dishes[i].amountOfMissingIngredients;
        }
    }
    
}
