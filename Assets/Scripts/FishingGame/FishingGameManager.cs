using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

[System.Serializable]
public class FishInfo
{
    public int fishType; // 0: silverfish, 1: normalfish
    public Sprite fishSprite;

    public FishInfo(int type, Sprite sprite)
    {
        fishType = type;
        fishSprite = sprite;
    }
}

public class FishingGameManager : LevelManagerBase
{
    [Header("URL")]
    [SerializeField] string fishRecognitionURL; // 魚類識別的URL
    [SerializeField] string scoreBoardURL; // 分數板的URL
    [Header("Dialogues")]
    [SerializeField] DialogueSequence startDialogue; // 開始釣魚的對話
    [SerializeField] DialogueSequence firstTimeEndDialogue; // 第一次結束釣魚的對話
    [SerializeField] DialogueSequence SecondTimeEndDialogue; // 結束釣魚的對話
    [Header("Fish List")]
    [SerializeField] List<FishInfo> fishList; // List of fish types and their sprites

    [Header("Fishing Settings")]
    bool startFishing = false; // 是否開始釣魚
    [SerializeField] int maxCatch = 10; // Maximum number of fish to catch
    [SerializeField] float hookSwingSpeed = 2f; // Speed of the hook swinging left and right
    [SerializeField] float hookSwingRange = 0.5f; // Range of the hook swinging
    [SerializeField] float hookCatchSpeed = 5f; // Speed of the hook when catching fish
    [SerializeField] float hookSwingAngle = 30f; // Angle of the hook swinging

    [Header("Reference")]
    [SerializeField] GameObject fishPrefab; // Prefab for the fish
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Hook hook;
    [SerializeField] Transform fixedpoint;//魚竿固定線的點
    [SerializeField] Button CastButton; // 按鈕用於釣魚
    [SerializeField] GameObject fishCaughtPanel; // 面板顯示預測結果
    [SerializeField] Text reconnitionOutcomeText; // 捕到魚的識別結果文本
    [SerializeField] Text SilverFishCountText;
    [SerializeField] GameObject resultPanel;
    [SerializeField] Image resultImage;
    [SerializeField] Image realFishImage;
    [Header("QTE Settings")]
    [SerializeField] GameObject qtePanel; // QTE面板
    [SerializeField] Button qteButton; // QTE按鈕
    [SerializeField] Slider qteProgressSlider; // QTE進度條
    [SerializeField] float qteProgressDropSpeed = 2.5f; // QTE進度條下降速度
    [SerializeField] float qteProgressIncreaseSpeed = 1f; // QTE進度條增加速度(按下按鈕時)
    [SerializeField] float qteHookShakeAmplitude = 0.5f; // QTE時魚鉤的抖動幅度
    float qteProgress = 0f; // QTE進度條的當前值

    [Header("Result Sprites")]
    [SerializeField] Sprite silverFishResultSprite;
    [SerializeField] Sprite normalFishResultSprite;

    public static FishingGameManager Instance;

    int state; // 0: 魚線未下放, 1: 魚線下放, 2: 捕捉到魚(檢查中), 3:捕捉到魚(拉鋸中), 4: 魚線上收
               //魚線未拉出時，魚鉤在上方左右擺動，直到釣魚按鈕被按下
    int currentCatchCount = 0; // 當前捕捉到的魚數量
    int silverfishCount = 0; // 捕到的銀龍魚數量
    int currentFishIndex = 0; // 當前釣到的魚的索引
    int confidence = 0; // 捕到魚的識別結果置信度
    int predicTypeResult = 0; // 捕到魚的識別結果，0: 銀魚, 1: 普通魚

