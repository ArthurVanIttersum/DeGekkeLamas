using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MatchingDetection : MonoBehaviour
{
    public CookingEquipment cookingEquipment;
    public Camera mainCamera; // Assign the main camera
    public MatchGridSystem grid;
    private Vector3 startWorldPos;
    private Vector2 startScreenPos;
    private Vector2 endScreenPos;
    private bool swiping = false;
    private Vector2Int[] alldirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startScreenPos = Input.mousePosition;
            startWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(startScreenPos.x, startScreenPos.y, mainCamera.nearClipPlane));
            swiping = true;
        }
        else if (swiping && Input.GetMouseButtonUp(0))
        {
            endScreenPos = Input.mousePosition;
            swiping = false;
            SwipeDetected();
        }
    }

    void SwipeDetected()
    {
        Vector2 direction = endScreenPos - startScreenPos;
        Debug.Log($"Swipe started at world position: {startWorldPos}");
        Vector2Int directionVector;


        if (direction.magnitude > 50) // Threshold to ensure it’s a valid swipe
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    directionVector = Vector2Int.right;
                }
                else
                {
                    directionVector = Vector2Int.left;
                }
                
            }
            else
            {
                if (direction.y > 0)
                {
                    directionVector = Vector2Int.up;
                }
                else
                {
                    directionVector = Vector2Int.down;
                }
            }
            TestMatch3(new Vector2Int((int)startScreenPos.x, (int)startScreenPos.y), directionVector);
        }
    }


    public bool TestMatch3(Vector2Int fromGridPos, Vector2Int direction)
    {
        Ingredient ingredientToMatch = grid.currentGrid[fromGridPos.x, fromGridPos.y];
        Vector2Int newPosition = fromGridPos + direction;
        Vector2Int opositeDirection = Vector2Int.zero - direction;
        List<Vector2Int> directionsToTest = alldirections.ToList();
        directionsToTest.Remove(opositeDirection);
        Vector2Int testpos;
        for (int i = 0; i < 3; i++)
        {
            testpos = newPosition + directionsToTest[i];
            bool found1 = grid.currentGrid[testpos.x, testpos.y].IndexEquals(ingredientToMatch);
            testpos = newPosition + directionsToTest[i] * 2;
            bool found2 = grid.currentGrid[testpos.x, testpos.y].IndexEquals(ingredientToMatch);
            if (found1 && found2)
            {
                cookingEquipment.CurrentDish.points += TestOverkill(fromGridPos, directionsToTest[i], ingredientToMatch);//match
                cookingEquipment.CurrentDish.AddIngredient(ingredientToMatch);
            }
        }
        directionsToTest.Remove(direction);
        bool found = false;
        for (int i = 0; i < 2; i++)
        {
            testpos = newPosition + directionsToTest[i];
            if (grid.currentGrid[testpos.x, testpos.y].IndexEquals(ingredientToMatch))
            {
                if (!found)
                {
                    found = true;
                }
                else
                {
                    cookingEquipment.CurrentDish.AddIngredient(ingredientToMatch);//match
                }
            }
        }
        return false;
    }

    private int TestOverkill(Vector2Int fromGridPos, Vector2Int direction, Ingredient ingredientToMatch)
    {
        Vector2Int opositeDirection = Vector2Int.zero - direction;
        Vector2Int testPos = fromGridPos + opositeDirection;
        int bonuspoints = 0;
        if (grid.currentGrid[testPos.x, testPos.y].IndexEquals(ingredientToMatch))
        {
            bonuspoints += 500;
        }
        if (grid.currentGrid[testPos.x, testPos.y].IndexEquals(ingredientToMatch))
        {
            bonuspoints += 1000;
        }
        return bonuspoints;
    }
}
