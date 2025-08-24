using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingHandler : MonoBehaviour
{
    public static LoadingHandler Instance;
    [SerializeField] private GameObject textBox;
    [SerializeField] Button homeButton;
    [SerializeField] float fadeDuration = 1f; // Duration for the fade effect
    Image bg;
    Image blackPanel;
    Text message;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        message = textBox.GetComponentInChildren<Text>();
        textBox.SetActive(false);

        bg = GetComponent<Image>();
        bg.enabled = true;
        bg.color = Color.black; // Set background color to black
        StartCoroutine(BgBlackToTransparent()); // Start fading out the background
        homeButton.onClick.AddListener(() => ChangeScene("LevelMenu"));
    }
    IEnumerator BgBlackToTransparent()
    {
        Debug.Log("Fading out background to transparent");
        float duration = fadeDuration; // Duration of the fade
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, alpha);
            yield return null;
        }
        bg.enabled = false; // Disable the background after fading out
        //set bg alpha to 0.8
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0.8f);
    }
    IEnumerator BgTransparentToBlack(string sceneName)
    {
        float duration = fadeDuration; // Duration of the fade
        float elapsedTime = 0f;
        bg.enabled = true; // Enable the background after fading in
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, alpha);
            yield return null;
        }
        
        if (!string.IsNullOrEmpty(sceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is null or empty. Cannot load scene.");
        }
    }

    public void ShowLoadingScreen(string messageText = "載入中...")
    {
        message.text = messageText;
        bg.enabled = true;
        textBox.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        bg.enabled = false;
        textBox.SetActive(false);
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(BgTransparentToBlack(sceneName));
    }
}
