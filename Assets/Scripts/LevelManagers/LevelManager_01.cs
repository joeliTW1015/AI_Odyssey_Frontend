using UnityEngine;
using UnityEngine.UI;

public class LevelManager_01 : LevelManagerBase
{
    public static LevelManager_01 Instance;
    [Header("Interactive Interface")]
    [SerializeField] InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Image outcomeImage;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    public override void ActivateInteractiveInterface()
    {
        base.ActivateInteractiveInterface();

    }
}