    float currentHookSwingAngle = 0f; // 魚鉤擺動的角度

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        startFishing = false; // 初始狀態為未開始釣魚
        state = 0; // 初始狀態為魚線未拉出
        currentCatchCount = 0;
        currentFishIndex = -1; // 初始沒有釣到魚
        silverfishCount = 0;
        predicTypeResult = -1; // 初始沒有識別結果
        confidence = 0; // 初始置信度為0
        CastButton.onClick.AddListener(CastTheHook); // 設置釣魚按鈕的點擊事件
        qteButton.onClick.AddListener(IncreaseQTEProgress); // 設置QTE按鈕的點擊事件
        fishCaughtPanel.SetActive(false); // 初始隱藏捕到魚的面板
        qtePanel.SetActive(false); // 初始隱藏QTE面板
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // 設置魚線的點數
            lineRenderer.SetPosition(0, fixedpoint.position); // 設置魚線的起點
            lineRenderer.SetPosition(1, fixedpoint.position); // 設置魚線的終點
        }
        SilverFishCountText.text = 0.ToString();

    }

    void Start()
    {
        GenerateFish();
        DialogueManager.Instance.StartDialogue(startDialogue); // 開始釣魚的對話
    }
    void GenerateFish()
    {
        // 生成魚
        for (int i = 0; i < fishList.Count; i++)
        {
            GameObject fishObject = Instantiate(fishPrefab);
            Fish fish = fishObject.GetComponent<Fish>();
            fish.fishIndex = i;
            fish.fishType = fishList[i].fishType;
        }
    }

    //只是魚上鉤不代表釣到
    public void OnFishCaught(GameObject fishObject)
    {
        if (state != 1) return; // 確保只有在魚線下放中狀態時才處理捕魚
        state = 2; // 捕捉到魚(檢查中)
        fishObject.transform.SetParent(hook.transform); // 將魚物件設置為魚鉤的子物件
        Fish fish = fishObject.GetComponent<Fish>();
        Debug.Log("Fish caught: " + fish.fishIndex);
        currentFishIndex = fish.fishIndex;
        fish.isCatched = true;
        fish.DisablePhysics(); // 禁用魚的物理效果防止其和其他魚碰撞

        StartCoroutine(GetPredictionResultFromBackend(fish.fishIndex)); //後端有魚的圖片
    }

    IEnumerator GetPredictionResultFromBackend(int fishIndex)
    {
        LoadingHandler.Instance.ShowLoadingScreen("魔法魚鉤辨識中...");
        string url = fishRecognitionURL + PlayerPrefs.GetString("username", "NO_Name");
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        string fishTypeName = fishList[fishIndex].fishType == 0 ? "銀龍魚" : "吳郭魚";
        string JsonBody = JsonConvert.SerializeObject(new { image_path = $"/{fishTypeName}/" + fishList[fishIndex].fishSprite.texture.name + ".jpg" });
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("key", ""));
        Debug.Log("Sending fish recognition request: " + JsonBody);
        yield return request.SendWebRequest();
        bool errorFlag = true;
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 處理成功的回應
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Fish recognition response: " + jsonResponse);
            JObject responseObject = JObject.Parse(jsonResponse);
            if ((bool)responseObject["success"])
            {
                errorFlag = false;
                confidence = (int)((float)responseObject["confidence"] * 100);
                predicTypeResult = (string)responseObject["predicted_class"] == "銀龍魚" ? 0 : 1;
            }
        }

        if (errorFlag)
        {
            // 處理錯誤的回應
            Debug.LogError("Fish recognition error: " + request.error);
            LoadingHandler.Instance.ShowLoadingScreen("魚類識別失敗，請稍後再試。");
            yield return new WaitForSeconds(2); // 等待2秒
            Fish fish = hook.GetComponentInChildren<Fish>();
            //把魚放走
            fishCaughtPanel.SetActive(false); // 隱藏捕到魚的面板
            currentFishIndex = -1; // 重置當前魚索引
            if (fish != null)
            {
                fish.EnablePhysics(); // 恢復魚的物理效果
                fish.isCatched = false; // 重置魚的捕捉狀態
                fish.transform.SetParent(null); // 將魚物件從魚鉤中移除
            }
            state = 4;
            LoadingHandler.Instance.HideLoadingScreen();
            yield break;
        }
        //文字範例: 判定結果：銀龍魚\n信心：98%
        reconnitionOutcomeText.text = "判定結果：" + (predicTypeResult == 0 ? "銀龍魚" : "吳郭魚") + "\n信心：" + confidence + "%";
        state = 3; // 捕捉到魚(拉鋸中)
        fishCaughtPanel.SetActive(true);
        LoadingHandler.Instance.HideLoadingScreen();
    }

    public void OnHookHitWall()
    {
        Debug.Log("Hook hit a wall!");
        if (state == 1)
            state = 4;
    }

    void CastTheHook()
    {
        if (state == 0) // 魚線未下放
        {
            state = 1; // 魚線下放
        }
    }

    void IncreaseQTEProgress()
    {
        qteProgress += qteProgressIncreaseSpeed;
    }


    void FixedUpdate() //因為有物理運算move to
    {
        if (!startFishing) return;

        // 更新魚線的起點和終點
        lineRenderer.SetPosition(0, fixedpoint.position);
        lineRenderer.SetPosition(1, hook.transform.position);

        if (state != 0)
        {
            CastButton.gameObject.SetActive(false); // 隱藏釣魚按鈕
        }
        else
        {
            CastButton.gameObject.SetActive(true); // 顯示釣魚按鈕
        }

        if (state == 0) // 魚線未下放
        {
            if (currentHookSwingAngle >= hookSwingAngle)
            {
                hookSwingSpeed = -Mathf.Abs(hookSwingSpeed); // 反向擺動
            }
            else if (currentHookSwingAngle <= -hookSwingAngle)
            {
                hookSwingSpeed = Mathf.Abs(hookSwingSpeed); // 反向擺動
            }
            currentHookSwingAngle += hookSwingSpeed * Time.deltaTime;
            float x = fixedpoint.position.x + Mathf.Cos((currentHookSwingAngle + 270f) * Mathf.Deg2Rad) * hookSwingRange;
            float y = fixedpoint.position.y + Mathf.Sin((currentHookSwingAngle + 270f) * Mathf.Deg2Rad) * hookSwingRange;
            hook.rb.MovePosition(new Vector2(x, y));
            hook.transform.rotation = Quaternion.Euler(0, 0, currentHookSwingAngle);
        }
        else if (state == 1) // 魚線下放
        {
            // 魚鉤向下移動
            Vector2 direction = hook.rb.position - (Vector2)fixedpoint.position;
            hook.rb.MovePosition(hook.rb.position + direction.normalized * hookCatchSpeed * Time.deltaTime);
        }
        else if (state == 2) // 捕捉到魚(檢查中)
        {
            return; //Do nothing, waiting for backend result
        }
        else if (state == 3) // 捕捉到魚(拉鋸中)
        {
            // 在這裡處理拉鋸的邏輯
            if (qtePanel.activeSelf == false)
            {
                qtePanel.SetActive(true); // 顯示QTE面板
                qteProgress = 0.5f; // 重置QTE進度條
                qteProgressSlider.value = qteProgress;
            }
            //魚鉤位置來回拉扯
            Vector3 direction = fixedpoint.position - hook.transform.position;
            float shakeOffset = Mathf.Sin(Time.time * 10f) * qteHookShakeAmplitude; // 使用正弦波來模擬抖動
            Vector3 shakePosition = hook.transform.position + direction.normalized * shakeOffset * Time.deltaTime;
            hook.transform.position = shakePosition;
            // 更新QTE進度條
            qteProgress -= qteProgressDropSpeed * Time.deltaTime; // 進度條下降

            if (qteProgress < 0f)
            {
                Fish fish = hook.GetComponentInChildren<Fish>();
                //把魚放走
                qtePanel.SetActive(false); // 隱藏QTE面板
                fishCaughtPanel.SetActive(false); // 隱藏捕到魚的面板
                currentFishIndex = -1; // 重置當前魚索引
                if (fish != null)
                {
                    fish.EnablePhysics(); // 恢復魚的物理效果
                    fish.isCatched = false; // 重置魚的捕捉狀態
                    fish.transform.SetParent(null); // 將魚物件從魚鉤中移除
                }
                state = 4;
            }
            else if (qteProgress >= 1f)
            {
                // QTE成功，捕到魚，但真正捕到魚的效果發生在魚線上收時
                Fish fish = hook.GetComponentInChildren<Fish>();
                fish.ChangeSprite();
                qtePanel.SetActive(false); // 隱藏QTE面板
                fishCaughtPanel.SetActive(false); // 隱藏捕到魚的面板
                state = 4; // 進入魚線上收狀態
            }
            else
            {
                qteProgressSlider.value = qteProgress; // 更新QTE進度條顯示
            }
        }
        else if (state == 4) // 魚線上收
        {
            // 魚鉤向上移動
            Vector3 direction = fixedpoint.position - hook.transform.position;
            hook.transform.position += direction.normalized * hookCatchSpeed * Time.deltaTime;
            if (Vector2.Distance(hook.rb.position, fixedpoint.position) <= hookSwingRange)
            {
                // 如果魚鉤已經接近固定點，則重置狀態
                state = 0;
                if (currentFishIndex != -1)
                {
                    //捕到魚
                    Fish fish = hook.GetComponentInChildren<Fish>();
                    if (fish != null)
                    {
                        fish.transform.SetParent(null);
                        currentCatchCount++;
                        if (fish.fishType == 0)
                        {
                            silverfishCount++;
                            SilverFishCountText.text = silverfishCount.ToString();
                        }
                        fish.gameObject.SetActive(false); // 隱藏魚物件
                    }
                    StartCoroutine(ShowResult(fish.fishType, fishList[currentFishIndex].fishSprite));
                    currentFishIndex = -1; // 重置當前魚索引
                    state = 0; // 重置狀態為魚線未下放
                    if (currentCatchCount >= maxCatch)
                    {
                        // 捕到足夠的魚，結束釣魚
                        if (!LevelManager_02.isSecondTimeEntered)
                        {
                            DialogueManager.Instance.StartDialogue(firstTimeEndDialogue); // 第一次結束釣魚的對話
                        }
                        else
                        {
                            DialogueManager.Instance.StartDialogue(SecondTimeEndDialogue); // 第二次結束釣魚的對話
                        }
                        startFishing = false; // 停止釣魚
                    }
                }
            }
        }
    }

    IEnumerator ShowResult(int fishType, Sprite realFishSprite)
    {
        resultPanel.SetActive(true);
        realFishImage.sprite = realFishSprite;
        if (fishType == 0)
        {
            resultImage.sprite = silverFishResultSprite;
        }
        else
        {
            resultImage.sprite = normalFishResultSprite;
        }
        yield return new WaitForSeconds(1.5f);
        resultPanel.SetActive(false);
    }

    public override void ActivateEvent(int EventIndex)
    {
        if (EventIndex == 1) // 開始釣魚
        {
            startFishing = true; //
        }
        else if (EventIndex == 2) // End fishing
        {
            if (!LevelManager_02.isSecondTimeEntered)
            {
                LevelManager_02.isSecondTimeEntered = true; // Set the flag to true after first time fishing
                LoadingHandler.Instance.ChangeScene("AILevel02_train"); // Back to training scene
            }
            else
            {
                Debug.Log("Level 02 fishing completed!");
                LevelManager_02.isSecondTimeEntered = false; // Set the flag to true after second time fishing
                LevelManager_02.haveGetRod = false;
                LevelManager_02.haveKnownThePhoto = false;
                LevelManager_02.haveOpenedBox = false;
                int totalScore = silverfishCount * 20; // 每條銀龍魚20分
                StartCoroutine(SendScoreToBackend(totalScore));
            }
        }
    }


    IEnumerator SendScoreToBackend(int score)
    {
        LoadingHandler.Instance.ShowLoadingScreen("正在上傳分數...");
        Debug.Log("Sending score to backend: " + score);
        string url = scoreBoardURL + PlayerPrefs.GetString("username") + "/score";
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        string jsonBody = JsonConvert.SerializeObject(new { score = score, level = 2 });
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
