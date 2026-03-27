using UnityEngine;

public class PaintBullet : MonoBehaviour
{
    public GameObject paintSplatPrefab;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("PAINTBULLET OnCollisionEnter colpito: " + collision.gameObject.name + " | tag: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("ArmaDaTerra")) return;

        TryDamage(collision.gameObject);

        // Splat solo se il prefab è assegnato
        if (paintSplatPrefab != null)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject splat = Instantiate(paintSplatPrefab, contact.point + contact.normal * 0.01f, Quaternion.LookRotation(contact.normal));
            Destroy(splat, 10f);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PAINTBULLET OnTriggerEnter colpito: " + other.gameObject.name + " | tag: " + other.gameObject.tag);

        if (other.CompareTag("Player") || other.CompareTag("ArmaDaTerra")) return;

        TryDamage(other.gameObject);

        if (paintSplatPrefab != null)
        {
            GameObject splat = Instantiate(paintSplatPrefab, transform.position, Quaternion.identity);
            Destroy(splat, 10f);
        }

        Destroy(gameObject);
    }

    void TryDamage(GameObject target)
    {
        Debug.Log("PAINTBULLET TryDamage su: " + target.name + " | tag: " + target.tag);

        if (target.CompareTag("Robot"))
        {
            RobotAI robot = target.GetComponent<RobotAI>();
            if (robot == null) robot = target.GetComponentInParent<RobotAI>();
            if (robot != null)
            {
                Debug.Log("PAINTBULLET: colpito robot! HP prima: " + robot.hp);
                robot.RiceviDanno(1);
                Debug.Log("PAINTBULLET: HP dopo: " + robot.hp);
                return;
            }
            else
            {
                Debug.LogError("PAINTBULLET: tag Robot trovato ma RobotAI NON trovato su " + target.name);
            }
        }

        NexusAntenna antenna = target.GetComponent<NexusAntenna>();
        if (antenna == null) antenna = target.GetComponentInParent<NexusAntenna>();
        if (antenna != null) { antenna.RiceviColore(1); return; }

        NexusAntenna_Zona antenaZona = target.GetComponent<NexusAntenna_Zona>();
        if (antenaZona == null) antenaZona = target.GetComponentInParent<NexusAntenna_Zona>();
        if (antenaZona != null) { antenaZona.RiceviColore(1); return; }

        NexusAntenna_Zona2 antenna2 = target.GetComponent<NexusAntenna_Zona2>();
        if (antenna2 == null) antenna2 = target.GetComponentInParent<NexusAntenna_Zona2>();
        if (antenna2 != null) { antenna2.RiceviColore(1); return; }

        NexusAntenna_Zona3 antenna3 = target.GetComponent<NexusAntenna_Zona3>();
        if (antenna3 == null) antenna3 = target.GetComponentInParent<NexusAntenna_Zona3>();
        if (antenna3 != null) { antenna3.RiceviColore(1); return; }
    }
}