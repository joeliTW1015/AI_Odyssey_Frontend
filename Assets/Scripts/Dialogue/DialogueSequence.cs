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
    public bool triggerInterfaceWhenEnding = false; // Whether to trigger the interface when the dialogue ends
}
