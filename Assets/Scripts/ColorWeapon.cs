using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; 

public class ColorWeapon : MonoBehaviour
{
    [Header("Stato")]
    public bool isHeld = false;

    [Header("Statistiche")]
    public int maxAmmo = 20;
    public int currentAmmo;
    public float fireRate = 0.2f;
    public float bulletSpeed = 40f;

    [Header("Riferimenti")]
    public Camera fpsCamera;       
    public Transform firePoint;     
    public GameObject bulletPrefab; 
    public ParticleSystem muzzleFlash;

    [Header("Interfaccia Utente (UI)")]
    public TextMeshProUGUI ammoText;

    private float nextTimeToFire = 0f;

    void Start()
    {
        currentAmmo = maxAmmo;
        if (transform.parent == null) isHeld = false;

        UpdateAmmoUI();
    }

    void Update()
    {
        if (!isHeld)
        {
            ammoText.gameObject.SetActive(false);
            return;
        }
        if(isHeld)
        {
            ammoText.gameObject.SetActive(true);
        }
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; 
        }

        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            if (currentAmmo > 0)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
            }
            else
            {
                //aggiungere un suono magari??
            }
        }
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleFlash != null) muzzleFlash.Play();

        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;

            NexusAntenna antennaColpita = hit.collider.GetComponent<NexusAntenna>();
            
            // --- CONTROLLO POTENZIATO ---
            // Verifica che l'antenna non sia nulla E che sia ancora attiva in gioco
            if (antennaColpita != null && antennaColpita.gameObject.activeInHierarchy)
            {
                antennaColpita.RiceviColore(1);
            }
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection * bulletSpeed;
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo) currentAmmo = maxAmmo;
        
        UpdateAmmoUI();
    }
    
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + maxAmmo;
        }
    }
}