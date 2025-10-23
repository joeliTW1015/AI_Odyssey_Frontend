using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;



[System.Serializable]
public class TrainClass
{
    public string name;
    public List<string> images;
}

[System.Serializable]
public class TrainDatasetWrapper
{
    public List<TrainClass> train_dataset = new List<TrainClass>();
}

public class LevelManager_02 : LevelManagerBase
{
    //GameProgress
    public static bool isSecondTimeEntered = false; //釣魚完後第二次回到這個關卡
    public static bool haveGetRod = false; //是否已經拿到釣竿(富翁第一次對話後)
    public static bool haveOpenedBox = false; //是否已經打開箱子
    public static bool haveKnownThePhoto = false; //是否已經和屋主對話過照片的事


    public static LevelManager_02 Instance;
    [Header("URL")]
    [SerializeField] string submitLabelUrl;

    [Header("TrainingSet")]
    [SerializeField] List<Sprite> normalFishSprites;
    [SerializeField] List<Sprite> silverFishSprites;
    int currentFishType = 0; //0 for silver fish, 1 for normal fish
    int currentNormalFishIndex = 0;
    int currentSilverFishIndex = 0;
    int labelCount = 0;
    TrainDatasetWrapper trainDatasetWrapper = new TrainDatasetWrapper();

    [Header("Dialogues")]
    [SerializeField] DialogueSequence firstTimeEnterInitDialogue;
    [SerializeField] DialogueSequence secondTimeEnterInitDialogue;

    [SerializeField] DialogueSequence firstTimeFishingDialogue;
    [SerializeField] DialogueSequence secondTimeFishingDialogue;
    [SerializeField] DialogueSequence houseOwnerPhotoDialogue;
    [SerializeField] DialogueSequence knowPhotoBoxDialogue;
    [SerializeField] DialogueSequence finishLabelingDialogue; //完成標記後的對話
    [SerializeField] DialogueSequence blockDialogue; //還有未完成的任務或對話，無法進一步互動

    [Header("Dialogue Triggers")]
    [SerializeField] DialogueTrigger houseownerTrigger; //屋主對話觸發器

    [Header("References")]
    [SerializeField] SpriteRenderer boxSpriteRenderer; //箱子的SpriteRenderer
    [SerializeField] Sprite boxOpenedSprite; //打開後的箱子圖片
    [SerializeField] GameObject labelingInterface; //標記介面
    [SerializeField] Image imageToBeLabeled; //圖片要被標記的UI
    [SerializeField] Button silverFishButton; //銀龍魚按鈕
    [SerializeField] Button normalFishButton; //吳郭魚按鈕
    [SerializeField] Text labelCountText; //標記數量的UI文本





    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;


