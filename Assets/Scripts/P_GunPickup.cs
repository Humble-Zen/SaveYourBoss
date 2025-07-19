using UnityEngine;

public class P_GunPickup : MonoBehaviour, IInteractable
{
    public GameObject gunUI;
    public void Interact()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        inventory.hasGun = true;
        Debug.Log("Gun picked up!");
        gunUI.SetActive(true); // Activate the gun UI to indicate the player has picked up a gun
        Destroy(gameObject);  // Remove the pickup from the scene
    }
}
