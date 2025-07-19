using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<QuestData> activeQuests;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartQuest(QuestData quest)
    {
        if (!activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            Debug.Log($"Started Quest: {quest.title}");
        }
    }

    public void CompleteObjective(string questID, string objectiveName)
    {
        var quest = activeQuests.Find(q => q.questID == questID);
        if (quest != null)
        {
            foreach (var obj in quest.objectives)
            {
                if (obj.objectiveName == objectiveName)
                {
                    obj.isComplete = true;
                    Debug.Log($"Completed Objective: {objectiveName}");

                    if (AllObjectivesComplete(quest))
                    {
                        quest.isCompleted = true;
                        Debug.Log($"Quest Completed: {quest.title}");
                        
                    }
                    return;
                }
            }
        }
    }

    private bool AllObjectivesComplete(QuestData quest)
    {
        foreach (var obj in quest.objectives)
            if (!obj.isComplete) return false;
        return true;
    }
}
