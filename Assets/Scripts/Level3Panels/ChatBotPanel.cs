using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections;

public class ChatBotPanel : MonoBehaviour
{
    [SerializeField] InputField responseText;
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
            StartCoroutine(GetResponseFromBackend(userInput));
        }
    }

    IEnumerator GetResponseFromBackend(string userInput)
    {
        LoadingHandler.Instance.ShowLoadingScreen();
        UnityWebRequest request = new UnityWebRequest(backendUrl, "POST");
        string jsonBody = JsonConvert.SerializeObject(new { question = userInput });
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("key"));

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            LoadingHandler.Instance.ShowLoadingScreen("發生錯誤: " + request.error);
            yield return new WaitForSeconds(2);
            LoadingHandler.Instance.HideLoadingScreen();
            yield break;
        }

        string rawResponse = request.downloadHandler.text;
        JObject jsonResponse = JObject.Parse(rawResponse);
        currentResponse = jsonResponse["answer"].ToString();
        responseText.text = currentResponse;
        //Force Update UI
        LayoutRebuilder.ForceRebuildLayoutImmediate(responseText.GetComponent<RectTransform>());
        LoadingHandler.Instance.HideLoadingScreen();
    }

    private void QuitChatBot()
    {
        gameObject.SetActive(false);
    }
}
    