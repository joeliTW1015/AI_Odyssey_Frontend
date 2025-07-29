using UnityEngine;
using UnityEngine.UI;

public class LoadingHandler : MonoBehaviour
{
    public static LoadingHandler Instance;
    [SerializeField] private GameObject loadingScreen;
    Image bg;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        loadingScreen.SetActive(false);
        bg = loadingScreen.GetComponent<Image>();
        bg.enabled = false;
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

}
