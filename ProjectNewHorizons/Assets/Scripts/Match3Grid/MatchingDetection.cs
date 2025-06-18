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
    private Vector3 endWorldPos;
    private bool swiping = false;
    private Vector2Int[] alldirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                startWorldPos = info.point;
            }
            swiping = true;
        }
        else if (swiping && Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                endWorldPos = info.point;
            }
            swiping = false;
            SwipeDetected();
        }
    }

    void SwipeDetected()
    {
        Vector2 direction = endWorldPos - startWorldPos;
        Vector2Int directionVector;
        

        if (direction.magnitude > 0.05f) // Threshold to ensure it’s a valid swipe
        {
            print("------");
            print("swipe is greater than magnitude");
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    print("right");
                    directionVector = Vector2Int.right;
                }
                else
                {
                    print("left");
                    directionVector = Vector2Int.left;
                }
                
            }
            else
            {
                if (direction.y > 0)
                {
                    print("up");
                    directionVector = Vector2Int.up;
                }
                else
                {
                    print("down");
                    directionVector = Vector2Int.down;
                }
            }
            
            Vector2Int startingPos = new Vector2Int((int)Mathf.Round(startWorldPos.x), (int)Mathf.Round(startWorldPos.y));

            Debug.Log($"startingPosition: {startingPos}");
            Debug.Log($"destination: {startingPos + directionVector}");

            TestMatch3(startingPos, directionVector);
        }
    }


    public bool TestMatch3(Vector2Int fromGridPos, Vector2Int direction)
    {
        
        Ingredient ingredientToMatch = grid.currentGrid[fromGridPos.y, fromGridPos.x];
        Vector2Int newPosition = fromGridPos + direction;
        Vector2Int opositeDirection = Vector2Int.zero - direction;
        List<Vector2Int> directionsToTest = alldirections.ToList();
        directionsToTest.Remove(opositeDirection);
        Vector2Int testpos;
        for (int i = 0; i < 3; i++)
        {
            testpos = newPosition + directionsToTest[i];
            bool found1 = SampleGrid(testpos, ingredientToMatch);
            testpos = newPosition + directionsToTest[i] * 2;
            bool found2 = SampleGrid(testpos, ingredientToMatch);
            if (found1 && found2)
            {
                print("found a match, type1");
                cookingEquipment.CurrentDish.points += TestOverkill(fromGridPos, directionsToTest[i], ingredientToMatch);//match
                cookingEquipment.CurrentDish.AddIngredient(ingredientToMatch);
            }
        }
        directionsToTest.Remove(direction);
        bool found = false;
        for (int i = 0; i < 2; i++)
        {
            testpos = newPosition + directionsToTest[i];
            if (SampleGrid(testpos, ingredientToMatch))
            {
                if (!found)
                {
                    found = true;
                }
                else
                {
                    found = false;
                    print("found a match, type2");
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
        if (SampleGrid(testPos, ingredientToMatch))
        {
            bonuspoints += 500;
        }
        if (SampleGrid(testPos, ingredientToMatch))
        {
            bonuspoints += 1000;
        }
        return bonuspoints;
    }

    private bool SampleGrid(Vector2Int position, Ingredient typeToTest)
    {
        if (position.x < 0)
        {
            return false;
        }
        if (position.x > grid.gridDimensions.x)
        {
            return false;
        }
        if (position.y < 0)
        {
            return false;
        }
        if (position.y > grid.gridDimensions.y)
        {
            return false;
        }
        //Debug.Log($"sampleing: {position}");
        //Debug.Log($"result: {grid.currentGrid[position.x, position.y].IndexEquals(typeToTest)}");
        //Debug.Log($"comparing this: {typeToTest.index}");
        //Debug.Log($"to this: {grid.currentGrid[position.x, position.y].index}");

        return grid.currentGrid[position.y, position.x].IndexEquals(typeToTest);
    }
}
