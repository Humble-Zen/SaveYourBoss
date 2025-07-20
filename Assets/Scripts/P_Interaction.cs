using UnityEngine;

public class P_Interaction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float interactionDistance = 5f;
    public LayerMask interActable;
    private IInteractable currentInteractable;
    public Camera playerCam;
    Vector3 screenCenter;
    void Start()
    {
        screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        playerInteraction();
        executeInteraction();
    }

    public void playerInteraction()
    {
        Ray ray = playerCam.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interActable))
        {
            currentInteractable = hit.collider.GetComponent<IInteractable>();
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                // Enable glow (emission)
                rend.material.EnableKeyword("_EMISSION");
                rend.material.SetColor("_EmissionColor", Color.green * 2f); // Glow intensity
            }
        }
        else
        {
            // If previously highlighted object exists, remove glow
            if (currentInteractable != null)
            {
                Renderer prevRend = (currentInteractable as MonoBehaviour).GetComponent<Renderer>();
                if (prevRend != null)
                {
                    prevRend.material.SetColor("_EmissionColor", Color.black);
                    prevRend.material.DisableKeyword("_EMISSION");
                }
            }

            currentInteractable = null;
        }
    }
    public void executeInteraction()
    {
        if ((currentInteractable != null && Input.GetKeyDown(KeyCode.E)))
        {
            currentInteractable.Interact();
            currentInteractable = null; // Reset after interaction
        }

    }
}

