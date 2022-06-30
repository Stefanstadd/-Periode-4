using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStation : MonoBehaviour
{
    public Description description;
    public InventoryManager inventoryManager;
    public PlayerMovement target;
    public float interactDst;

    Animator animator;

    const string toggleAnimation = "Toggle";
    private void Start()
    {
        target = PlayerMovement.player;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float dst = Vector3.Distance(target.transform.position, transform.position);
        if (dst < interactDst && !inventoryManager.IsInInventory("Upgrade")) // the player is near a upgrade station and can interact with it
        {
            description.Enable(transform.position);
            Description.onInteract = delegate{ inventoryManager.Toggle("Upgrade", true); };
            animator.SetBool(toggleAnimation,true);
        }
        else if(dst > interactDst && inventoryManager.IsInInventory("Upgrade"))//the inventory closes and the player cannot interact from this distance
        {
            inventoryManager.ToggleOff();
        }
        else
        {
            if (dst > interactDst && !inventoryManager.IsInInventory("Upgrade"))// disabled the station and plays the closing animation
            {
                animator.SetBool(toggleAnimation, false);
                description.Disable();
            }
        }
    }
}
