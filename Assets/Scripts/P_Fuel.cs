using UnityEngine;

public class P_Fuel : MonoBehaviour, IInteractable
{
    public GameObject fuelUI; // Reference to the UI element that indicates fuel pickup
    public void Interact()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        inventory.hasFuel = true;
        Debug.Log("Fuel picked up!");
        fuelUI.SetActive(true); // Activate the fuel UI to indicate the player has picked up fuel
        Destroy(gameObject);  // Remove the pickup from the scene
    }
}
