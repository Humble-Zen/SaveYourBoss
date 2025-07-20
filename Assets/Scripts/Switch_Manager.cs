using UnityEngine;

public class Switch_Manager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public P_Shooting shootingScript;
    public GameObject gunPrefab;
    public GameObject fuel;
    public GameObject matchStick;
    public int currentWeapon;

    public int tomatoCount = 0; // Track total collected tomatoes

    private void Update()
    {
        CheckInput();
        CheckTomatoStatus();
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

    private void CheckTomatoStatus()
    {
        if (tomatoCount >= 5)
        {
            Debug.Log("All 3 tomatoes collected! Trigger the next objective or event.");
            QuestManager.Instance.CompleteObjective("Quest_1","CollectTomatoes"); // Assuming you have a QuestManager to handle quests
            // Add your logic here for what happens next.
        }
    }

    // Call this method when a tomato is collected
    public void CollectTomato()
    {
        tomatoCount++;
        Debug.Log("Tomato collected! Total tomatoes: " + tomatoCount);
    }
}
