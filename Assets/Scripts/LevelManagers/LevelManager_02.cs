using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager_02 : LevelManagerBase
{
    //GameProgress
    public static bool isSecondTimeEntered = false; //釣魚完後第二次回到這個關卡
    public static bool haveGetRod = false; //是否已經拿到釣竿(富翁第一次對話後)
    public static bool haveOpenedBox = false; //是否已經打開箱子
    public static bool haveKnownThePhoto = false; //是否已經和屋主對話過照片的事


    public static LevelManager_02 Instance;
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

    [Header("placeholder")]
    [SerializeField] List<Sprite> placeholderFishSprites; //用於標記的placeholder圖片

    Sprite fishSprite; //魚的圖片
    int fishSpriteIndex = 0; //用於placeholder圖片的索引




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
            labelingInterface.SetActive(true);
            GetFishSpriteFromBackEnd();
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
            DialogueManager.Instance.StartDialogue(finishLabelingDialogue);
        }
        else
        {
            Debug.LogWarning("Unknown event index: " + EventIndex);
        }

    }

    async void GetFishSpriteFromBackEnd()
    {
        //placeholder for the logic to get the fish sprite from the backend
        // This could involve an API call to fetch the sprite and then setting it to the image
        LoadingHandler.Instance.ShowLoadingScreen();
        await System.Threading.Tasks.Task.Delay(1000); // Simulating a delay for fetching the sprite
        fishSprite = placeholderFishSprites[fishSpriteIndex]; // For now, using a placeholder sprite
        LoadingHandler.Instance.HideLoadingScreen();
        imageToBeLabeled.sprite = fishSprite; // Set the sprite to the image to
    }

    void NextFishImage()
    {
        fishSpriteIndex++;
        GetFishSpriteFromBackEnd();
        imageToBeLabeled.sprite = fishSprite; // Set the sprite to the image to
        if (fishSpriteIndex >= placeholderFishSprites.Count)
        {
            fishSpriteIndex = 0; // Reset to the first image if we reach the end
            labelingInterface.SetActive(false); // Hide the labeling interface after cycling through all images
            DialogueManager.Instance.StartDialogue(finishLabelingDialogue); // Start the finish labeling dialogue
        }
    }

    void LableFishImage(int fishType) //0是銀龍魚, 1是吳郭魚
    {
        //TODO 將選擇的結果傳給後端
        Debug.Log("Labeled fish type: " + fishType);
        NextFishImage(); // Proceed to the next fish image after labeling
        labelCountText.text = $"目前資料量：\n{fishSpriteIndex}張圖片"; // Update the label count text
    }
}
