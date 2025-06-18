using UnityEngine;

[RequireComponent (typeof(MatchGridSystem))]
public class DishToGridTester : MonoBehaviour
{
    public Dish dish;
    public int total;

    MatchGridSystem matchGridSystem;

    [NaughtyAttributes.Button]
    void SetGrid()
    {
        matchGridSystem = this.GetComponent<MatchGridSystem>();
        matchGridSystem.SetDish(dish, total);
    }
}
