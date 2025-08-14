using UnityEngine;

public class DoorToNextLevel : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            // Load the next level or scene
            Debug.Log("Player has entered the door to the next level.");
            LoadingHandler.Instance.ChangeScene(nextLevelName);
        }
    }
}
