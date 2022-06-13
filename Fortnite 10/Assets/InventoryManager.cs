using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class InventoryManager : MonoBehaviour
{
    public Inventory[] inventorys;

    public GameObject baseHud;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Toggle(" ", false);
        }

        baseHud.SetActive(!IsInInventory());
    }
    public void Toggle(string inventoryID, bool value)
    {
        for (int i = 0; i < inventorys.Length; i++)
        {
            Inventory inv = inventorys[i];
            if (inv.CompareId(inventoryID))
            {
                inv.Enable();
                break;
            }
            else if(inv.Enabled())
                inv.Disable();
        }

        //Set Cursor State

        if (value)
            Cursor.lockState = CursorLockMode.None;
        else 
            Cursor.lockState = CursorLockMode.Locked;
    }

    public int GetIndexOfInventory(string id)
    {
        int index = 0;
        for (int i = 0; i < inventorys.Length; i++)
        {
            if (inventorys[i].CompareId(id))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public bool IsInInventory()
    {
        for (int i = 0; i < inventorys.Length; i++)
        {
            if (inventorys[i].Enabled()) return true;
        }
        return false;
    }

    [System.Serializable]
    public struct Inventory
    {
        public string inventoryId;
        public GameObject gameObject;
        public Animator animator;
        public int value;

        public void Enable()
        {
            Toggle(true);
            animator.SetBool("Toggle",true);
        }
        public async void Disable()
        {
            animator.SetBool("Toggle",false);
            await Task.Delay(value);
            Toggle(false);
        }
        public void Toggle(bool value) => gameObject.SetActive(value);
        public bool CompareId(string id)
        {
            return id == inventoryId;
        }
        public bool Enabled()
        {
            return gameObject.activeInHierarchy;
        }
    }
}
