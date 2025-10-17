using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    [SerializeField] private Text guideText;
    public static Guide Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        guideText.text = "當前目標：\n";
    }

    public void SetGuideText(string text)
    {
        if (guideText != null)
        {
            guideText.text = "當前目標：\n" + text;
        }
    }
}
