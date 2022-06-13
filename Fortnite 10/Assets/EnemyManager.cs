using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject bytes;
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            print("z");
            for (int i = 0; i < 10; i++)
            {
                GameObject enemy = Instantiate(this.enemy, PlayerMovement.player.transform.position + new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * 10, Quaternion.identity);
                enemy.GetComponent<BaseEnemy>().bytes = bytes;

            }
        }
    }
}
