using UnityEngine;

public class Switch_Manager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public P_Shooting shootingScript;
    public GameObject gunPrefab;
    public GameObject fuel;
    public GameObject matchStick;
    public int currentWeapon;

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Q) && playerInventory.hasGun)
        {
            gunPrefab.SetActive(true);
            shootingScript.canShoot = true; // Re-enable shooting
            currentWeapon = 0;
            fuel.SetActive(false);
            matchStick.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.F) && playerInventory.hasFuel)
        {
            shootingScript.canShoot = false;
            gunPrefab.SetActive(false);
            currentWeapon = 1;
            fuel.SetActive(true);
            matchStick.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.G) && playerInventory.hasMatchstick)
        {
            shootingScript.canShoot = false;
            gunPrefab.SetActive(false);
            currentWeapon = 2;
            fuel.SetActive(false);
            matchStick.SetActive(true);
        }
    }
}