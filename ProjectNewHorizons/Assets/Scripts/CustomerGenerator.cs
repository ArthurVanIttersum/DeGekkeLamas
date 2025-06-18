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
    public Vector3 spawnPosition;
    
    private GameObject newCustomer;
    public void SpawnNewCustomer()
    {
        availableCustomerCount = customerPrefabs.Count - customerQue.Count;
        int randomCustomer = Random.Range(0, availableCustomerCount);
        newCustomer = Instantiate(customerAntiQue[randomCustomer], spawnPosition, Quaternion.identity);
        customerQue.Add(newCustomer);
        customerAntiQue.Remove(customerAntiQue[randomCustomer]);
    }

    public void GiveCustomerOrder()
    {
        print(newCustomer);
        Order newlyGeneratedOrder = book.GenerateRandomOrder();
        print(newlyGeneratedOrder);
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
