using UnityEngine;
using System.Collections;

public class P_Shooting : MonoBehaviour
{
    [Header("Gun Settings")]
    public float reloadTime = 1f;
    public int maxBullets = 6;
    public int currentBullets = 6;
    public int magazines = 30;
    public float gunCooldown = 0.5f;
    public bool canShoot = true;
    private bool isReloading = false;


    [Header("References")]
    public Camera playerCamera;
    public AudioSource gunshotSound;
    public AudioSource reloadSound;
    public GameObject bloodEffectPrefab;
    public GameObject flashEffect;
    public GameObject bulletOrigin;
    public GameObject gunModel;

    [Header("Zoom Settings")]
    public float normalFOV = 90f;
    public float zoomedFOV = 75f;
    public float zoomSpeed = 5f;
    private bool isZooming = false;

    [Header("Gun Recoil")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.05f;

    private Vector3 screenCenter;
    private Vector3 originalCamPos;
    private Quaternion originalGunRotation;

    private void Start()
    {
        screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        originalCamPos = playerCamera.transform.localPosition;
        originalGunRotation = gunModel.transform.localRotation;
    }

    private void Update()
    {
        HandleZoom();
        HandleShootingInput();
    }

    private void HandleZoom()
    {
        if (Input.GetMouseButtonDown(1)) isZooming = true;
        if (Input.GetMouseButtonUp(1)) isZooming = false;

        float targetFOV = isZooming ? zoomedFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }

    private void HandleShootingInput()
    {
        // Fixed shooting condition - added check for not reloading
        if (Input.GetMouseButtonDown(0) && canShoot && currentBullets > 0 && !isReloading)
            Shoot();

        // Fixed reload condition - separated manual reload from auto-reload
        if (Input.GetKeyDown(KeyCode.R) && currentBullets < maxBullets && magazines > 0 && !isReloading)
            StartCoroutine(Reload());

        // Auto-reload when out of bullets
        if (currentBullets == 0 && magazines > 0 && !isReloading)
            StartCoroutine(Reload());

        if (magazines <= 0 && currentBullets == 0)
            Debug.Log("No bullets left to reload!");
    }

    private void Shoot()
    {
        // Check again before shooting to prevent race conditions
        if (!canShoot || currentBullets <= 0 || isReloading)
            return;

        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        currentBullets--;

        gunshotSound.Play();
        StartCoroutine(GunRecoil());
        StartCoroutine(FlashEffect());
        StartCoroutine(GunCooldown());

        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            Debug.Log($"Hit: {hit.collider.name} | Bullets left: {currentBullets}");
            StartCoroutine(DrawBulletTrace(bulletOrigin.transform.position, hit.point));

            if (hit.collider.CompareTag("Zombie") && hit.collider.TryGetComponent(out ZombieAI zombie))
            {
                zombie.TakeDamage(30);
                Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        else
        {
            StartCoroutine(DrawBulletTrace(bulletOrigin.transform.position, ray.origin + ray.direction * 100f));
        }
    }

    private IEnumerator Reload()
    {
        // Additional safety checks
        if (isReloading || currentBullets == maxBullets || magazines <= 0)
            yield break;

        isReloading = true;
        canShoot = false;

        Debug.Log("Reloading...");
        reloadSound.Play();

        yield return new WaitForSeconds(reloadTime);

        int bulletsToReload = maxBullets - currentBullets;
        int bulletsUsed = Mathf.Min(bulletsToReload, magazines);

        currentBullets += bulletsUsed;
        magazines -= bulletsUsed;

        Debug.Log($"Reloaded: {currentBullets} bullets | Remaining magazines: {magazines}");

        isReloading = false;
        canShoot = true;
    }

    private IEnumerator GunCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(gunCooldown);

        // Only allow shooting again if not reloading
        if (!isReloading)
            canShoot = true;
    }

    private IEnumerator GunRecoil()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float zRot = Mathf.Lerp(10f, 0f, elapsed / shakeDuration);
            gunModel.transform.localRotation = originalGunRotation * Quaternion.Euler(-zRot, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        gunModel.transform.localRotation = originalGunRotation;
    }

    private IEnumerator FlashEffect()
    {
        flashEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        flashEffect.SetActive(false);
    }

    private IEnumerator DrawBulletTrace(Vector3 start, Vector3 end)
    {
        GameObject trace = new GameObject("BulletTrace");
        LineRenderer lr = trace.AddComponent<LineRenderer>();

        Material traceMat = new Material(Shader.Find("Unlit/Color"));
        traceMat.SetColor("_Color", new Color(2f, 1f, 0.2f, 1f));
        lr.material = traceMat;

        lr.startColor = new Color(2f, 1.5f, 0.2f, 1f);
        lr.endColor = new Color(1.5f, 0.4f, 0.1f, 0.8f);
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.numCapVertices = 8;

        float elapsed = 0f;
        float duration = 0.07f;
        while (elapsed < duration)
        {
            float intensity = Mathf.Lerp(1.5f, 0f, elapsed / duration);
            lr.startColor = new Color(2f * intensity, 1.5f * intensity, 0.2f * intensity);
            lr.endColor = new Color(1.5f * intensity, 0.4f * intensity, 0.1f * intensity);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(trace);
    }

    public bool IsReloading => isReloading;
}