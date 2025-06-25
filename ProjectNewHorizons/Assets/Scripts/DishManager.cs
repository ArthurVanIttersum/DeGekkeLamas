using UnityEngine;

public class DishManager : MonoBehaviour
{
    MatchGridSystem grid;
    public static DishManager instance;
    public int dishesRequired = 20;
    int dishesDone;
    [HideInInspector] public Dish currentDish;
    GameObject customer;
    int customerIndex;
    private void Awake()
    {
        grid = GetComponent<MatchGridSystem>();
        if (instance == null) instance = this;
    }
    public void SetDish(Dish dish, (GameObject, int) customer)
    {
        this.customer = customer.Item1;
        customerIndex = customer.Item2;
        grid.SetDish(dish, dish.dishType.recipeIngredientsList.Length + 4);
        currentDish = dish;
        GridActivator.dishActive = true;
    }

    public void CollectIngredient(Ingredient ingredient)
    {
        grid.CollectIngredient(ingredient);
        currentDish.AddIngredient(ingredient);//match

        if (currentDish.dishType.recipeIngredientsList.Length == currentDish.currentIngredients.Count)
        {
            CompleteDish();
        }
    }

    /// <summary>
    /// Called when all required ingredients are collected
    /// </summary>
    void CompleteDish()
    {
        print("Completed dish!");
        ScoreManager SM = ScoreManager.instance;
        CustomerGenerator CG = CustomerGenerator.instance;

        SM.IncreaseScore(SM.scoreDishComplete);
        GridActivator.dishActive = false;
        GridActivator.isPlayingMatch3 = false;
        grid.ToggleUI();
        CG.customerAntiQue.Add(customerIndex);
        CG.customerQue.Remove(customerIndex);
        Destroy(customer);
        CG.SpawnNewCustomer();
        dishesDone++;

        if (dishesDone >= dishesRequired) WinGame();
    }

    void WinGame()
    {
        print("You won!");
    }
}
