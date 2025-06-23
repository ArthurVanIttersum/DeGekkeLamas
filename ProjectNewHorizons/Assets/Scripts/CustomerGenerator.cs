using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

public class CustomerGenerator : MonoBehaviour
{
    public RecipeBook book;
    public List<GameObject> customerPrefabs = new();
    [HideInInspector]public List <int> customerQue = new();
    [HideInInspector]public List <int> customerAntiQue = new();
    //when a customer is not in the que it's in the anti que.
    //this way i can pick a random customer from the anti que.
    [Header("Spawn position")]
    public Vector2 spawnRangeX;
    public Vector2 spawnRangeY;
    public Vector2 spawnRangeZ;
    
    private GameObject newCustomer;
    public static CustomerGenerator instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    /// <summary>
    /// Spawns next customer from the antiqueue a random, removing it fromm antiqueue and adding it to queue
    /// </summary>
    public void SpawnNewCustomer()
    {
        int randomCustomer = Random.Range(0, customerAntiQue.Count);
        Vector3 spawnPosition = new(
            Random.Range(spawnRangeX.x, spawnRangeX.y),
            Random.Range(spawnRangeY.x, spawnRangeY.y),
            Random.Range(spawnRangeZ.x, spawnRangeZ.y)
            );
        GameObject toSpawn = customerPrefabs[customerAntiQue[randomCustomer]];
        newCustomer = Instantiate(toSpawn, spawnPosition, toSpawn.transform.rotation);
        newCustomer.name = $"customer number {customerAntiQue[randomCustomer]}";
        customerQue.Add(randomCustomer);
        customerAntiQue.Remove(customerAntiQue[randomCustomer]);

        GiveCustomerOrder(randomCustomer);
    }

    /// <summary>
    /// Assign generated customer an order
    /// </summary>
    public void GiveCustomerOrder(int index)
    {
        //print(newCustomer);
        Order newlyGeneratedOrder = book.GenerateRandomOrder();
        //print(newlyGeneratedOrder);
        Customer c = newCustomer.GetOrAddComponent<Customer>();
        c.thisCustomersOrder = newlyGeneratedOrder;
        c.index = index;
    }

    public bool CustomerInQue(GameObject customer)
    {
        for (int i = 0; i < customerQue.Count; i++)
        {
            if (customerPrefabs[customerQue[i]] == customer)
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        customerAntiQue = new();
        for (int i = 0;i < customerPrefabs.Count; i++)
        {
            customerAntiQue.Add(i);
        }
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
            yield return new WaitForSeconds(1);
            
        }
    }

    
}
