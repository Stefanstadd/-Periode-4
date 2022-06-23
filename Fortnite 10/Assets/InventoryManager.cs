using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class InventoryManager : MonoBehaviour
{
    public Inventory[] inventorys;

    public UIButton inventoryButton;
    public GameMenuManager gameMenu;


    public GameObject baseHud;

    private void Update()
    {
        CheckInventory();

        //zet de state van de main HUD
        baseHud.SetActive(!IsInInventory());
    }
    public void Toggle(string inventoryID, bool value)
    {
        print("Toggle");
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

    public void ToggleOff()
    {
        print("Toggle Off");
        for (int i = 0; i < inventorys.Length; i++)
        {
            inventorys[i].Disable();
        }
        Cursor.lockState = CursorLockMode.Locked;
    }



    void CheckInventory()
    {
        if (CanOpenInventory() && Input.GetButtonDown("TAB"))
        {
            inventoryButton.OnUseButton();
            Toggle("Inventory", true);
        }
        else if (IsInInventory() && Input.GetButtonDown("TAB") || Input.GetButtonDown("Escape"))
        {
            ToggleOff();
        }
        
    }

    bool CanOpenInventory()
    {
        if (gameMenu.InMenu) return false;
        if (IsInInventory()) return false;
        return true;
    }

    public bool IsInInventory()
    {
        for (int i = 0; i < inventorys.Length; i++)
        {
            if (inventorys[i].Enabled()) return true;
        }
        return false;
    }

    public bool IsInInventory(string inventoryID)
    {
        for (int i = 0; i < inventorys.Length; i++)
        {
            if (inventorys[i].CompareId(inventoryID))
            {
                if (inventorys[i].Enabled())
                    return true;
                else 
                    return false;
            }
        }
        return false;
    }

    [System.Serializable]
    public struct Inventory
    {
        public string inventoryId;
        [SerializeField] GameObject gameObject;
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
