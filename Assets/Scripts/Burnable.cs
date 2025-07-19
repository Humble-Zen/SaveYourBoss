using UnityEngine;

public class Burnable : MonoBehaviour, IInteractable
{
    [Header("Managers & Inventory")]
    public Switch_Manager switchManager;
    public PlayerInventory playerInventory;

    [Header("Burn Settings")]
    public bool isFueled = false;
    public GameObject burnEffectPrefab;
    public GameObject fireSoundPrefab;
    public GameObject fuelSoundPrefab;
    public Transform effectSpawnPoint; // Optional: where to spawn fire effects (defaults to this.transform)

    private void Start()
    {
        if (effectSpawnPoint == null)
            effectSpawnPoint = this.transform;
    }

    public void Interact()
    {
        if (!CompareTag("Burnable")) return;

        int weapon = switchManager.currentWeapon;

        if (weapon == 1 && playerInventory.hasFuel)
        {
            FuelObject();
        }
        else if (weapon == 2 && playerInventory.hasMatchstick)
        {
            if (isFueled)
                BurnObject();
            else
                Debug.Log("You need to fuel this object first!");
        }
    }

    private void FuelObject()
    {
        if (isFueled)
        {
            Debug.Log("Object is already fueled.");
            return;
        }

        isFueled = true;
        Debug.Log("Object has been fueled!");

        if (fuelSoundPrefab != null)
            Instantiate(fuelSoundPrefab, effectSpawnPoint.position, Quaternion.identity);
        else
            Debug.LogWarning("Fuel sound prefab is not assigned!");
    }

    private void BurnObject()
    {
        Debug.Log("Object is burning!");

        if (burnEffectPrefab != null)
            Instantiate(burnEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
        else
            Debug.LogWarning("Burn effect prefab is not assigned!");

        if (fireSoundPrefab != null)
            Instantiate(fireSoundPrefab, effectSpawnPoint.position, Quaternion.identity);
        else
            Debug.LogWarning("Fire sound prefab is not assigned!");

        Destroy(gameObject, 5f); // Destroy after 5 seconds
    }
}
