using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour
{
    public enum StatoRobot { Pattuglia, Inseguimento, Combattimento, Morto }

    [Header("Stato attuale (debug)")]
    public StatoRobot stato = StatoRobot.Pattuglia;

    [Header("Vita")]
    public int hp = 5;

    [Header("Pattuglia Area")]
    public float raggioPattugliamento = 10f;
    public bool usaPosizioneInizialeComeCentro = true;
    public Vector3 centroManuale;
    public float velocitaPattuglia = 2f;
    public float attesaDestinazione = 1.5f;

    private Vector3 _centroPattugliamento;
    private Vector3 _destinazioneAttuale;

    [Header("Rilevamento Player")]
    public float raggioVista = 12f;
    public float angoloVista = 90f;
    public LayerMask layerPlayer;
    public LayerMask layerOstacoli;

    [Header("Combattimento")]
    public float distanzaSparo = 10f;
    public float distanzaMinSparo = 2f;
    public float cadenzaSparo = 1.5f;
    public GameObject proiettilePrefab;
    public Transform puntoSparo;
    public float velocitaProiettile = 20f;
    public int dannoProiettile = 1;

    [Header("Morte")]
    public GameObject esplosioneVFX;

    [Header("Riferimenti")]
    public Animator animator;

    private NavMeshAgent agent;
    private Transform player;
    private float timerAttesa = 0f;
    private float timerSparo = 0f;
    private bool morto = false;

    // Nomi parametri Animator (devono corrispondere ai tuoi)
    private static readonly int animSpeed    = Animator.StringToHash("Speed");
    private static readonly int animShoot    = Animator.StringToHash("Shoot");
    private static readonly int animDie      = Animator.StringToHash("Die");
    private static readonly int animReload   = Animator.StringToHash("Reload");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (animator == null) animator = GetComponentInChildren<Animator>();

        _centroPattugliamento = usaPosizioneInizialeComeCentro ? transform.position : centroManuale;
        ScegliNuovaDestinazione();

        agent.speed = velocitaPattuglia;
    }

    void Update()
    {
        if (morto) return;

        switch (stato)
        {
            case StatoRobot.Pattuglia:    AggiornaPattuglia();    break;
            case StatoRobot.Inseguimento: AggiornaInseguimento(); break;
            case StatoRobot.Combattimento:AggiornaCombattimento();break;
        }

        AggiornaAnimator();
    }

    // ──────────────────────────────────────────────
    //  STATI
    // ──────────────────────────────────────────────

    void AggiornaPattuglia()
    {
        if (PlayerInVista())
        {
            CambiStato(StatoRobot.Inseguimento);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timerAttesa -= Time.deltaTime;
            if (timerAttesa <= 0f)
                ScegliNuovaDestinazione();
        }
    }

    void ScegliNuovaDestinazione()
    {
        Vector3 puntoRandom = _centroPattugliamento + (Vector3)(Random.insideUnitCircle * raggioPattugliamento);
        puntoRandom.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(puntoRandom, out hit, raggioPattugliamento, NavMesh.AllAreas))
        {
            _destinazioneAttuale = hit.position;
            agent.SetDestination(_destinazioneAttuale);
        }

        timerAttesa = attesaDestinazione;
    }

    void AggiornaInseguimento()
    {
        if (player == null) return;

        float distanza = Vector3.Distance(transform.position, player.position);

        // Se il player è abbastanza vicino → combatti
        if (distanza <= distanzaSparo)
        {
            CambiStato(StatoRobot.Combattimento);
            return;
        }

        // Se il player è sfuggito alla vista da troppo tempo → torna a pattugliare
        if (!PlayerInVista() && distanza > raggioVista * 1.5f)
        {
            CambiStato(StatoRobot.Pattuglia);
            return;
        }

        agent.SetDestination(player.position);
    }

    void AggiornaCombattimento()
    {
        if (player == null) return;

        float distanza = Vector3.Distance(transform.position, player.position);

        // Ruota verso il player
        Vector3 dir = (player.position - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);

        // Se il player si allontana troppo → insegui di nuovo
        if (distanza > distanzaSparo * 1.3f)
        {
            CambiStato(StatoRobot.Inseguimento);
            return;
        }

        // Mantieni distanza minima
        if (distanza < distanzaMinSparo)
            agent.SetDestination(transform.position - dir.normalized * 2f);
        else
            agent.ResetPath();

        // Spara
        timerSparo -= Time.deltaTime;
        if (timerSparo <= 0f)
        {
            Spara();
            timerSparo = cadenzaSparo;
        }
    }

    // ──────────────────────────────────────────────
    //  SPARO
    // ──────────────────────────────────────────────

    void Spara()
    {
        if (proiettilePrefab == null || puntoSparo == null) return;

        animator?.SetTrigger(animShoot);

        Vector3 direzione = (player.position + Vector3.up * 1f - puntoSparo.position).normalized;
        GameObject bullet = Instantiate(proiettilePrefab, puntoSparo.position, Quaternion.LookRotation(direzione));

        RobotBullet rb = bullet.GetComponent<RobotBullet>();
        if (rb != null) rb.danno = dannoProiettile;

        Rigidbody rigidBody = bullet.GetComponent<Rigidbody>();
        if (rigidBody != null) rigidBody.linearVelocity = direzione * velocitaProiettile;

        Destroy(bullet, 5f);
    }

    // ──────────────────────────────────────────────
    //  DANNO E MORTE
    // ──────────────────────────────────────────────

    public void RiceviDanno(int danno = 1)
    {
        if (morto) return;

        hp -= danno;

        // Se era in pattuglia, ora insegue
        if (stato == StatoRobot.Pattuglia)
            CambiStato(StatoRobot.Inseguimento);

        if (hp <= 0) Muori();
    }

    void Muori()
    {
        morto = true;
        stato = StatoRobot.Morto;

        agent.isStopped = true;
        agent.enabled = false;

        animator?.SetTrigger(animDie);

        if (esplosioneVFX != null)
            Instantiate(esplosioneVFX, transform.position, Quaternion.identity);

        // Disabilita collider ma lascia il corpo nella scena
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Disabilita questo script
        this.enabled = false;
    }

    // ──────────────────────────────────────────────
    //  ATTIVAZIONE / DISATTIVAZIONE (antenna)
    // ──────────────────────────────────────────────

    // Chiamato da TutorialManager quando l'antenna viene distrutta
    public void DisattivaRobot()
    {
        if (morto) return;
        morto = true;
        stato = StatoRobot.Morto;

        agent.isStopped = true;
        agent.enabled = false;

        animator?.SetTrigger(animDie);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        this.enabled = false;
    }

    // ──────────────────────────────────────────────
    //  UTILITÀ
    // ──────────────────────────────────────────────

    bool PlayerInVista()
    {
        if (player == null) return false;

        Vector3 dirPlayer = player.position - transform.position;
        float distanza = dirPlayer.magnitude;

        if (distanza > raggioVista) return false;

        float angolo = Vector3.Angle(transform.forward, dirPlayer);
        if (angolo > angoloVista * 0.5f) return false;

        // Raycast per controllare ostacoli tra robot e player
        if (Physics.Raycast(transform.position + Vector3.up, dirPlayer.normalized, distanza, layerOstacoli))
            return false;

        return true;
    }

    void CambiStato(StatoRobot nuovoStato)
    {
        stato = nuovoStato;

        switch (nuovoStato)
        {
            case StatoRobot.Pattuglia:
                agent.speed = velocitaPattuglia;
                ScegliNuovaDestinazione();
                break;

            case StatoRobot.Inseguimento:
                agent.speed = velocitaPattuglia * 1.6f;
                break;

            case StatoRobot.Combattimento:
                agent.speed = velocitaPattuglia * 0.5f;
                timerSparo = 0.5f; // piccolo ritardo prima del primo sparo
                break;
        }
    }

    void AggiornaAnimator()
    {
        if (animator == null) return;
        animator.SetFloat(animSpeed, agent.enabled ? agent.velocity.magnitude : 0f);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 centroVisuale = usaPosizioneInizialeComeCentro ? transform.position : centroManuale;
        Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
        Gizmos.DrawSphere(centroVisuale, raggioPattugliamento);
        Gizmos.color = new Color(1f, 1f, 0f, 0.6f);
        Gizmos.DrawWireSphere(centroVisuale, raggioPattugliamento);
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, raggioVista);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanzaSparo);
    }
}
