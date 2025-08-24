using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class ChatBotPanel : MonoBehaviour
{
    [SerializeField] Text responseText;
    [SerializeField] InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] Button quitButton;
    [SerializeField] string backendUrl;

    string currentResponse;

    private void Awake()
    {
        submitButton.onClick.AddListener(SubmitPrompt);
        quitButton.onClick.AddListener(QuitChatBot);
    }

    private void SubmitPrompt()
    {
        string userInput = inputField.text;
        if (!string.IsNullOrEmpty(userInput))
        {
            GetResponseFromBackend(userInput);
        }
    }

    async void GetResponseFromBackend(string userInput)
    {
        LoadingHandler.Instance.ShowLoadingScreen();
        UnityWebRequest request = new UnityWebRequest(backendUrl, "POST");
        string jsonBody = JsonConvert.SerializeObject(new { question = userInput });
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("key"));

        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            LoadingHandler.Instance.ShowLoadingScreen("錯誤: " + request.error);
            await Task.Delay(2000);
            LoadingHandler.Instance.HideLoadingScreen();
            return;
        }

        string rawResponse = request.downloadHandler.text;
        JObject jsonResponse = JObject.Parse(rawResponse);
        currentResponse = jsonResponse["answer"].ToString();
        responseText.text = currentResponse;
        LoadingHandler.Instance.HideLoadingScreen();
    }

    private void QuitChatBot()
    {
        gameObject.SetActive(false);
    }
}
    