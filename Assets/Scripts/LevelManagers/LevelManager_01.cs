using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.Networking;
using NUnit.Framework.Constraints;
using System.Collections;

public class LevelManager_01 : LevelManagerBase
{
    public static LevelManager_01 Instance;
    [Header("Cooking Interface")]
    [SerializeField] GameObject CookingInterface;
    [SerializeField] InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] Button quitCookingInterfaceButton;
    [SerializeField] Image outcomeImage;
    Texture2D outcomeImageTexture;

    [Header("Judging Interface")]
    [SerializeField] GameObject JudgingInterface;
    [SerializeField] Image foodImage;
    [SerializeField] Text judgingReviewText;
    [SerializeField] Slider scoreSlider;
    [SerializeField] [Range(0, 100)] int successThreshold = 80; // Threshold for success in judging
    [SerializeField] Button quitJudgingInterfaceButton;
    [SerializeField] DialogueSequence successDialogue;
    [SerializeField] DialogueSequence failureDialogue;
    [SerializeField] DialogueSequence noInputDialogue;
    bool isJudging = false;
    int currentScore = 0;
    string feedback = "";
    [Header("Backend URL")]
    [SerializeField] string stringToImageUrl;
    [SerializeField] string imageToStringUrl;
    [Header("placeholder for backend image")]
    [SerializeField] Texture2D placeholderImageBad, placeholderImageGood;

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

    public override void ActivateEvent(int EventIndex)
    {
        if (EventIndex == 1) // Activate the cooking interface
        {
            CookingInterface.SetActive(true);
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
            StartCoroutine(GetImageFromBackend(inputText));
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

    IEnumerator GetImageFromBackend(string prompt)
    {
        //placeholder for actual backend call
        Debug.Log("Getting image from backend for prompt: " + prompt);
        //TODO: api post request to get image
        //TODO : loading image and error handling
        LoadingHandler.Instance.ShowLoadingScreen();
        yield return new WaitForSeconds(3); // Simulate network delay
        LoadingHandler.Instance.HideLoadingScreen();
        Debug.Log("Image received from backend.");
        if(prompt == "good")
        {
            outcomeImageTexture = placeholderImageGood;
        }
        else 
        {
            outcomeImageTexture = placeholderImageBad;
        }
        outcomeImage.sprite = Sprite.Create(outcomeImageTexture, new Rect(0, 0, outcomeImageTexture.width, outcomeImageTexture.height), new Vector2(0.5f, 0.5f));
    }

    void StartJudging()
    {
        if (outcomeImageTexture == null)
        {
            DialogueManager.Instance.StartDialogue(noInputDialogue);
            return;
        }
        isJudging = true;
        JudgingInterface.SetActive(true);
        foodImage.sprite = Sprite.Create(outcomeImageTexture, new Rect(0, 0, outcomeImageTexture.width, outcomeImageTexture.height), new Vector2(0.5f, 0.5f));
        StartCoroutine(GetReviewFromBackend());
    }

    void EndJudging()
    {
        if(isJudging)
        {
            Debug.LogWarning("Judging is still in progress. Please wait for the review to complete.");
            return;
        }
        JudgingInterface.SetActive(false);
        if (currentScore >= successThreshold)
        {
            DialogueManager.Instance.StartDialogue(successDialogue);
        }
        else
        {
            DialogueManager.Instance.StartDialogue(failureDialogue);
        }
    }

    IEnumerator GetReviewFromBackend()
    {
        //placeholder for actual backend call
        Debug.Log("Getting review from backend for image.");
        LoadingHandler.Instance.ShowLoadingScreen();
        yield return new WaitForSeconds(3);
        LoadingHandler.Instance.HideLoadingScreen();
        // Simulate network delay
        //TODO: api post request to get review and score
        //TODO : loading icon and error handling
        Debug.Log("Review received from backend.");
        if(outcomeImageTexture == placeholderImageGood)
        {
            currentScore = 90; // Simulated score for good image
            feedback = "好吃好吃";
        }
        else
        {
            currentScore = 30; // Simulated score for bad image
            feedback = "噁心死了";
        }
        judgingReviewText.text = feedback;
        scoreSlider.value = currentScore / 100f;
        isJudging = false;   
    }

    private void LevelComplete()
    {
        Debug.Log("Level 1 complete!");
    }
}
