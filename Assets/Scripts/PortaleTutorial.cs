using UnityEngine;

public class PortaleTutorial : MonoBehaviour
{
    private bool playerVicino = false;

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerVicino = true;
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
            playerVicino = false;
            if (TutorialManager.Instance != null)
                TutorialManager.Instance.MostraUIUscita(false);
        }
    }
}