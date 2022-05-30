using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemies")]

[System.Serializable]
public class EnemyData : ScriptableObject
{
    public GameObject prefab;
    public string enemyName;
    public float maxHP;
    public float resistance;
    public float detectRadius, attackRadius;

    public float movementSpeed;
    public float rotationSpeed;

    public Vector2 dataDrops;
}
