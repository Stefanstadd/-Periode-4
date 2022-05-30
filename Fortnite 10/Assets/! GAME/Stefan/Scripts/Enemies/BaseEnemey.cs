using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyState
{
    Wandering,
    Fleeing,
    Attacking,
    Threatening
}
public class BaseEnemey : MonoBehaviour
{
    [Header("Settings")]
    public EnemyState state;
    public EnemyData data;
    public float currentHp;

    [Header("Other")]
    public Color threadColor,attackColor;
    public GameObject spawnedEnemy;

    Transform target;
    NavMeshAgent agent;

    bool canSeePlayer;
    public float wanderTimer;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = PlayerMovement.player.transform;

        InitializeEnemy(data);
        InvokeRepeating("UpdateEnemy", 0, 0.1f);
    }
    void InitializeEnemy(EnemyData data)
    {
        spawnedEnemy = Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
        agent.speed = data.movementSpeed;
        agent.angularSpeed = data.rotationSpeed;
        agent.stoppingDistance = data.attackRadius;
        currentHp = data.maxHP;
    }

    void UpdateEnemy()
    {
        SetEnemyState();
        if (canSeePlayer)
        {
            if (state == EnemyState.Threatening)
            {
                SetTarget(target.position);
            }
            if(state == EnemyState.Attacking)
            {
                //do stuff for attacking
            }
        }
        else
        {
            if (state == EnemyState.Wandering)
            {
                if (wanderTimer <= 0) 
                {
                    wanderTimer = data.WanderTimer;
                    SetTarget(Random.insideUnitSphere * 30);
                }
                else
                {
                    wanderTimer -= Time.deltaTime * 10;
                }

            }
        }
    }


    void CheckPlayerVisible()
    {
        bool newValue = CanSeePlayer();
        if(canSeePlayer && newValue == false)
        {
            OnLostPlayer();
        }
        else if(canSeePlayer == false && newValue)
        {
            OnSeePlayer();
        }
        canSeePlayer = newValue;
    }

    void SetEnemyState()
    {
        CheckPlayerVisible();
        float dst = Vector3.Distance(transform.position, target.position);
        if (dst < data.detectRadius && canSeePlayer)
        {
            state = EnemyState.Threatening;
            if(dst < data.attackRadius)
            {
                SetTarget(target.position);
                state = EnemyState.Attacking;
            }
        }
        else
        {
            state = EnemyState.Wandering;
        }
    }

    public bool CanSeePlayer()
    {
        RaycastHit hit;

        Quaternion previousRot = transform.rotation;

        transform.LookAt(target.position);

        Debug.DrawRay(transform.position, transform.forward * 100,Color.red,0.1f);

        if (Physics.Raycast(transform.position,transform.forward,out hit, data.detectRadius))
        {
            transform.rotation = previousRot;
            if (hit.transform.root.CompareTag("Player")) return true;
        }
        transform.rotation = previousRot;
        return false;
    }

    void OnSeePlayer()
    {

    }

    void OnLostPlayer()
    {

    }


    public void SetTarget(Vector3 pos)
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(pos);
        }
    }

    private void EnableAgent(bool value)
    {
        if (!value) agent.SetDestination(transform.position);
        agent.isStopped = !value;
    }
    public void TakeDamage(float dmg)
    {
        currentHp -= dmg * data.resistance;

        if(currentHp <= 0)
        {
            Die();
        }
        SetTarget(target.position);
    }
    void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = threadColor;
        Gizmos.DrawWireSphere(transform.position,data.detectRadius);

        Gizmos.color = attackColor;
        Gizmos.DrawWireSphere(transform.position,data.attackRadius);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;
        }
    }
}
