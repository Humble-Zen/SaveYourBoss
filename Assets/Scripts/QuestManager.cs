// QuestManager.cs
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Define your quest steps in order")]
    public List<Quests> questSteps = new List<Quests>();

    private int currentStepIndex = 0;

    [Header("UI")]
    public TextMeshProUGUI fetch;
    public TextMeshProUGUI deliver;
    // Called by interactable objects

    private void Start()
    {
        fetch.text = "Get the post from the postoffice";
    }
    public void CheckInteraction(GameObject interactedObject)
    {
        if (currentStepIndex >= questSteps.Count)
        {
            Debug.Log("? All quest steps complete!");
            return;
        }

        Quests currentStep = questSteps[currentStepIndex];
        Quests nextStep = currentStepIndex + 1 < questSteps.Count ? questSteps[currentStepIndex + 1] : null;
        if (interactedObject == currentStep.targetObject)
        {
            currentStep.isCompleted = true;
            Debug.Log($"? Completed Step: {currentStep.stepName}");
            //fetch.text = $"You completed this : {currentStep.stepName}";
            deliver.text = $"Deliver the post to : {nextStep.stepName}";
            currentStepIndex++;

            if (currentStepIndex < questSteps.Count)
            {
                Debug.Log($"?? Next Step: {questSteps[currentStepIndex].stepName}");
            }
            else
            {
                Debug.Log("?? All quests complete!");
            }
        }
        else
        {
            Debug.Log($"? That's not the correct target! Current step: {currentStep.stepName}");
        }
    }

    public Quests GetCurrentStep()
    {
        if (currentStepIndex < questSteps.Count)
        {
            return questSteps[currentStepIndex];
        }
        return null;
    }
}
