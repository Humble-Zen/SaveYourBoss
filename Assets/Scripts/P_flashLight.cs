using UnityEngine;

public class P_flashLight : MonoBehaviour,IInteractable
{
    public GameObject lightObject; // Reference to the light GameObject
                                   // Ensure the light is initially inactive
    public GameObject spotLight;
    public bool isFlash = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Interact()
    {
        isFlash = true; // Set the flashlight state to true
        lightObject.SetActive(true); // Toggle the light's active state
        spotLight.SetActive(true); // Toggle the spotlight's active state
        Destroy(gameObject); // Remove the pickup from the scene
    }
}
