using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Byte : MonoBehaviour
{
    public int amount;
    public float mergeDst;
    public float pickupDst;

    PlayerInventory playerInventory;
    bool pickedUp;
    void Start()
    {
        playerInventory = PlayerInventory.playerInventory;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pickedUp) return;
        CheckPickup();
    }

    void CheckPickup()
    {
        float dst = Vector3.Distance(transform.position,PlayerInventory.playerInventory.transform.position);
        if(dst < pickupDst)
        {
            pickedUp = true;
            OnPickup();
        }
    }

    void OnPickup()
    {
        ByteManager.OnCollectBytes(this);
        print("Opgepakt");
    }

    public void Disable()
    {
        Rigidbody rb = transform.parent.GetComponent<Rigidbody>();
        if(rb != null)
            Destroy(rb);
        Collider col = transform.parent.GetComponent<Collider>();
        if (col != null)
            Destroy(col);
    }

    void CheckMerge()
    {
        var col = Physics.OverlapSphere(transform.position,mergeDst);
        for (int i = 0; i < col.Length; i++)
        {
            Byte b = col[i].GetComponent<Byte>();
            if(b != null && b != this)
            {
                b.amount += amount;
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
