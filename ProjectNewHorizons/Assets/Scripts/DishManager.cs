using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DishManager : MonoBehaviour
{
    MatchGridSystem grid;
    public static DishManager instance;
    public int dishesRequired = 20;
    public TMP_Text dishText;
    [HideInInspector] public int dishesDone;
    [HideInInspector] public Dish currentDish;
    GameObject customer;
    int customerIndex;
    public UnityEvent onGameWon;
    private void Awake()
    {
        grid = GetComponent<MatchGridSystem>();
        if (instance == null) instance = this;
        UpdateDishText();
    }
    public void SetDish(Dish dish, (GameObject, int) customer)
    {
        this.customer = customer.Item1;
        customerIndex = customer.Item2;
        grid.SetDish(dish, dish.dishType.recipeIngredientsList.Length + MatchGridSystem.instance.extraIngredients);
        currentDish = dish;
        GridActivator.dishActive = true;
        StationHighlighter.instance.Highlight(dish.dishType.dishType);
    }

    public void CollectIngredient(Ingredient ingredient)
    {
        grid.CollectIngredient(ingredient);
        //currentDish.AddIngredient(ingredient);//match

        
    }

    public void TestFinished()
    {
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
        StationHighlighter.instance.RemoveHighlight();

        SM.IncreaseScore(SM.scoreDishComplete);
        GridActivator.dishActive = false;
        grid.ToggleUI();
        customer.GetComponent<Customer>().thisCustomersOrder.orderComplete = true;//assuming only one order per customer
    }

    public void WinGame()
    {
        onGameWon.Invoke();
        print("You won!");
    }

    public void DespawnAndRespawnCustomer(GameObject customer, int index)
    {
        CustomerGenerator CG = CustomerGenerator.instance;
        CG.customerAntiQue.Add(index);
        CG.customerQue.Remove(index);
        Destroy(customer);
        CG.SpawnNewCustomer();
    }
    public void UpdateDishText()
    {
        dishText.text = dishesDone > dishesRequired ?
            $"Dishes done: {dishesDone}" : $"Dishes done: {dishesDone} / {dishesRequired}";
    }
}
