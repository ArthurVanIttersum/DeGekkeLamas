using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    public MatchGridSystem matchGridSystem;

    /// <summary>
    /// Use this variable to lock player moment and interactions when in match3 minigame
    /// </summary>
    [HideInInspector] public bool movementLocked;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (movementLocked || GridActivator.isPlayingMatch3) return;

        // Touchscreen is also considered mouse button 0
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from camera position to mouse pos
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                // Interact instead of setting destination if interactions
                if (info.collider.gameObject.CompareTag("Interactible"))
                {
                    // Get order from customer
                    if (info.collider.gameObject.TryGetComponent(out Customer customer))
                    {
                        List<Dish> dishes = customer.thisCustomersOrder.dishes;
                        if (matchGridSystem != null) matchGridSystem.SetDish(dishes[0], dishes.Count+2);
                        Debug.Log("Received order from customer");
                    }
                    // Open oven for match3 minigame
                    else if (info.collider.gameObject.TryGetComponent(out GridActivator activator))
                    {
                        activator.ToggleGame();
                        print("Clicked on grid activator");
                    }
                    // Old unused UI version of grid
                    else if (info.collider.gameObject.TryGetComponent(out GridUIRenderer renderer))
                    {
                        print("Clicked on grid UI generator");
                        renderer.GenerateUI();
                    }


                }
                else agent.SetDestination(info.point);
            }
        }
    }
}
