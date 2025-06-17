using System.Collections.Generic;

using UnityEngine;

public class CookingEquipment : MonoBehaviour
{
    public Dish CurrentDish;
    
    
    public void CompleteDish()
    {
        CurrentDish.EvaluateDish();

        //use CurrentDish to award points
        ClearDish();
    }

    public void ClearDish()
    {
        Dish ClearDish = new Dish();
        CurrentDish = ClearDish;
    }

    public void SetDish(Dish newDish)
    {
        CurrentDish = newDish;
    }
}
