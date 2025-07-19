using UnityEngine;

public class P_Drawer : MonoBehaviour, IInteractable
{
    public bool isOpen = false;
    public Vector3 closedLocalPos;
    public Vector3 openLocalPos;
    public float positionSpeed = 2f;

    public AudioSource openDrawerSound;
    public AudioSource closeDrawerSound;

    private Vector3 targetLocalPos;

    private void Start()
    {
        closedLocalPos = transform.localPosition;
        openLocalPos = closedLocalPos + new Vector3(0,0,0.5f); // Pushes drawer forward 0.5 units
        targetLocalPos = closedLocalPos;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * positionSpeed);
    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            OpenDrawer();
        }
        else
        {
            CloseDrawer();
        }

        Debug.Log("Drawer is now " + (isOpen ? "open" : "closed"));
    }

    private void OpenDrawer()
    {
        Debug.Log("Opening drawer...");
        targetLocalPos = openLocalPos;
        openDrawerSound.Play();
    }

    private void CloseDrawer()
    {
        Debug.Log("Closing drawer...");
        targetLocalPos = closedLocalPos;
        closeDrawerSound.Play();
    }
}
