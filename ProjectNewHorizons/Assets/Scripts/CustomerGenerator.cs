using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class CustomerGenerator : MonoBehaviour
{
    public List<GameObject> customerPrefabs = new();
    [HideInInspector]public List <GameObject> customerQue = new();
    [HideInInspector]public List <GameObject> customerAntiQue = new();
    //when a customer is not in the que it's in the anti que.
    //this way i can pick a random customer from the anti que.
    public int availableCustomers;

    public Vector3 spawnPosition;
    public void SpawnNewCustomer()
    {
        availableCustomers = customerPrefabs.Count - customerQue.Count;
        int randomCustomer = Random.Range(0, availableCustomers);
        GameObject newCustomer = Instantiate(customerAntiQue[randomCustomer], spawnPosition, Quaternion.identity);
        customerQue.Add(newCustomer);
        customerAntiQue.Remove(customerAntiQue[randomCustomer]);
        print(randomCustomer);
    }

    public bool CustomerInQue(GameObject teacher)
    {
        for (int i = 0; i < customerQue.Count; i++)
        {
            if (customerQue[i] == teacher)
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
        print(count);
        for (int i = 0; i < count; i++)
        {
            SpawnNewCustomer();
            yield return new WaitForSeconds(1);
            print("dshoahdfo");
        }
    }

    
}
