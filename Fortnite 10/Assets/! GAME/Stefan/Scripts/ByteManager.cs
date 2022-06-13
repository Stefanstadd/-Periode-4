using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ByteManager : MonoBehaviour
{
    static List<CollectedBytes> collectedBytes = new List<CollectedBytes>();
    public static int bytes;
    int byteDisplay;
    public float byteSmoothSpeed;
    float byteVel;
    public Transform target;

    public float speedMultiplier = 1;
    public float treshold, mergeTreshold;

    public TextMeshProUGUI addedBytes,currency;
    public float addedByteLifeTime,addedByteWaitTime;
    float addedBytecurrentLifeTime;
    int addedByteTotal;
    bool canEditAddedBytes;
    bool addedBytesActive;
    Vector3 scaleVelocity;
    Vector3 addedByteTargetPos;
    Vector3 addedByteBasePos;
    Vector3 posVelocity;
    public float addedByteScale;
    public float addedByteSmoothTime,addedBytePosSmoothTime;

    private void Start()
    {
        addedByteBasePos = addedBytes.transform.parent.position;
        addedByteTargetPos = addedByteBasePos;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateCollectedBytes();
        UpdateAddedBytes();

        byteDisplay = Mathf.RoundToInt(Mathf.SmoothDamp(byteDisplay,bytes,ref byteVel, byteSmoothSpeed));
        currency.text = byteDisplay.ToString();

        canEditAddedBytes = true;
    }

    void UpdateCollectedBytes()
    {
        for (int i = 0; i < collectedBytes.Count; i++)
        {
            CollectedBytes @byte = collectedBytes[i];
            Transform target = @byte._byte.transform;
            @byte._byte.Disable();
            @byte._byte.transform.parent.position = Vector3.MoveTowards(@byte._byte.transform.parent.position, this.target.position, @byte.speed);
            @byte.AddToSpeed(speedMultiplier * Time.deltaTime);

            //Set byte size

            float dst = Vector3.Distance(this.target.position, @byte._byte.transform.position);

            if (dst > @byte.maxDst) @byte.maxDst = dst;

            float scaleNumber = Mathf.InverseLerp(0, @byte.maxDst, dst);
            Vector3 scale = new Vector3(scaleNumber,scaleNumber,scaleNumber);

            @byte._byte.transform.localScale = scale;

            if (Vector3.Distance(this.target.position, @byte._byte.transform.parent.position) < treshold)
            {
                print("1");
                OnByteReachedPosition(@byte);
            }
        }
    }

    void OnByteReachedPosition(CollectedBytes _byte)
    {
        addedByteTotal += _byte.amount;

        if (canEditAddedBytes)
        {
            print("2");
            if (!addedBytesActive)
            {
                print("3");
                ActivateAddedBytes();
            }
            else
                addedBytes.transform.localScale = new Vector3(9.367661f, 9.367661f, 9.367661f) * 1.3f;

            addedBytecurrentLifeTime = addedByteLifeTime;

            collectedBytes.Remove(_byte);
            Destroy(_byte._byte.transform.root.gameObject);
        }
    }

    void UpdateAddedBytes()
    {
        Vector3 targetScale = new Vector3(addedByteScale, addedByteScale, addedByteScale);

        addedBytes.transform.localScale = Vector3.SmoothDamp(addedBytes.transform.localScale, targetScale, ref scaleVelocity, addedByteSmoothTime);
        addedBytes.transform.parent.position = Vector3.SmoothDamp(addedBytes.transform.parent.position, addedByteTargetPos, ref posVelocity, addedBytePosSmoothTime);

        string text = "";

        if (addedByteTotal > 0)
            text = "+ " + addedByteTotal.ToString();
        addedBytes.text = text;
        if(addedBytesActive)
            addedBytecurrentLifeTime -= Time.deltaTime;

        if(addedBytecurrentLifeTime <= 0 && addedBytesActive)
        {
            print("4");
            MergeWithCurrency();
        }

        if (Vector3.Distance(addedBytes.transform.parent.position, currency.transform.position) < mergeTreshold)
        {
            print("5");
            ResetAddedBytes();
        }

    }

    void MergeWithCurrency()
    {
        addedByteTargetPos = currency.transform.position;
        addedByteScale = 0;
        canEditAddedBytes = false;
    }

    async void ResetAddedBytes()
    {
        bytes += addedByteTotal;
        addedByteTotal = 0;
        addedBytesActive = false;
        addedByteTargetPos = addedByteBasePos;
        await System.Threading.Tasks.Task.Delay(Mathf.RoundToInt(addedByteWaitTime * 1000));

        canEditAddedBytes = true;
    }

    void ActivateAddedBytes()
    {
        addedBytes.gameObject.SetActive(true);
        addedBytes.transform.localScale = Vector3.zero;
        addedBytesActive = true;
        addedBytecurrentLifeTime = addedByteLifeTime;

        addedByteScale = 9.367661f;
    }

    public static void OnCollectBytes(Byte _byte)
    {
        collectedBytes.Add(new CollectedBytes(_byte));
    }

    class CollectedBytes
    {
        public Byte _byte;
        public float speed;
        public int amount;

        public Vector3 spawnPos;
        public float maxDst = float.MinValue;
        public CollectedBytes(Byte _byte)
        {
            this._byte = _byte;
            speed = 0;
            amount = _byte.amount;
            spawnPos = _byte.transform.position;
        }
        public void AddToSpeed(float speed)
        {
            this.speed += speed;
        }
    }
}
