using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform attackPoint;
    public LayerMask playerLayers;
    public GameObject coins;

    public float lookRadius;
    public float wanderRadius;

    public float attackRange = 1f;
    public int attackDamage = 20;
    
    public int maxHealth = 100;
    private int currentHealth;
    
    private Animator animator;
    private Rigidbody rb;
    
    private Vector3 wanderWaypoint;
 
    private float wanderSpeed = 1.25f;
    private float runSpeed = 2.25f;
    
    private float attackRate = 3f;
    private float nextAttack;
    private float waitTime = 3f;
    private float wanderTimer;

    void Start()
    {
         agent = GetComponent<NavMeshAgent>();
         animator = GetComponentInChildren<Animator>();
         rb = GetComponent<Rigidbody>();
         currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        // Distance to the target
        float distance = Vector3.Distance(player.position, transform.position);
        
        // If not inside the lookRadius
        if (distance >= lookRadius)
        {
            if (Time.time > wanderTimer) 
            {
                Wander();
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                wanderTimer = Time.time + waitTime;
            }
        }
        
        if (distance < lookRadius)
        {
            FaceTarget();
            // If within attacking distance
            if (distance < agent.stoppingDistance)
            {
                // FaceTarget(); 
                Idle();
                if (Time.time > nextAttack)
                {
                    nextAttack = Time.time + attackRate;
                    StartCoroutine(Slash());
                }
            }
            else
            {
                // Move towards the target
                agent.SetDestination(player.position);
                // FaceTarget(); 
                Run();
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) 
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
     
        return navHit.position;
    }
    
    private void Idle()
    {
        animator.SetFloat("Speed", 0f, 0.2f, Time.deltaTime);
    }

    private void Wander()
    {
        agent.speed = wanderSpeed;
        animator.SetFloat("Speed", 0.5f, 0.2f, Time.deltaTime);
    }
    
    private void Run()
    {
        agent.speed = runSpeed;
        animator.SetFloat("Speed", 1f, 0.2f, Time.deltaTime);
    }

    private IEnumerator Slash()
    {
        agent.isStopped = true;
        animator.SetTrigger("Swing");
        yield return new WaitForSeconds(0.85f);
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);

        // yield return new WaitForSeconds(0.5f);
        foreach (Collider player in hitPlayers)
        {
            player.GetComponent<PlayerCombat>().TakePlayerDamage(attackDamage);
            Debug.Log("Player hit!");
        }

        yield return new WaitForSeconds(1.15f);
        agent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // Rotate to face the target
    void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Play hurt animation
        animator.SetTrigger("Hurt");
        
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        
        animator.SetBool("IsDead", true);

        rb.constraints = RigidbodyConstraints.FreezeAll;
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
        Instantiate(coins, rb.position, Quaternion.identity);
    }
}
