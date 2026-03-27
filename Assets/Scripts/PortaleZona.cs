using UnityEngine;

// Versione generica di PortaleTutorial.
// Mettilo sul cubo invisibile che funge da portale (Is Trigger attivo).
public class PortaleZona : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ZoneManager.Instance != null)
                ZoneManager.Instance.MostraUIUscita(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (ZoneManager.Instance != null)
                ZoneManager.Instance.VaiProssimaScena();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ZoneManager.Instance != null)
                ZoneManager.Instance.MostraUIUscita(false);
        }
    }
}
