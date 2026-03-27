using UnityEngine;

public class PortaleTutorial : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TutorialManager.Instance != null)
                TutorialManager.Instance.MostraUIUscita(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (TutorialManager.Instance != null)
                TutorialManager.Instance.VaiProssimaScena();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TutorialManager.Instance != null)
                TutorialManager.Instance.MostraUIUscita(false);
        }
    }
}