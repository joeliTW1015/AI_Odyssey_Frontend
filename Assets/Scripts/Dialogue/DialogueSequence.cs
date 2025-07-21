using UnityEngine;
using System.Collections.Generic;

public enum EventType
{
    
}

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    public string content;
    
}

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "Dialogue/Sequence")]
public class DialogueSequence : ScriptableObject
{
    public List<DialogueLine> lines;
}
