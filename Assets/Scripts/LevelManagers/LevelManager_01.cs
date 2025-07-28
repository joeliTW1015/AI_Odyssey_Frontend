using UnityEngine;

public class LevelManager_01 : MonoBehaviour
{
    public static LevelManager_01 Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }
}
