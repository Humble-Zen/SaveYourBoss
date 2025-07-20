using UnityEngine;

public class P_Tomato : MonoBehaviour, IInteractable
{
    public AudioSource collectSound;
    public Switch_Manager switchManager; // Assign this in Inspector

    public void Interact()
    {
        if (collectSound != null)
            collectSound.Play();

        switchManager.CollectTomato(); // Add 1 to tomato count

        Destroy(gameObject);
    }
}
