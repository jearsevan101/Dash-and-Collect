using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int damage = 1;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Chase Settings")]
    [SerializeField] private float detectionRange = 8f;      // how far enemy can see player
    [SerializeField] private float senseRange = 4f;          // "feel" range (no LoS needed)
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private Transform player;

    private Vector3 targetPos;
    private enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    void Start()
    {
        PickNewDestination();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameStopped) return;

        if (player != null)
        {
            CheckForPlayer();
        }

        if (currentState == EnemyState.Patrol)
            Patrol();
        else if (currentState == EnemyState.Chase)
            Chase();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.PlayerDamaged(damage);
        }
    }

    // ================= DETECTION =================
    void CheckForPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        bool hasLineOfSight = false;

        if (dist <= detectionRange)
        {
            Vector3 eye = transform.position + Vector3.up * 1.5f;
            Vector3 targetEye = player.position + Vector3.up * 1.5f;

            if (!Physics.Linecast(eye, targetEye, obstacleMask))
            {
                hasLineOfSight = true;
            }
        }

        // Chase if has LoS OR within sense range
        if (hasLineOfSight || dist <= senseRange)
        {
            if (currentState != EnemyState.Chase)
            {
                GameManager.Instance.EnemyStartedChase();
                currentState = EnemyState.Chase;
                agent.speed = chaseSpeed;
            }
            return;
        }

        // Otherwise return to patrol
        if (currentState == EnemyState.Chase)
        {
            GameManager.Instance.EnemyStoppedChase();
            currentState = EnemyState.Patrol;
            PickNewDestination();
        }
    }

    // ================= PATROL =================
    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            PickNewDestination();
        }
    }

    void PickNewDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y,
                transform.position.z + randomCircle.y
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
            {
                agent.speed = patrolSpeed;
                agent.SetDestination(hit.position);
                targetPos = hit.position;
                return;
            }
        }

        agent.SetDestination(transform.position); // fallback
    }

    // ================= CHASE =================
    void Chase()
    {
        agent.SetDestination(player.position);
    }
}