        silverFishButton.onClick.AddListener(() => LableFishImage(0)); //銀龍魚
        normalFishButton.onClick.AddListener(() => LableFishImage(1)); //吳郭魚(normal fish)
    }

    void Start()
    {
        if (isSecondTimeEntered)
        {
            houseownerTrigger.dialogueSequence = houseOwnerPhotoDialogue;
            DialogueManager.Instance.StartDialogue(secondTimeEnterInitDialogue);
        }
        else
        {
            DialogueManager.Instance.StartDialogue(firstTimeEnterInitDialogue);
        }
    }

    public override void ActivateEvent(int EventIndex)
    {
        PlayerMove.canMove = true; //後面如果沒有特別設定，預設都可以移動
        if (EventIndex == 1) // interact with the fishing pool
        {
            if (!haveGetRod)
            {
                DialogueManager.Instance.StartDialogue(blockDialogue);
            }
            else if (isSecondTimeEntered)
            {
                if (haveOpenedBox)
                {
                    DialogueManager.Instance.StartDialogue(secondTimeFishingDialogue);
                }
                else
                {
                    DialogueManager.Instance.StartDialogue(blockDialogue);
                }
            }
            else
            {
                DialogueManager.Instance.StartDialogue(firstTimeFishingDialogue);
            }
        }
        else if (EventIndex == 2) // Load the fishing game scene
        {
            Debug.Log("Load the fishing game scene");
            // Logic to load the fishing game scene can be added here
            LoadingHandler.Instance.ChangeScene("AILevel02_fishing");
        }
        else if (EventIndex == 3) // Interact with the box
        {
            if (haveKnownThePhoto)
            {
                DialogueManager.Instance.StartDialogue(knowPhotoBoxDialogue);
                haveOpenedBox = true; // Set the flag to true after opening the box
                boxSpriteRenderer.sprite = boxOpenedSprite; // Change the box sprite to opened state
            }
            else
            {
                DialogueManager.Instance.StartDialogue(blockDialogue);
            }
        }
        else if (EventIndex == 4) // Activate Labeling interface
        {
            PlayerMove.canMove = false;
            labelingInterface.SetActive(true);
            currentNormalFishIndex = 0; // Reset the index for normal fish
            currentSilverFishIndex = 0; // Reset the index for silver fish
            labelCount = 0; // Reset the label count
            trainDatasetWrapper.train_dataset.Add(new TrainClass { name = "銀龍魚", images = new List<string>() });
            trainDatasetWrapper.train_dataset.Add(new TrainClass { name = "吳郭魚", images = new List<string>() });

            GetFishSpriteFromTrainSet(); // Get the fish sprite from the training set
            //TODO: 處理兩次釣魚的差異
            labelCountText.text = $"目前資料量：\n{0}張圖片"; // Update the label count text

        }
        else if (EventIndex == 5) // know the photo
        {
            haveKnownThePhoto = true;
        }
        else if (EventIndex == 6) // get the fishing rod
        {
            haveGetRod = true;
        }
        else if (EventIndex == 7) // finish labeling
        {
            labelingInterface.SetActive(false);
            StartCoroutine(SubmitLabelsToBackend());

        }
        else
        {
            Debug.LogWarning("Unknown event index: " + EventIndex);
        }

    }

    void GetFishSpriteFromTrainSet()
    {
        //決定接下來顯示的圖片的魚的種類
        if (currentNormalFishIndex >= normalFishSprites.Count)
        {
            currentFishType = 0; //silver fish
        }
        else if (currentSilverFishIndex >= silverFishSprites.Count)
        {
            currentFishType = 1; //normal fish
        }
        else
        {
            currentFishType = UnityEngine.Random.Range(0, 2); // Randomly choose between silver fish and normal fish
        }

        //顯示圖片至UI並將圖片索引加1
        if (currentFishType == 0)
        {
            imageToBeLabeled.sprite = silverFishSprites[currentSilverFishIndex];
            currentSilverFishIndex++;
        }
        else
        {
            imageToBeLabeled.sprite = normalFishSprites[currentNormalFishIndex];
            currentNormalFishIndex++;
        }

    }

    void LableFishImage(int fishType) //0是銀龍魚, 1是吳郭魚
    {
        //儲存標記結果
        Debug.Log("Labeled fish type: " + fishType);
        if(fishType == 0)
        {
            //將sprite的原始檔案名稱加入到對應的魚類列表中
            trainDatasetWrapper.train_dataset[0].images.Add("/銀龍魚/" + imageToBeLabeled.sprite.texture.name + ".jpg"); // Add to silver fish
        }
        else if (fishType == 1)
        {
            trainDatasetWrapper.train_dataset[1].images.Add("/吳郭魚/" + imageToBeLabeled.sprite.texture.name + ".jpg"); // Add to normal fish
        }
        else
        {
            Debug.LogError("Unknown fish type: " + fishType);
            return;
        }

        labelCount++;
        labelCountText.text = $"目前資料量：\n{labelCount}張圖片"; // Update the label count text
        if ((!isSecondTimeEntered && labelCount >= 4) || labelCount >= normalFishSprites.Count + silverFishSprites.Count)
        {
            //結束標記
            ActivateEvent(7); // Finish labeling
            return;
        }
        GetFishSpriteFromTrainSet(); // Get the next fish sprite from the training set  
    }

    IEnumerator SubmitLabelsToBackend()
    {
        LoadingHandler.Instance.ShowLoadingScreen("正在訓練辨識模型...");
        string url = submitLabelUrl + PlayerPrefs.GetString("username", "NO_USERNAME!"); 
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        string jsonBody = JsonConvert.SerializeObject(trainDatasetWrapper, Formatting.Indented);
        Debug.Log("Submitting labels to backend: " + jsonBody);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("key")); //使用存儲的token才能上傳標記資料

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Labels submitted successfully.");
        }
        else
        {
            PlayerMove.canMove = true;
            Debug.LogError("Error submitting labels: " + request.error);
            LoadingHandler.Instance.ShowLoadingScreen("上傳標記資料失敗");
            yield return new WaitForSeconds(2); // Wait for 2 seconds to show the error message
            LoadingHandler.Instance.HideLoadingScreen();
            yield break;
        }

        LoadingHandler.Instance.HideLoadingScreen();
        DialogueManager.Instance.StartDialogue(finishLabelingDialogue);
    }


}
