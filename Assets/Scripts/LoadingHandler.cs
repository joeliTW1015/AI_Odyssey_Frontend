using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingHandler : MonoBehaviour
{
    public static LoadingHandler Instance;
    [SerializeField] private GameObject loadingScreen;
    Image bg;
    Image blackPanel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        loadingScreen.SetActive(false);
        bg = GetComponent<Image>();
        bg.enabled = true;
        bg.color = Color.black; // Set background color to black
        StartCoroutine(BgBlackToTransparent()); // Start fading out the background
    }
    IEnumerator BgBlackToTransparent()
    {
        Debug.Log("Fading out background to transparent");
        float duration = 2f; // Duration of the fade
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
        float duration = 1f; // Duration of the fade
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

    public void ShowLoadingScreen()
    {
        bg.enabled = true;
        loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        bg.enabled = false;
        loadingScreen.SetActive(false);
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(BgTransparentToBlack(sceneName));
    }
}
