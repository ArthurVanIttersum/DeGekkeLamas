using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

public class CustomerGenerator : MonoBehaviour
{
    public RecipeBook book;
    public List<GameObject> customerPrefabs = new();
    [HideInInspector]public List <GameObject> customerQue = new();
    [HideInInspector]public List <GameObject> customerAntiQue = new();
    //when a customer is not in the que it's in the anti que.
    //this way i can pick a random customer from the anti que.
    [HideInInspector] public int availableCustomerCount;
    [Header("Spawn position")]
    public Vector2 spawnRangeX;
    public Vector2 spawnRangeY;
    public Vector2 spawnRangeZ;
    
    private GameObject newCustomer;

    /// <summary>
    /// Spawns next customer from the antiqueue a random, removing it fromm antiqueue and adding it to queue
    /// </summary>
    public void SpawnNewCustomer()
    {
        availableCustomerCount = customerPrefabs.Count - customerQue.Count;
        int randomCustomer = Random.Range(0, availableCustomerCount);
        Vector3 spawnPosition = new(
            Random.Range(spawnRangeX.x, spawnRangeX.y),
            Random.Range(spawnRangeY.x, spawnRangeY.y),
            Random.Range(spawnRangeZ.x, spawnRangeZ.y)
            );
        GameObject toSpawn = customerAntiQue[randomCustomer];
        newCustomer = Instantiate(toSpawn, spawnPosition, toSpawn.transform.rotation);
        customerQue.Add(newCustomer);
        customerAntiQue.Remove(customerAntiQue[randomCustomer]);
    }

    /// <summary>
    /// Assign generated customer an order
    /// </summary>
    public void GiveCustomerOrder()
    {
        //print(newCustomer);
        Order newlyGeneratedOrder = book.GenerateRandomOrder();
        //print(newlyGeneratedOrder);
        newCustomer.GetOrAddComponent<Customer>().thisCustomersOrder = newlyGeneratedOrder;
    }

    public bool CustomerInQue(GameObject customer)
    {
        for (int i = 0; i < customerQue.Count; i++)
        {
            if (customerQue[i] == customer)
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        customerAntiQue = customerPrefabs.ToList();
        StartCoroutine(TestSpawning());
        
    }

    /// <summary>
    /// Spawns until every customer is spawned, to test
    /// </summary>
    private IEnumerator TestSpawning()
    {
        int count = customerPrefabs.Count;
        
        for (int i = 0; i < count; i++)
        {
            SpawnNewCustomer();
            GiveCustomerOrder();
            yield return new WaitForSeconds(1);
            
        }
    }

    
}
