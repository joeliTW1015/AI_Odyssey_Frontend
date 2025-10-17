using UnityEngine;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] Button level1Button;
    [SerializeField] Button level2Button;
    [SerializeField] Button level3Button;

    void Awake()
    {
        level1Button.onClick.AddListener(() => LoadLevel("AILevel01"));
        level2Button.onClick.AddListener(() => LoadLevel("AILevel02_train"));
        //level3Button.onClick.AddListener(() => LoadLevel("AILevel03"));
    }

    void LoadLevel(string levelName)
    {
        LoadingHandler.Instance.ChangeScene(levelName);
    }
}
