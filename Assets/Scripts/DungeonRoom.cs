using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    [Header("Traps Animators")]
    [SerializeField] Animator entryTrapAnimator;
    [SerializeField] Animator exitTrapAnimator;
    [Header("Enemy Waves")]
    [SerializeField] GameObject[] enemyWaves;

    bool roomActivated = false;
    bool roomCompleted = false;

    public int remainingEnemies;
    int currentWaveIndex = 0;
    private void Awake()
    {
        roomActivated = false;
        roomCompleted = false;
        foreach (GameObject wave in enemyWaves)
        {
            wave.SetActive(false); // Ensure all enemy waves are inactive at the start
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !roomActivated && !roomCompleted)
        {
            ActivateRoom();
        }
    }

    private void ActivateRoom()
    {
        roomActivated = true;
        Debug.Log("Room activated. Starting traps and spawning enemies.");
        // Start traps
        if (entryTrapAnimator != null)
        {
            entryTrapAnimator.SetTrigger("Activate");
        }
        if (exitTrapAnimator != null)
        {
            exitTrapAnimator.SetTrigger("Activate");
        }
        if (enemyWaves.Length > 0)
        {
            remainingEnemies = enemyWaves[0].transform.childCount; // Assuming each wave has child enemies
            enemyWaves[0].SetActive(true); // Activate the first enemy wave
            Debug.Log($"Wave {currentWaveIndex + 1} activated with {remainingEnemies} enemies.");
        }
        else
        {
            Debug.LogWarning("No enemy waves assigned to DungeonRoom.");
        }

    }
    
    private void Update()
    {
        if (roomActivated && !roomCompleted && remainingEnemies <= 0)
        {
            currentWaveIndex++;
            if (currentWaveIndex < enemyWaves.Length)
            {
                // Activate the next wave of enemies
                remainingEnemies = enemyWaves[currentWaveIndex].transform.childCount;
                enemyWaves[currentWaveIndex].SetActive(true);
                Debug.Log($"Wave {currentWaveIndex + 1} activated with {remainingEnemies} enemies.");
            }
            else
            {
                // All waves completed
                roomCompleted = true;
                Debug.Log("All enemy waves completed. Room is now clear.");
                if (exitTrapAnimator != null)
                {
                    exitTrapAnimator.SetTrigger("Deactivate");
                }
                if (entryTrapAnimator != null)
                {
                    entryTrapAnimator.SetTrigger("Deactivate");
                }
            }
        }
    }
}
