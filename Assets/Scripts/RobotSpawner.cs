using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Attacca questo script a un GameObject vuoto "SpawnManager" nella scena.
// Puoi avere più spawn point: trascina qui i Transform dei punti di spawn.
public class RobotSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [Tooltip("Trascina qui i Transform dei punti di spawn (GameObject vuoti posizionati nella stanza)")]
    public Transform[] spawnPoints;

    [Header("Prefab Robot")]
    public GameObject robotPrefab;

    [Header("Impostazioni Spawn")]
    [Tooltip("Quanti robot al massimo possono essere vivi contemporaneamente")]
    public int maxRobotContemporanei = 5;
    [Tooltip("Quanti robot totali spawnare in questo turno di spawn")]
    public int robotTotaliDaSpawnare = 10;
    [Tooltip("Tempo tra uno spawn e l'altro (secondi)")]
    public float intervalloSpawn = 3f;

    // Lista dei robot vivi per contarli
    private List<GameObject> robotVivi = new List<GameObject>();
    private int robotSpawnati = 0;
    private bool spawnAttivo = false;
    private Coroutine spawnCoroutine;

    // Chiamato dal BossFightManager per avviare una nuova ondata
    public void AvviaOndata(int totaleRobot, int maxContemporanei, float intervallo)
    {
        robotTotaliDaSpawnare = totaleRobot;
        maxRobotContemporanei = maxContemporanei;
        intervalloSpawn = intervallo;
        robotSpawnati = 0;
        spawnAttivo = true;

        // Pulisci lista da eventuali riferimenti null
        robotVivi.RemoveAll(r => r == null);

        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StoppaSpawn()
    {
        spawnAttivo = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    // Distruggi tutti i robot vivi (usato a fine boss fight)
    public void DistruggiTuttiRobot()
    {
        StoppaSpawn();
        robotVivi.RemoveAll(r => r == null);
        foreach (GameObject robot in robotVivi)
        {
            if (robot != null) Destroy(robot);
        }
        robotVivi.Clear();
    }

    IEnumerator SpawnRoutine()
    {
        while (spawnAttivo && robotSpawnati < robotTotaliDaSpawnare)
        {
            // Pulisci riferimenti null (robot morti)
            robotVivi.RemoveAll(r => r == null);

            if (robotVivi.Count < maxRobotContemporanei)
            {
                SpawnaRobot();
            }

            yield return new WaitForSeconds(intervalloSpawn);
        }

        // Quando ha finito di spawnare, notifica il manager solo quando
        // tutti i robot sono morti
        if (spawnAttivo)
        {
            StartCoroutine(AspettaMorteRobot());
        }
    }

    IEnumerator AspettaMorteRobot()
    {
        // Aspetta finché tutti i robot spawnati sono morti
        while (true)
        {
            robotVivi.RemoveAll(r => r == null);
            if (robotVivi.Count == 0) break;
            yield return new WaitForSeconds(1f);
        }

        // Notifica il BossFightManager che l'ondata è finita
        BossFightManager.Instance?.OnOndataCompletata();
    }

    void SpawnaRobot()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (robotPrefab == null) return;

        // Scegli uno spawn point casuale
        Transform punto = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject nuovoRobot = Instantiate(robotPrefab, punto.position, punto.rotation);

        robotVivi.Add(nuovoRobot);
        robotSpawnati++;
    }

    // Numero robot vivi attualmente (per debug o UI)
    public int GetRobotVivi()
    {
        robotVivi.RemoveAll(r => r == null);
        return robotVivi.Count;
    }
}
