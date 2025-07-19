using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float damage = 10f;
    public float health = 50f;

    private float lastAttackTime;
    public float attackCooldown = 2f;

    public Animator animator;
    private Rigidbody[] rb;

    public AudioSource bulletHitSound;
    public AudioSource deathSound;
    public AudioSource chaseSound;
    public AudioSource attackSound;

    private bool isChasingSoundPlayed = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponentsInChildren<Rigidbody>();

        // Make sure each AudioSource is properly assigned in the Inspector
        if (bulletHitSound == null || deathSound == null || chaseSound == null || attackSound == null)
        {
            Debug.LogWarning("One or more AudioSources not assigned!");
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= detectionRange || health<50)
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    void Idle()
    {
        agent.isStopped = true;
        animator.SetBool("isIdle", true);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttack", false);

        isChasingSoundPlayed = false;
    }

    void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        animator.SetBool("isChasing", true);
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", false);

        if (!chaseSound.isPlaying && !isChasingSoundPlayed)
        {
            chaseSound.Play();
            isChasingSoundPlayed = true;
        }
    }

    void Attack()
    {
        agent.isStopped = true;
        animator.SetBool("isIdle", false);
        animator.SetBool("isChasing", false);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetBool("isAttack",true); // use trigger instead of bool
            if (attackSound != null)
                attackSound.Play();

            Debug.Log("Zombie attacks player!");
            lastAttackTime = Time.time;

            // Uncomment when Player has a health system
            //Health playerHealth = player.GetComponent<Health>();
            //if (playerHealth != null)
            //{
            //    playerHealth.TakeDamage(damage);
            //}
        }
    }

    public void TakeDamage(float amount)
    {
        if (bulletHitSound != null)
            bulletHitSound.Play();

        health -= amount;
        Debug.Log("Zombie took damage! Remaining health: " + health);
    }

    void Die()
    {
        Debug.Log("Zombie died!");

        foreach (Rigidbody r in rb)
        {
            r.isKinematic = false; // Enable ragdoll
        }

        if (deathSound != null)
            deathSound.Play();

        animator.enabled = false;
        agent.enabled = false;
        this.enabled = false;

        Destroy(gameObject, 5f);
    }
}
