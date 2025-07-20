using UnityEngine;

public class P_Boards : MonoBehaviour,IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource soundClip;

    public void Interact()
    {
        if (soundClip != null)
        {
            soundClip.Play();
        }
    }
    
}

