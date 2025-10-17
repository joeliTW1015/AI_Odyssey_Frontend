using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class LevelManager_03 : LevelManagerBase
{
    [Header("Backend Settings")]
    [SerializeField] string scoreBoardURL;

    [Header("UI References")]
    [SerializeField] GameObject qAndAPanel;
    [SerializeField] GameObject ragPanel;
    [SerializeField] GameObject chatBotPanel;


    [Header("Dialogue")]
    [SerializeField] DialogueSequence initialDialogue;
    [SerializeField] DialogueSequence endingDialogue;

    public static LevelManager_03 Instance;

    float playTime = 0f;

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
        playTime = 0f;
    }

    void Update()
    {
        playTime += Time.deltaTime;
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
            Debug.Log("Level 03 is done");
            StartCoroutine(SendScoreToBackend((int)playTime));
        }
    }

    IEnumerator SendScoreToBackend(int score)
    {
        Debug.Log("Sending score to backend: " + score);
        LoadingHandler.Instance.ShowLoadingScreen("正在上傳分數...");
        string url = scoreBoardURL + PlayerPrefs.GetString("username") + "/score";
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        string jsonBody = JsonConvert.SerializeObject(new { score = score, level = 3 });
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("key")); //使用存儲的token才能發送分數
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error sending score: " + request.error);
            yield break;
        }
        else
        {
            Debug.Log("Score successfully sent to backend.");
        }
        LoadingHandler.Instance.HideLoadingScreen();
        LoadingHandler.Instance.ChangeScene("LevelMenu");
    }
}
