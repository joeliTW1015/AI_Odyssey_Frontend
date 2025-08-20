using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.Networking;
using NUnit.Framework.Constraints;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Unity.Android.Gradle.Manifest;

[System.Serializable]
public class Dish
{
    [TextArea] public string dishName;
    [TextArea] public string dishRequirements;
}
public class LevelManager_01 : LevelManagerBase
{
    public static LevelManager_01 Instance;
    [Header("Dishes")]
    int currentDishIndex = 0;
    [SerializeField] Dish[] dishes;
    [Header("Cooking Interface")]
    [SerializeField] GameObject CookingInterface;
    [SerializeField] Text dishNameText;
    [SerializeField] InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] Button quitCookingInterfaceButton;
    [SerializeField] Image outcomeImage;
    Texture2D outcomeImageTexture;
    string outcomeImageName;

    [Header("Judging Interface")]
    [SerializeField] GameObject JudgingInterface;
    [SerializeField] Image foodImage;
    [SerializeField] Text judgingReviewText;
    [SerializeField] Slider scoreSlider;
    [SerializeField][Range(0, 100)] int successThreshold = 60; // Threshold for success in judging
    [SerializeField] Button quitJudgingInterfaceButton;
    
    bool isJudging = false;
    int currentScore = 0;
    [Header("Backend URL")]
    [SerializeField] string stringToImageUrl;
    [SerializeField] string imageToStringUrl;

    [Header("Dialogue")]
    [SerializeField] DialogueSequence startDialogue;
    [SerializeField] DialogueSequence endDialogue;
    [SerializeField] DialogueSequence successDialogue;
    [SerializeField] DialogueSequence failureDialogue;
    [SerializeField] DialogueSequence noInputDialogue;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        CookingInterface.SetActive(false);
        JudgingInterface.SetActive(false);
        outcomeImageTexture = null;
        submitButton.onClick.AddListener(SubmitPrompt);
        quitCookingInterfaceButton.onClick.AddListener(QuitCookingInterface);
        quitJudgingInterfaceButton.onClick.AddListener(EndJudging);
    }

    private void Start()
    {
        DialogueManager.Instance.StartDialogue(startDialogue);
    }

    public override void ActivateEvent(int EventIndex)
    {
        if (EventIndex == 1) // Activate the cooking interface
        {
            CookingInterface.SetActive(true);
            dishNameText.text = "要做的料理: " + dishes[currentDishIndex].dishName;
            inputField.text = "";
        }
        else if (EventIndex == 2) // Start the judging process
        {
            StartJudging();
        }
        else if (EventIndex == 3) // End the level
        {
            LevelComplete();
        }
        else
        {
            Debug.LogWarning("Unknown Event index: " + EventIndex);
        }
    }

    void SubmitPrompt()
    {
        string inputText = inputField.text;
        if (!string.IsNullOrEmpty(inputText))
        {
            // Process the input text
            Debug.Log("Submitted: " + inputText);
            GetImageFromBackend(inputText);
        }
        else
        {
            Debug.LogWarning("Input field is empty!");
        }
    }

    void QuitCookingInterface()
    {
        //保留圖片和輸入框文字
        CookingInterface.SetActive(false);
    }

    async void GetImageFromBackend(string prompt)
    {
        LoadingHandler.Instance.ShowLoadingScreen();

        Debug.Log("Getting image from backend for prompt: " + prompt);
        UnityWebRequest request = new UnityWebRequest(stringToImageUrl, "POST");
        string jsonBody = JsonConvert.SerializeObject(new { prompt = prompt });
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("key")); //使用存儲的token才能獲取圖片
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            //show error message for 2 seconds
            LoadingHandler.Instance.ShowLoadingScreen("獲取圖片失敗: " + request.error);
            await Task.Delay(2000);
            LoadingHandler.Instance.HideLoadingScreen();
            return;
        }
        else
        {
            Debug.Log("Image received from backend: " + request.downloadHandler.text);
            string responseText = request.downloadHandler.text;
            string imageUrl = JObject.Parse(responseText)["image_url"].ToString();
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
            await imageRequest.SendWebRequest();
            if (imageRequest.result == UnityWebRequest.Result.ConnectionError || imageRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                LoadingHandler.Instance.ShowLoadingScreen("獲取圖片失敗: " + request.error);
                await Task.Delay(2000);
                LoadingHandler.Instance.HideLoadingScreen();
                return;
            }
            else
            {
                string imageName = JObject.Parse(responseText)["file_info"]["original_filename"].ToString();
                Debug.Log("Image name: " + imageName);
                outcomeImageName = imageName;
                outcomeImageTexture = DownloadHandlerTexture.GetContent(imageRequest);
            }
            outcomeImage.sprite = Sprite.Create(outcomeImageTexture, new Rect(0, 0, outcomeImageTexture.width, outcomeImageTexture.height), new Vector2(0.5f, 0.5f));
        }
        LoadingHandler.Instance.HideLoadingScreen();
    }

    void StartJudging()
    {
        if (outcomeImageTexture == null || string.IsNullOrEmpty(outcomeImageName))
        {
            DialogueManager.Instance.StartDialogue(noInputDialogue);
            return;
        }
        isJudging = true;
        JudgingInterface.SetActive(true);
        foodImage.sprite = Sprite.Create(outcomeImageTexture, new Rect(0, 0, outcomeImageTexture.width, outcomeImageTexture.height), new Vector2(0.5f, 0.5f));
        GetReviewFromBackend();
    }

    void EndJudging()
    {
        if (isJudging)
        {
            Debug.LogWarning("Judging is still in progress. Please wait for the review to complete.");
            return;
        }
        JudgingInterface.SetActive(false);
        if (currentScore >= successThreshold)
        {
            currentDishIndex++;
            if (currentDishIndex < dishes.Length)
            {
                DialogueManager.Instance.StartDialogue(successDialogue);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(endDialogue);
            }
        }
        else
        {
            DialogueManager.Instance.StartDialogue(failureDialogue);
        }
    }

    async void GetReviewFromBackend()
    {
        //placeholder for actual backend call
        Debug.Log("Getting review from backend for image.");
        LoadingHandler.Instance.ShowLoadingScreen();
        UnityWebRequest request = new UnityWebRequest(imageToStringUrl, "POST");
        string jsonBody = JsonConvert.SerializeObject(new { image_hash = outcomeImageName, dish_expect = dishes[currentDishIndex].dishRequirements });
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("key")); //使用存儲的token才能獲取評價
        request.SetRequestHeader("Content-Type", "application/json");
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error getting review: " + request.error);
            LoadingHandler.Instance.ShowLoadingScreen("獲取評價失敗: " + request.error);
            await Task.Delay(2000);
            LoadingHandler.Instance.HideLoadingScreen();
            JudgingInterface.SetActive(false);
            isJudging = false;
            return;
        }
        JObject responseJson = JObject.Parse(request.downloadHandler.text);
        string feedback = responseJson["analysis"].ToString();
        currentScore = (int)responseJson["score"];

        Debug.Log("Review received from backend.");
        judgingReviewText.text = "分數: " + currentScore + "/100" + "\n" + feedback;
        scoreSlider.value = currentScore / 100f;
        isJudging = false;
        LoadingHandler.Instance.HideLoadingScreen();
    }

    private void LevelComplete()
    {
        Debug.Log("Level 1 complete!");
        LoadingHandler.Instance.ChangeScene("LevelMenu");
    }
}
