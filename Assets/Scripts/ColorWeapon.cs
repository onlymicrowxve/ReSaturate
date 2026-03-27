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

    [Header("Proiettili Colorati")]
    [Tooltip("Trascina qui i 3 prefab dei proiettili con colori diversi")]
    public GameObject[] bulletPrefabs = new GameObject[3];

    public ParticleSystem muzzleFlash;
    public Slider cooldownSlider;

    [Header("Interfaccia Utente (UI)")]
    public TextMeshProUGUI ammoText;

    [Header("Audio")]
    public AudioSource audioSourceArma;
    public AudioClip suonoSparo;

    private int colpiEsplosi = 0;
    private float nextTimeToFire = 0f;
    private bool inCooldown = false;
    private float cooldownTimer = 0f;
    private int lastBulletIndex = -1;

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
        if (audioSourceArma != null && suonoSparo != null)
            audioSourceArma.PlayOneShot(suonoSparo);

        // Calcola direzione tramite raycast
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, 100f)
            ? hit.point
            : ray.GetPoint(100f);

        // Spawna il proiettile — tutto il danno è gestito da PaintBullet
        GameObject bulletPrefab = GetRandomBulletPrefab();
        if (bulletPrefab != null)
        {
            Vector3 shootDirection = (targetPoint - firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = shootDirection * bulletSpeed;
        }

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

    GameObject GetRandomBulletPrefab()
    {
        int count = 0;
        for (int i = 0; i < bulletPrefabs.Length; i++)
            if (bulletPrefabs[i] != null) count++;

        if (count == 0)
        {
            Debug.LogWarning("ColorWeapon: nessun bulletPrefab assegnato!");
            return null;
        }

        if (count == 1)
        {
            for (int i = 0; i < bulletPrefabs.Length; i++)
                if (bulletPrefabs[i] != null) return bulletPrefabs[i];
        }

        int newIndex;
        do { newIndex = Random.Range(0, bulletPrefabs.Length); }
        while (bulletPrefabs[newIndex] == null || newIndex == lastBulletIndex);

        lastBulletIndex = newIndex;
        return bulletPrefabs[newIndex];
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