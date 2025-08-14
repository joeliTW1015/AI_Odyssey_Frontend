using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] InputField userNameInputField;
    [SerializeField] InputField passwordInputField;
    [SerializeField] Button LoginButton;
    [SerializeField] Button RegisterButton;
    void Awake()
    {
        LoginButton.onClick.AddListener(SubmitLogin);
        RegisterButton.onClick.AddListener(SubmitRegister);
    }

    async void SubmitRegister()
    {
        string userName = userNameInputField.text;
        string password = passwordInputField.text;
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username or password cannot be empty.");
            StartCoroutine(ShowError("使用者名稱或密碼不能為空。"));
            return;
        }
        string registerJson = JsonConvert.SerializeObject(new { username = userName, password = password });
        LoadingHandler.Instance.ShowLoadingScreen("正在註冊...");
        string url = "https://ai-odyssey-backend-rbzz.onrender.com/auth/register"; // Replace with your actual
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(registerJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            StartCoroutine(ShowError(request.error));
        }
        else
        {
            // Assuming the response contains a success message
            string responseText = request.downloadHandler.text;
            StartCoroutine(ShowRegisterSuccess(responseText));
        }
    }

    async void SubmitLogin()
    {
        string userName = userNameInputField.text;
        string password = passwordInputField.text;
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username or password cannot be empty.");
            StartCoroutine(ShowError("使用者名稱或密碼不能為空。"));
            return;
        }
        string loginJson = JsonConvert.SerializeObject(new { username = userName, password = password });
        LoadingHandler.Instance.ShowLoadingScreen("正在登入...");
        string url = "https://ai-odyssey-backend-rbzz.onrender.com/auth/login"; // Replace with your actual
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(loginJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            StartCoroutine(ShowError(request.error));
        }
        else
        {
            // Assuming the response contains a success message
            string responseText = request.downloadHandler.text;
            StartCoroutine(ShowSuccessLogin(responseText));
        }
    }

    IEnumerator ShowRegisterSuccess(string successMessage)
    {
        LoadingHandler.Instance.ShowLoadingScreen("註冊成功: " + successMessage);
        Debug.Log(successMessage);
        // Optionally, show a success message to the user
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before hiding the success message
        LoadingHandler.Instance.HideLoadingScreen();
        LoadingHandler.Instance.ChangeScene("LevelMenu"); // Change to the main menu scene
    }

    IEnumerator ShowError(string errorMessage)
    {
        LoadingHandler.Instance.ShowLoadingScreen("登入或註冊失敗: " + errorMessage);
        Debug.LogError(errorMessage);
        // Optionally, show an error message to the user
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before hiding the error
        LoadingHandler.Instance.HideLoadingScreen();
        userNameInputField.text = "";
        passwordInputField.text = "";
    }

    IEnumerator ShowSuccessLogin(string successMessage)
    {
        LoadingHandler.Instance.ShowLoadingScreen("登入成功: " + successMessage);
        Debug.Log(successMessage);
        // Optionally, show a success message to the user
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before hiding the success message
        LoadingHandler.Instance.HideLoadingScreen();
        LoadingHandler.Instance.ChangeScene("LevelMenu"); // Change to the main menu scene
    }

    

}
