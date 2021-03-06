using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMenuManager : MonoBehaviour
{

    public InventoryManager inventory;
    public bool InMenu 
    {
        get
        {
            return gameObject.activeInHierarchy;
        }    
    }

    public static bool Paused { get; private set; }

    private void Start()
    {
        OnChangeMenuState(false);
    }
    public void UpdateMenu()
    {
        if (inventory != null && inventory.IsInInventory()|| PlayerMovement.Dead) return;

        if (Input.GetButtonDown("Escape"))
        {
            OnChangeMenuState(!InMenu);
        }

        if (!gameObject.activeInHierarchy) return;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void OnChangeMenuState(bool active)
    {
        Paused = active;

        gameObject.SetActive(active);

        Cursor.lockState = active ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = active;

        Time.timeScale = active ? 0 : 1;
    }
}
