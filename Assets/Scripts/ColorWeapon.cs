using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorWeapon : MonoBehaviour
{
    [Header("Stato")]
    public bool isHeld = false;

    [Header("Statistiche")]
    public int colpiPrimaCooldown = 20;
    public float fireRate = 0.2f;
    public float bulletSpeed = 40f;
    public float cooldownDurata = 4f;

    [Header("Riferimenti")]
    public Camera fpsCamera;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public ParticleSystem muzzleFlash;
    public Slider cooldownSlider; 

    [Header("Interfaccia Utente (UI)")]
    public TextMeshProUGUI ammoText;


    private int colpiEsplosi = 0;
    private float nextTimeToFire = 0f;
    private bool inCooldown = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        UpdateAmmoUI();
        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = cooldownDurata;
            cooldownSlider.value = cooldownDurata;
            cooldownSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isHeld)
        {
            if (ammoText != null) ammoText.gameObject.SetActive(false);
            if (cooldownSlider != null) cooldownSlider.gameObject.SetActive(false);
            return;
        }

        if (ammoText != null) ammoText.gameObject.SetActive(true);

        // Gestione cooldown
        if (inCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownSlider != null)
                cooldownSlider.value = cooldownTimer;

            if (cooldownTimer <= 0f)
            {
                inCooldown = false;
                colpiEsplosi = 0;
                if (cooldownSlider != null) cooldownSlider.gameObject.SetActive(false);
                UpdateAmmoUI();
            }
            return; 
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        colpiEsplosi++;
        UpdateAmmoUI();

        if (muzzleFlash != null) muzzleFlash.Play();

        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;

            NexusAntenna antennaColpita = hit.collider.GetComponent<NexusAntenna>();
            if (antennaColpita != null && antennaColpita.gameObject.activeInHierarchy)
                antennaColpita.RiceviColore(1);
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = shootDirection * bulletSpeed;

        // Dopo X colpi entra in cooldown
        if (colpiEsplosi >= colpiPrimaCooldown)
        {
            inCooldown = true;
            cooldownTimer = cooldownDurata;

            if (cooldownSlider != null)
            {
                cooldownSlider.maxValue = cooldownDurata;
                cooldownSlider.value = cooldownDurata;
                cooldownSlider.gameObject.SetActive(true);
            }
            UpdateAmmoUI();
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            if (inCooldown)
                ammoText.text = "RICARICA..";
            else
                ammoText.text = (colpiPrimaCooldown - colpiEsplosi) + " / " + colpiPrimaCooldown;
        }
    }
}
