using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Events;

public class CustomerGenerator : MonoBehaviour
{
    public RecipeBook book;
    public List<GameObject> customerPrefabs = new();
    public List <int> customerQue = new();
    public List <int> customerAntiQue = new();
    //when a customer is not in the que it's in the anti que.
    //this way i can pick a random customer from the anti que.
    public int customerCount = 3;
    public int possibleSpots = 5;
    [Header("Spawn position")]
    public Vector2Int spawnRangeX;
    public Vector2Int spawnRangeY;
    public Vector2Int spawnRangeZ;
    [HideInInspector] public List<int> possiblePositions;
    
    private GameObject newCustomer;
    public static CustomerGenerator instance;
    GameObject customerContainer;

    [Header("Events")]
    public UnityEvent onOrderTaken;
    public UnityEvent onOrderGiven;

    [Header("Speechbubbe stuff")]
    public GameObject speechbubblePrefab;
    public Sprite speechBubbleSprite;
    public Sprite thoughtBubbleSprite;
    public Sprite waitingSprite;
    public Sprite satisfiedSprite;

    public Material highlight;
    List<MeshRenderer> highlights = new();

    public static bool firstOrderReceived;
    private void Awake()
    {
        if (instance == null) instance = this;
        possiblePositions = new();
        for(int i = 0; i < possibleSpots; i++)
        {
            possiblePositions.Add(i);
        }
    }
    private void OnValidate()
    {
        customerCount = Mathf.Min(customerCount, customerPrefabs.Count-1);
        customerCount = Mathf.Clamp(customerCount, 0, possibleSpots);
    }

    /// <summary>
    /// Spawns next customer from the antiqueue a random, removing it fromm antiqueue and adding it to queue
    /// </summary>
    public void SpawnNewCustomer()
    {
        int randomCustomer = Random.Range(0, customerAntiQue.Count);

        int spawnIndex = Random.Range(0, possiblePositions.Count);
        int spawnPos = possiblePositions[spawnIndex];
        //print($"0, {possiblePositions.Count}, Index = {spawnIndex}, spawnPos = {spawnPos}");

        Vector3 spawnPosition = new(
            MathTools.Remap(0, possibleSpots, spawnRangeX.x, spawnRangeX.y, spawnPos),
            MathTools.Remap(0, possibleSpots, spawnRangeY.x, spawnRangeY.y, spawnPos),
            MathTools.Remap(0, possibleSpots, spawnRangeZ.x, spawnRangeZ.y, spawnPos)
            );
        possiblePositions.RemoveAt(spawnIndex);

        GameObject toSpawn = customerPrefabs[customerAntiQue[randomCustomer]];
        newCustomer = Instantiate(toSpawn, spawnPosition, toSpawn.transform.rotation, customerContainer.transform);
        newCustomer.name = $"customer number {customerAntiQue[randomCustomer]}";

        if (!firstOrderReceived)
        {
            // Highlight if no order taken before
            MeshRenderer highlight = Instantiate(newCustomer, newCustomer.transform.position, 
                newCustomer.transform.rotation, newCustomer.transform).GetComponent<MeshRenderer>();
            highlight.material = this.highlight;
            highlight.transform.localScale = Vector3.one;
            highlight.material.SetTexture("_Map", newCustomer.GetComponent<MeshRenderer>().material.mainTexture);
            Destroy(highlight.GetComponent<Collider>());
            highlights.Add(highlight);
        }

        GiveCustomerOrder(customerAntiQue[randomCustomer], spawnPos);
        customerQue.Add(customerAntiQue[randomCustomer]);
        customerAntiQue.Remove(customerAntiQue[randomCustomer]);

    }

    public void RemoveHighLights()
    {
        for (int i = highlights.Count-1; i >= 0; i--)
        {
            Destroy(highlights[i]);
            highlights.RemoveAt(i);
        }
    }

    /// <summary>
    /// Assign generated customer an order
    /// </summary>
    public void GiveCustomerOrder(int index, int posIndex)
    {
        //print(newCustomer);
        Order newlyGeneratedOrder = book.GenerateRandomOrder();
        //print(newlyGeneratedOrder);
        Customer c = newCustomer.GetOrAddComponent<Customer>();
        c.thisCustomersOrder = newlyGeneratedOrder;
        c.index = index;
        c.positionIndex = posIndex;
        c.walkingAwayDirectionAndSpeed = new Vector3 (0.1f, 0, 0);
        c.walkingTime = 2f;
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
        customerContainer = new("Customer container");

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
        for (int i = 0; i < customerCount; i++)
        {
            SpawnNewCustomer();
            yield return new WaitForSeconds(1);
        }
    }

    
}
