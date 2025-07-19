using UnityEngine;

public class P_Door : MonoBehaviour, IInteractable
{
    public bool isOpen = false;
    public float openAngle = 90f;
    public float closeAngle = 180f;
    public float rotationSpeed = 2f;
    public AudioSource doorSound; // Optional sound effect for door interaction
    public AudioSource openDoorSound;
    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }

        Debug.Log("Door is now " + (isOpen ? "open" : "closed"));
    }

    private void OpenDoor()
    {
        Debug.Log("Opening door...");
        targetRotation = Quaternion.Euler(0, openAngle, 0);
        openDoorSound.Play(); // Play the open door sound if assigned
    }

    private void CloseDoor()
    {
        Debug.Log("Closing door...");
        targetRotation = Quaternion.Euler(0, closeAngle, 0);
        doorSound.Play(); // Play the close door sound if assigned
    }
}
