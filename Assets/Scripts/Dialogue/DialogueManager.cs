using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [Header("Dialogue Objects")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Text dialogueText;
    [SerializeField] Image leftPortraitImage;
    [SerializeField] Image rightPortraitImage;
    [SerializeField] Text speakerNameText;
    [SerializeField] Button nextButton;

    [Header("Scroll Content")]
    [SerializeField] GameObject scrollContentPanel;
    [SerializeField] GameObject scrollContent;
    [SerializeField] GameObject imageContentPrefab;
    [SerializeField] GameObject textContentPrefab;

    [Header("Character Portraits")]
    [SerializeField] List<string> characterNames;
    [SerializeField] List<Sprite> characterPortraits;

    int currentLineIndex = 0;
    DialogueSequence currentDialogueSequence;
    List<GameObject> tmpContentObjects = new List<GameObject>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        dialoguePanel.SetActive(false);
        scrollContentPanel.SetActive(false);
        leftPortraitImage.gameObject.SetActive(false);
        rightPortraitImage.gameObject.SetActive(false);
        nextButton.onClick.AddListener(NextLine);
    }



    public void StartDialogue(DialogueSequence dialogueSequence)
    {
        currentLineIndex = 0;
        currentDialogueSequence = dialogueSequence;
        if (dialogueSequence != null && dialogueSequence.lines.Count > 0)
        {
            dialoguePanel.SetActive(true);
            if (dialogueSequence.numberOfSpeakers == 1)
            {
                leftPortraitImage.gameObject.SetActive(true);
                rightPortraitImage.gameObject.SetActive(false);
                int speakerIndex = -1;
                for (int i = 0; i < characterNames.Count; i++)
                {
                    if (characterNames[i] == dialogueSequence.speakerCharacterNames[0])
                    {
                        speakerIndex = i;
                        break;
                    }
                }
                if (speakerIndex >= 0 && speakerIndex < characterPortraits.Count)
                {
                    leftPortraitImage.sprite = characterPortraits[speakerIndex];
                }
                else
                {
                    Debug.LogWarning("Speaker index out of range or character portrait not found.");
                }
            }
            else if (dialogueSequence.numberOfSpeakers == 2)
            {
                leftPortraitImage.gameObject.SetActive(true);
                rightPortraitImage.gameObject.SetActive(true);
                int leftSpeakerIndex = -1;
                int rightSpeakerIndex = -1;
                for (int i = 0; i < characterNames.Count; i++)
                {
                    if (characterNames[i] == dialogueSequence.speakerCharacterNames[0])
                    {
                        leftSpeakerIndex = i;
                    }
                    if (characterNames[i] == dialogueSequence.speakerCharacterNames[1])
                    {
                        rightSpeakerIndex = i;
                    }
                }
                if (leftSpeakerIndex >= 0 && leftSpeakerIndex < characterPortraits.Count)
                {
                    leftPortraitImage.sprite = characterPortraits[leftSpeakerIndex];
                }
                else
                {
                    Debug.LogWarning("Left speaker index out of range or character portrait not found.");
                }
                if (rightSpeakerIndex >= 0 && rightSpeakerIndex < characterPortraits.Count)
                {
                    rightPortraitImage.sprite = characterPortraits[rightSpeakerIndex];
                }
                else
                {
                    Debug.LogWarning("Right speaker index out of range or character portrait not found.");
                }
            }

            DisplayDialogueLine(dialogueSequence.lines[0]);
        }
        else
        {
            Debug.LogWarning("No dialogue sequence assigned or empty.");
        }
    }

    private void DisplayDialogueLine(DialogueLine line)
    {
        dialogueText.text = line.speakingContent;
        speakerNameText.text = characterNames[line.speakerIndex];
        // 讓沒有說話的人的肖像變暗
        if (currentDialogueSequence.numberOfSpeakers > 1)
        {
            if (line.speakerIndex == 0)
            {
                rightPortraitImage.color = new Color(1, 1, 1, 0.5f); // Dim the right portrait
                leftPortraitImage.color = new Color(1, 1, 1, 1); // Reset the left portrait
            }
            else if (line.speakerIndex == 1)
            {
                leftPortraitImage.color = new Color(1, 1, 1, 0.5f); // Dim the left portrait
                rightPortraitImage.color = new Color(1, 1, 1, 1); // Reset the right portrait
            }
        }

        //處理額外內容顯示
        if (line.showScrollContent)
        {
            scrollContentPanel.SetActive(true);
            if (line.showImage && line.image != null)
            {
                GameObject imageContent = Instantiate(imageContentPrefab, scrollContent.transform);
                imageContent.GetComponent<Image>().sprite = line.image;
                tmpContentObjects.Add(imageContent);
                //imageContent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset position
            }

            if (line.showText && !string.IsNullOrEmpty(line.textContent))
            {
                GameObject textContent = Instantiate(textContentPrefab, scrollContent.transform);
                textContent.GetComponent<Text>().text = line.textContent;
                tmpContentObjects.Add(textContent);
                //textContent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset position
            }
        }

    }

    void NextLine()
    {
        foreach (GameObject content in tmpContentObjects)
        {
            Destroy(content);
        }
        currentLineIndex++;
        if (currentLineIndex < currentDialogueSequence.lines.Count)
        {
            DisplayDialogueLine(currentDialogueSequence.lines[currentLineIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        scrollContentPanel.SetActive(false);
        currentLineIndex = 0;


        // Trigger interface if specified
        if (currentDialogueSequence != null && currentDialogueSequence.triggerInterfaceWhenEnding)
        {
            // Logic to trigger the interface can be added here
            Debug.Log("Triggering interface after dialogue ends.");
            //TODO: Implement interface triggering logic
        }
        currentDialogueSequence = null;
    }
}
