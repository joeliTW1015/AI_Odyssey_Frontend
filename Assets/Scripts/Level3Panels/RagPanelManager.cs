using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections;

public class RagPanelManager : MonoBehaviour
{
    [SerializeField] InputField responseText;
    [SerializeField] InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] Button quitButton;
    [SerializeField] Text bookNameText;
    [SerializeField] Button changeBookButton;
    [SerializeField] string backendUrl_1;
    [SerializeField] string backendUrl_2;
    int currentBookIndex = 0;

    string currentResponse;

    private void Awake()
    {
        submitButton.onClick.AddListener(SubmitPrompt);
        quitButton.onClick.AddListener(QuitChatBot);
        changeBookButton.onClick.AddListener(ChangeBook);
        bookNameText.text = "當前文本：\n〈黑與白－虎鯨〉";
        currentBookIndex = 0;
    }

    private void SubmitPrompt()
    {
        string userInput = inputField.text;
        if (!string.IsNullOrEmpty(userInput))
        {
            currentResponse = "";
            responseText.text = currentResponse;
            StartCoroutine(GetResponseFromBackend(userInput));
        }
    }

    IEnumerator GetResponseFromBackend(string userInput)
    {
        LoadingHandler.Instance.ShowLoadingScreen("機器人檢索中...");
        UnityWebRequest request = new UnityWebRequest(currentBookIndex == 0 ? backendUrl_1 : backendUrl_2, "POST");
        string jsonBody = JsonConvert.SerializeObject(new { query = userInput });
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
        foreach (var item in jsonResponse["results"])
        {
            currentResponse += item["text"].ToString() + "\n\n";
        }
        responseText.text = currentResponse;
        //Force Update UI
        LayoutRebuilder.ForceRebuildLayoutImmediate(responseText.GetComponent<RectTransform>());
        LoadingHandler.Instance.HideLoadingScreen();
    }

    private void ChangeBook()
    {
        currentResponse = "";
        responseText.text = currentResponse;
        currentBookIndex = (currentBookIndex + 1) % 2;
        if (currentBookIndex == 0)
        {
            bookNameText.text = "當前文本：\n〈黑與白－虎鯨〉";
        }
        else
        {
            bookNameText.text = "當前文本：\n《紅樓夢》";
        }
    }

    private void QuitChatBot()
    {
        gameObject.SetActive(false);
    }
}
    