using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public string questID;                 // ? This is what was missing
    public string title;
    public string description;
    public List<QuestObjective> objectives;
    public bool isCompleted;
}
