using UnityEngine;

public class BasicEnemy : EnemyBase
{
    protected Transform playerTransform; // Reference to the player's transform, if needed
    protected override void Awake()
    {
        base.Awake();
        // Additional initialization for BasicEnemy if needed
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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

    protected override void Die()
    {
        // Custom logic for when the BasicEnemy dies
        Debug.Log("BasicEnemy has died.");
        base.Die();
    }
}

