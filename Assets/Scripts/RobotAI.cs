using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour
{
    [Header("Riferimenti")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform antenna; // L'antenna da proteggere
    public Transform player;
    
    [Header("Combattimento")]
    public float attackRange = 2f;
    public float detectionRange = 15f;
    public float attackCooldown = 2f;
    public int maxHealth = 100;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 30f;
    
    [Header("Pattugliamento")]
    public float patrolRadius = 10f;
    public float waitTimeAtPoint = 3f;
    
    private int currentHealth;
    private float lastAttackTime;
    private Vector3 patrolPoint;
    private float waitTimer;
    private bool isWaiting;
    private bool antennaDestroyed = false;
    
    private enum State { Patrol, Chase, Attack, Idle, Disabled }
    private State currentState;

    void Start()
    {
        currentHealth = maxHealth;
        currentState = State.Patrol;
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        SetNewPatrolPoint();
    }

    void Update()
    {
        // Controlla se l'antenna è stata distrutta
        if (antenna == null && !antennaDestroyed)
        {
            OnAntennaDestroyed();
            return;
        }

        if (antennaDestroyed || currentState == State.Disabled)
        {
            animator.SetBool("Idle", true);
            return;
        }

        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        // State Machine
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                
                // Rileva il giocatore
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = State.Chase;
                    isWaiting = false;
                }
                break;

            case State.Chase:
                Chase();
                
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attack;
                }
                else if (distanceToPlayer > detectionRange * 1.5f)
                {
                    currentState = State.Patrol;
                    SetNewPatrolPoint();
                }
                break;

            case State.Attack:
                Attack();
                
                if (distanceToPlayer > attackRange)
                {
                    currentState = State.Chase;
                }
                break;
        }

        UpdateAnimations();
    }

    void Patrol()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNewPatrolPoint();
            }
            return;
        }

        agent.SetDestination(patrolPoint);

        if (Vector3.Distance(transform.position, patrolPoint) <= agent.stoppingDistance + 0.5f)
        {
            isWaiting = true;
            waitTimer = waitTimeAtPoint;
        }
    }

    void Chase()
    {
        if (player == null) return;
        
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (player == null) return;

        agent.SetDestination(transform.position); // Ferma il movimento
        
        // Ruota verso il giocatore
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Spara
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Shoot();
        }
    }

    void Shoot()
    {
        animator.SetTrigger("Shoot");

        if (bulletPrefab != null && firePoint != null && player != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * bulletSpeed;
            }
            
            Destroy(bullet, 5f);
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += antenna != null ? antenna.position : transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
        }
    }

    void UpdateAnimations()
    {
        float speed = agent.velocity.magnitude;
        
        animator.SetFloat("Speed", speed);
        animator.SetBool("Idle", speed < 0.1f && currentState != State.Attack);
        animator.SetBool("Walk", speed > 0.1f && speed < agent.speed * 0.6f);
        animator.SetBool("Run", speed >= agent.speed * 0.6f);
    }

    public void TakeDamage(int damage)
    {
        if (antennaDestroyed) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
        currentState = State.Disabled;
        
        if (agent != null)
            agent.enabled = false;
        
        Destroy(gameObject, 3f); // Distruggi dopo l'animazione
    }

    void OnAntennaDestroyed()
    {
        antennaDestroyed = true;
        currentState = State.Disabled;
        
        // Ferma il robot
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
        
        // Animazione di disattivazione
        animator.SetTrigger("Die");
        
        Debug.Log("Robot disattivato: antenna distrutta!");
    }

    // Chiamata dall'animazione di morte
    public void OnDeathAnimationComplete()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizza il raggio di rilevamento
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Visualizza il raggio di attacco
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Visualizza il raggio di pattugliamento
        Gizmos.color = Color.blue;
        Vector3 center = antenna != null ? antenna.position : transform.position;
        Gizmos.DrawWireSphere(center, patrolRadius);
    }
}