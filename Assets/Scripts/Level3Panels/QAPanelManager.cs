using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    [TextArea] public string questionText;
    [TextArea] public string answer;
}

public class QAPanelManager : MonoBehaviour
{
    [SerializeField] InputField questionText;
    [SerializeField] Text questionNumberText;
    [SerializeField] InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] Button quitButton;
    [Header("Question List")]
    [SerializeField] Question[] questions;
    int currentQuestionIndex = 0;

    void Awake()
    {
        currentQuestionIndex = 0;
        submitButton.onClick.AddListener(() => StartCoroutine(SubmitAnswer()));
        quitButton.onClick.AddListener(QuitQAPanel);
        questionText.text = questions[currentQuestionIndex].questionText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(questionText.GetComponent<RectTransform>());
        questionNumberText.text = "問題 " + (currentQuestionIndex + 1) + " / " + questions.Length;
    }

    IEnumerator SubmitAnswer()
    {
        if (inputField.text == questions[currentQuestionIndex].answer)
        {
            LoadingHandler.Instance.ShowLoadingScreen("回答正確");
            yield return new WaitForSeconds(2);
            LoadingHandler.Instance.HideLoadingScreen();

            currentQuestionIndex++;
            if(currentQuestionIndex >= questions.Length)
            {
                LoadingHandler.Instance.ShowLoadingScreen("所有問題已完成");
                yield return new WaitForSeconds(2);
                LoadingHandler.Instance.HideLoadingScreen();
                LevelManager_03.Instance.ActivateEvent(4);
                gameObject.SetActive(false);
                yield break;
            }

            questionText.text = questions[currentQuestionIndex].questionText;
            LayoutRebuilder.ForceRebuildLayoutImmediate(questionText.GetComponent<RectTransform>());
            questionNumberText.text = "問題 " + (currentQuestionIndex + 1) + " / " + questions.Length;
        }
        else
        {
            LoadingHandler.Instance.ShowLoadingScreen("回答錯誤");
            yield return new WaitForSeconds(2);
            LoadingHandler.Instance.HideLoadingScreen();
            yield break;
        }
    }

    void QuitQAPanel()
    {
        gameObject.SetActive(false);
    }
}
