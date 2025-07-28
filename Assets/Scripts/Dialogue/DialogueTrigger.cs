using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    bool retriggerable = true; // Allows the dialogue to be triggered multiple times
    [SerializeField] float clickRadius = 100f; // Radius within which the player can trigger dialogue
    [SerializeField] DialogueSequence dialogueSequence;
    [SerializeField] GameObject exclamationMark;
    bool playerInRange;

    void Start()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }
        playerInRange = false;
    }

    // Method to trigger the dialogue sequence
    public void TriggerDialogue()
    {
        if (dialogueSequence != null && dialogueSequence.lines.Count > 0)
        {
            // Logic to start the dialogue using the dialogueSequence
            Debug.Log("Starting dialogue");
            // Additional code to handle the dialogue display can be added here
            DialogueManager.Instance.StartDialogue(dialogueSequence);
            if (!retriggerable)
            {
                Destroy(gameObject); // Destroy the trigger if not retriggerable
            }
        }
        else
        {
            Debug.LogWarning("No dialogue sequence assigned or empty.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            exclamationMark.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            exclamationMark.SetActive(false);
        }
    }

    void Update()
    {
        //mobile input handling
        if (playerInRange)
        {
            
            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                foreach (Touch touch in touches)
                {
                    if (touch.phase == TouchPhase.Began && Vector2.Distance(touch.position, Camera.main.WorldToScreenPoint(exclamationMark.transform.position)) < clickRadius)
                    {
                        playerInRange = false; // Reset playerInRange to prevent multiple triggers
                        exclamationMark.SetActive(false);
                        TriggerDialogue();
                        break;
                    }
                }
            }
        }
    }
}

    