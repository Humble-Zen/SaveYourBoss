using JetBrains.Annotations;
using UnityEngine;

public class P_Matchstick : MonoBehaviour, IInteractable
{
    public GameObject matchstickUI; // Reference to the UI element that indicates matchstick pickup
    public void Interact()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        inventory.hasMatchstick = true;
        Debug.Log("Matchstick picked up!");
        matchstickUI.SetActive(true); // Activate the matchstick UI to indicate the player has picked up a matchstick
        Destroy(gameObject);  // Remove the pickup from the scene
    }
}
