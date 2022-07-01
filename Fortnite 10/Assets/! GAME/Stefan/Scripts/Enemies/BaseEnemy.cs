using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyState
{
    Wandering,
    Fleeing,
    Attacking,
    Threatening
}
public class BaseEnemy : MonoBehaviour
{
    [Header("Settings")]
    public EnemyState state;
    public EnemyData data;
    public float currentHp;

    [Header("Other")]
    public Color threadColor,attackColor;
    public GameObject spawnedEnemy;
    public GameObject bytes;
    public bool hit;

    Transform target;
    NavMeshAgent agent;
    public Wave wave;

    Animator animator;

    bool canSeePlayer;
    public float wanderTimer;
    bool attacking;

    float dataMultiplier = 1;
    protected virtual void Start()
    {
        target = PlayerMovement.player.transform;
        InvokeRepeating("UpdateEnemy", 0, 0.1f);
        
        agent = GetComponent<NavMeshAgent>();
        
        if(spawnedEnemy)
        animator = spawnedEnemy.GetComponent<Animator>();   
    }
    public void InitializeEnemy(EnemyData data,Wave wave)
    {
        this.data = data;   
        WaveMultiplier multiplier = WaveMultiplier.Standard;
        if(wave != null)
            multiplier = wave.multiplier;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        spawnedEnemy = Instantiate(data.prefab, transform.position, Quaternion.identity, transform);

        animator = spawnedEnemy.GetComponent<Animator>();

        if(animator)
            animator.SetFloat("Speed", 1 * multiplier.movementSpeed);

        agent.speed = data.movementSpeed * multiplier.movementSpeed;
        agent.angularSpeed = data.rotationSpeed * multiplier.rotationSpeed;
        agent.stoppingDistance = data.attackRadius;
        currentHp = data.maxHP * multiplier.hp;
        dataMultiplier = multiplier.dataDrops;

        this.wave = wave;
    }

    void UpdateEnemy()
    {
        print(state);
        if (spawnedEnemy == null) return;

        SetEnemyState();
        print(canSeePlayer);
        if (canSeePlayer)
        {
            if (state == EnemyState.Threatening)
            {
                SetTarget(target.position);
            }
            if(state == EnemyState.Attacking && attacking == false)
            {
                print("Attack");
                Attack();
            }
        }
        else
        {
            if (state == EnemyState.Wandering)
            {
                if (wanderTimer <= 0) 
                {
                    wanderTimer = data.WanderTimer;
                    SetTarget(transform.position + Random.insideUnitSphere * 30);
                }
                else
                {
                    wanderTimer -= 0.1f;//omdat het om de 0.1 seconde gebeurt
                }

            }
        }
    }

    async void Attack()
    {
        print("Attack");
        attacking = true;
        SetTarget(transform.position);
        if (animator)
        {
            animator.SetTrigger("Attacking");
        }

        PlayerMovement.player.TakeDamage(data.damage * wave.multiplier.damage);

        await Task.Delay((int)(data.attackSpeed * 1000));
        attacking = false;
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
        if (attacking) return;

        CheckPlayerVisible();
        float dst = Vector3.Distance(transform.position, target.position);
        if (wave != null || (dst < data.detectRadius && canSeePlayer))
        {
            state = EnemyState.Threatening;
            if(wave != null || dst < data.attackRadius)
            {
                SetTarget(target.position);

                if(dst<= data.attackRadius)
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
        RageManager.AddRage();
        if (wave != null) wave.killed++;
        var bytes = Instantiate(this.bytes, transform.position, Quaternion.identity);
        bytes.GetComponentInChildren<Byte>().amount = Mathf.RoundToInt(Random.Range(data.byteDrops.x,data.byteDrops.y) * dataMultiplier);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
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

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
}
