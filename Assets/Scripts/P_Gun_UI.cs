using TMPro;
using UnityEngine;

public class P_Gun_UI : MonoBehaviour
{
    [Header("References")]
    public P_Shooting shooter;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magWarningText;
    public TextMeshProUGUI reloadingText;

    private int lastBullets = -1;
    private int lastMagazines = -1;
    private bool wasReloading = false;

    void Update()
    {
        // Update ammo text only when it changes
        if (shooter.currentBullets != lastBullets || shooter.magazines != lastMagazines)
        {
            ammoText.text = $"{shooter.currentBullets}/{shooter.magazines}";
            lastBullets = shooter.currentBullets;
            lastMagazines = shooter.magazines;

            // Update mag warning
            magWarningText.text = (lastBullets == 0 && lastMagazines == 0) ? "No Magazines!" : "";
        }

        // Update reloading status
        if (shooter.IsReloading != wasReloading)
        {
            reloadingText.text = shooter.IsReloading ? "Reloading..." : "";
            wasReloading = shooter.IsReloading;
        }
    }
}
