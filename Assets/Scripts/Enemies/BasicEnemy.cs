using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : EnemyBase
{
    [Header("Basic Enemy Settings")]
    [SerializeField] private float attackRange = 2f; // Range within which the enemy can attack
    [SerializeField] Animator animator;
    protected Transform playerTransform; 
    protected PlayerTakeDamage playerTakeDamage;
    float attackCooldownTimer = 0f; // Timer to manage attack cooldown
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        // Additional initialization for BasicEnemy if needed
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerTakeDamage = playerTransform.GetComponent<PlayerTakeDamage>();
        if (playerTransform == null)
        {
            Debug.LogError("Player transform not found in BasicEnemy.");
        }
    }
    protected override void GetTargetPosition()
    {
        // Logic to determine the target position for the enemy
        // For example, it could be the player's position or a random point in the game world
        targetPosition = playerTransform.position;
    }

    public override void TakeDamage(float damage, Vector2 damageSourcePosition)
    {
        base.TakeDamage(damage, damageSourcePosition); // Call the base class method to handle common behavior
        if (health <= 0)
        {
           animator.SetTrigger("Die"); // Trigger death animation
        }
    }

    protected override void Die()
    {
        // Custom logic for when the BasicEnemy dies
        Debug.Log("BasicEnemy has died.");
        base.Die();
    }

    protected override void Update()
    {
        base.Update(); // Call the base class Update method to handle common behavior
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange && health > 0)
        {
            if (attackCooldownTimer <= 0f)
            {
                animator.SetTrigger("Attack");
                attackCooldownTimer = attackCooldown; // Reset the cooldown timer
                Attact(); // Call the attack method
            }
            attackCooldownTimer -= Time.deltaTime; // Decrease the cooldown timer
        }
        else
        {
            attackCooldownTimer -= Time.deltaTime; // Decrease the cooldown timer
        }
    }

    private void Attact()
    {
        canMove = false;
        if (playerTakeDamage != null)
        {
            // Apply damage to the player
            playerTakeDamage.TakeDamage(attackDamage, transform.position);
            Debug.Log($"BasicEnemy attacked the player for {attackDamage} damage.");
        }
        else
        {
            Debug.LogError("PlayerTakeDamage component not found on the player.");
        }
        canMove = true;
    }
}

