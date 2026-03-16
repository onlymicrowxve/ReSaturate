using UnityEngine;

// Aggiungi questo script al prefab del proiettile del robot
public class RobotBullet : MonoBehaviour
{
    public int danno = 1;

   void OnTriggerEnter(Collider other)
{
    Debug.Log("Proiettile ha colpito: " + other.gameObject.name + " | Tag: " + other.tag);

    if (other.CompareTag("Robot")) return;

    if (other.CompareTag("Player"))
    {
        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null) ph = other.GetComponentInParent<PlayerHealth>();
        if (ph != null) ph.RiceviDanno(danno);
        else Debug.Log("PlayerHealth NON trovato!");
    }

    Destroy(gameObject);
}
}
