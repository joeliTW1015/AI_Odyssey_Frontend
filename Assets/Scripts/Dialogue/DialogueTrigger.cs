using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] bool retriggerable = true; // Allows the dialogue to be triggered multiple times
    [SerializeField] float clickRadius = 300f; // Radius within which the player can trigger dialogue
    public DialogueSequence dialogueSequence;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] GameObject nameTextObject;
    bool playerInRange;
    Vector2 touchPosition;

    void Start()
    {
        if (transform.parent != null)
        {
            touchPosition = transform.parent.position;
        }
        else
        {
            touchPosition = transform.position;
        }
        
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }
        playerInRange = false;
        nameTextObject.SetActive(true);
    }

    // Method to trigger the dialogue sequence
    public void TriggerDialogue()
    {
        if (dialogueSequence != null && dialogueSequence.lines.Count > 0)
        {
            // Logic to start the dialogue using the dialogueSequence
            Debug.Log("Trigger dialogue");
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
            nameTextObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            exclamationMark.SetActive(false);
            nameTextObject.SetActive(true);
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
                    if (touch.phase == TouchPhase.Began && Vector2.Distance(touch.position, Camera.main.WorldToScreenPoint(touchPosition)) < clickRadius)
                    {
                        playerInRange = false; // Reset playerInRange to prevent multiple triggers
                        exclamationMark.SetActive(false);
                        TriggerDialogue();
                        break;
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Input.mousePosition;
                if (Vector2.Distance(mousePos, Camera.main.WorldToScreenPoint(touchPosition)) < clickRadius)
                {
                    playerInRange = false;
                    exclamationMark.SetActive(false);
                    TriggerDialogue();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                playerInRange = false;
                exclamationMark.SetActive(false);
                TriggerDialogue();
            }
        }
    }
}

    