using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public BaseWeapon[] weapons;
    InventoryUIButton[] uIButtons;

    [Header("Buttons")]

    public Transform buttonsParent;
    public GameObject buttonPrefab;
    public Vector3 startPos;
    public float spaceBtwn;

    private void Start()
    {
        CreateButtons();
    }


    void CreateButtons()
    {
        uIButtons = new InventoryUIButton[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            Vector3 pos = startPos + new Vector3(spaceBtwn * i, 0f, 0f);

            uIButtons[i] = Instantiate(buttonPrefab, pos, Quaternion.identity, buttonsParent).GetComponent<InventoryUIButton>();
            if (uIButtons[i])
            {
                uIButtons[i].Init(weapons[i]);
            }
        }
    }

    private void Update()
    {
        UpdateButtons();
    }

    void UpdateButtons()
    {
        InventoryUIButton hoveredButton = null;
        float closestDst = float.MaxValue;

        var mousePos = Input.mousePosition;
        for (int i = 0; i < uIButtons.Length; i++)
        {
            float dst = Vector3.Distance(uIButtons[i].transform.position, mousePos);
            if(dst< closestDst)
            {
                hoveredButton = uIButtons[i];
                closestDst = dst;
            }
        }
    }
}
