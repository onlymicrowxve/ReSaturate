using UnityEngine;

public class PickUpandDrop : MonoBehaviour
{
    [Header("Impostazioni")]
    public float pickupRange = 3f;
    
    [Header("Riferimenti")]
    public GameObject hand;
    public Transform playerCamera;

    void Update()
    {
        HandleInteraction();
        //HandleDrop();
    }

    void HandleInteraction()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickupRange))
        {
            if (hit.transform.CompareTag("Door"))
            {
                DoorController door = hit.transform.GetComponent<DoorController>();
                if (door == null) door = hit.transform.GetComponentInParent<DoorController>();
                if (door != null) door.ToggleDoor();
            }
            else if (hand.transform.childCount < 1)
            {
                if (hit.transform.CompareTag("PickUp") || hit.transform.CompareTag("ArmaDaTerra"))
                {
                    PickUpObject(hit.transform);
                }
            }
        }
    }

    void PickUpObject(Transform item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        CambiaLayerRicorsivo(item.gameObject, 2);

        item.position = hand.transform.position;
        item.rotation = hand.transform.rotation;
        item.parent = hand.transform;

        ColorWeapon weaponScript = item.GetComponent<ColorWeapon>();
        if (weaponScript != null)
        {
            weaponScript.isHeld = true;
            weaponScript.fpsCamera = playerCamera.GetComponent<Camera>();
        }
    }
    void CambiaLayerRicorsivo(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            CambiaLayerRicorsivo(child.gameObject, newLayer);
        }
    }

    /*
    void HandleDrop()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (hand.transform.childCount > 0)
            {
                Transform item = hand.transform.GetChild(0);
                ColorWeapon weaponScript = item.GetComponent<ColorWeapon>();
                if (weaponScript != null)
                {
                    weaponScript.isHeld = false;
                }

                item.parent = null;

                // Quando riattiverai il Drop, ricordati di rimettere il layer a 0 (Default)
                // così potrai raccoglierla di nuovo!
                // CambiaLayerRicorsivo(item.gameObject, 0);

                Rigidbody rb = item.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.linearVelocity = playerCamera.forward * 10f;
                }
            }
        }
    }
    */
}