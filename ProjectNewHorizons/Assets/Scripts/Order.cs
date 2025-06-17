using UnityEngine;

public class Order : MonoBehaviour
{
    public Dish[] dishes;
    public int points = 0;
    public int amountOfCorrectIngredients = 0;
    public int amountOfInCorrectIngredients = 0;
    public int amountOfMissingIngredients = 0;


    public void EvaluateOrder()
    {
        for (int i = 0; i < dishes.Length; i++)
        {
            points += dishes[i].points;
            amountOfCorrectIngredients += dishes[i].amountOfCorrectIngredients;
            amountOfInCorrectIngredients += dishes[i].amountOfInCorrectIngredients;
            amountOfMissingIngredients += dishes[i].amountOfMissingIngredients;
        }
    }
}
