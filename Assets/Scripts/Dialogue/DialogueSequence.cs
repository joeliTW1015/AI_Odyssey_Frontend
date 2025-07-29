using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public int speakerIndex; // Index of the speaker in the dialogue sequence
    [TextArea] public string speakingContent;
    public bool showScrollContent;
    public bool showImage;
    public Sprite image;
    public bool showText;
    [TextArea] public string textContent;
    
}

[System.Serializable]
public class DialogueSequence
{
    public int numberOfSpeakers = 1;
    public List<string> speakerCharacterNames; // Names of the characters speaking in the dialogue
    public List<DialogueLine> lines;
    public bool triggerEventWhenEnding = false; // Whether to trigger the event when the dialogue ends
    public int triggeredEventIndex = 0; // Index of the event to trigger when the dialogue ends
}
