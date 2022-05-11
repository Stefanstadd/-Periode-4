using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextManager : MonoBehaviour
{
    public GameObject damageText;
    public Color defaultColor, criticalColor;
    public Vector3 defaultSize, criticalSize;
    public float defaultLifeTime, criticalLifeTime,yStartPos;
    
    public void SetDamageText(Vector3 pos,float damage, bool critical)
    {
        pos += Vector3.up * yStartPos;
        var obj = Instantiate(damageText, pos, Quaternion.identity);

        var text = obj.GetComponent<TextMeshPro>();
        text.faceColor = critical ? criticalColor : defaultColor;
        text.transform.localScale = Vector3.zero;
        text.text = damage.ToString();
        obj.GetComponent<DamageText>().Initialize(critical? criticalLifeTime : defaultLifeTime,PlayerMovement.player.cam, 1f, critical ? criticalSize : defaultSize);

        //Invoke(Destroy(text.gameObject), critical ? criticalLifeTime : defaultLifeTime);
    }
}
