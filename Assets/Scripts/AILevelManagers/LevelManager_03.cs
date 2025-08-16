using UnityEngine;
using UnityEngine.UI;

public class LevelManager_03 : LevelManagerBase
{

    [Header("UI References")]
    [SerializeField] GameObject qAndAPanel;
    [SerializeField] GameObject ragPanel;
    [SerializeField] GameObject chatBotPanel;
    

    [Header("Dialogue")]
    [SerializeField] DialogueSequence initialDialogue;
    [SerializeField] DialogueSequence endingDialogue;

    public static LevelManager_03 Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    void Start()
    {
        DialogueManager.Instance.StartDialogue(initialDialogue);
    }

    public override void ActivateEvent(int EventIndex)
    {
        if (EventIndex == 1) //檢索(rag)機器人
        {
            ragPanel.SetActive(true);
        }
        else if (EventIndex == 2) //ChatBot知識機器人
        {
            chatBotPanel.SetActive(true);
        }
        else if (EventIndex == 3) //問答面板
        {
            qAndAPanel.SetActive(true);
        }
        else if (EventIndex == 4) //全部答對
        {
            DialogueManager.Instance.StartDialogue(endingDialogue);
        }
        else if (EventIndex == 5) //回到關卡選擇
        {
            LoadingHandler.Instance.ChangeScene("LevelMenu");
        }
    }
}
