// QuestTarget.cs
using UnityEngine;

public class QuestTarget : MonoBehaviour, IInteractable
{
    [Header("Reference to QuestManager in scene")]
    public QuestManager questManager;

    public void Interact()
    {
        if (questManager != null)
        {
            questManager.CheckInteraction(gameObject);
        }
    }
}
