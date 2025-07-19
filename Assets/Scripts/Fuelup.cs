using UnityEngine;
using System.Collections;
public class Fuelup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject FuelTank;
    public float shakeDuration = 1f;

    private void Update()
    {
        fueling();
    }
    public void fueling()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FuelShake());
        }
    }
    IEnumerator FuelShake()
    {
        Quaternion originalRotation = FuelTank.transform.localRotation;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float zRotation = Mathf.Lerp(20, 0f, elapsed / shakeDuration);  // 5 degrees recoil

            FuelTank.transform.localRotation = originalRotation * Quaternion.Euler(zRotation, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

    
        FuelTank.transform.localRotation = originalRotation;
    }
}
