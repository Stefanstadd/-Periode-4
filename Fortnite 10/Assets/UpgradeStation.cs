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
        int index = inventoryManager.GetIndexOfInventory("Upgrade");
        float dst = Vector3.Distance(target.transform.position, transform.position);
        if (dst < interactDst && !inventoryManager.inventorys[index].Enabled())
        {
            description.Enable(transform.position);
            Description.onInteract = delegate{ inventoryManager.Toggle("Upgrade", true); };
            animator.SetBool(toggleAnimation,true);
        }
        else if(dst > interactDst && inventoryManager.inventorys[index].Enabled())
        {
            inventoryManager.Toggle(" ", false);
        }
        else
        {
            if (dst > interactDst && !inventoryManager.inventorys[index].Enabled())
            {
                animator.SetBool(toggleAnimation, false);
            }
            description.Disable();
        }
    }
}
