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
    [SerializeField] Text questionText;
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
        submitButton.onClick.AddListener(SubmitAnswer);
        quitButton.onClick.AddListener(QuitQAPanel);
        questionText.text = questions[currentQuestionIndex].questionText;
        questionNumberText.text = "問題 " + (currentQuestionIndex + 1) + " / " + questions.Length;
    }

    async void SubmitAnswer()
    {
        if (inputField.text == questions[currentQuestionIndex].answer)
        {
            LoadingHandler.Instance.ShowLoadingScreen("回答正確");
            await System.Threading.Tasks.Task.Delay(2000); 
            LoadingHandler.Instance.HideLoadingScreen();

            currentQuestionIndex++;
            if(currentQuestionIndex >= questions.Length)
            {
                LoadingHandler.Instance.ShowLoadingScreen("所有問題已完成");
                await System.Threading.Tasks.Task.Delay(2000); 
                LoadingHandler.Instance.HideLoadingScreen();
                LevelManager_03.Instance.ActivateEvent(3);
                gameObject.SetActive(false);
                return;
            }

            questionText.text = questions[currentQuestionIndex].questionText;
            questionNumberText.text = "問題 " + (currentQuestionIndex + 1) + " / " + questions.Length;
        }
        else
        {
            LoadingHandler.Instance.ShowLoadingScreen("回答錯誤");
            await System.Threading.Tasks.Task.Delay(2000); 
            LoadingHandler.Instance.HideLoadingScreen();
            return;
        }
    }

    void QuitQAPanel()
    {
        gameObject.SetActive(false);
    }
}
