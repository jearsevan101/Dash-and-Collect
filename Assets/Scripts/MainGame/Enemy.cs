using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Scriptable Object Enemy Data")]
    [SerializeField] private EnemyStatsSO enemyStats;

    private Vector3 targetPos;
    private enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = enemyStats.stoppingDistance;
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
            GameEvents.PlayerDamaged(enemyStats.damage);
        }
    }


    // DETECTION
    #region Detection Player
    void CheckForPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        bool hasLineOfSight = false;

        if (dist <= enemyStats.detectionRange)
        {
            Vector3 eye = transform.position + Vector3.up * 1.5f;
            Vector3 targetEye = player.position + Vector3.up * 1.5f;

            if (!Physics.Linecast(eye, targetEye, enemyStats.obstacleMask))
            {
                hasLineOfSight = true;
            }
        }

        // Chase if has LoS OR within sense range
        if (hasLineOfSight || dist <= enemyStats.senseRange)
        {
            if (currentState != EnemyState.Chase)
            {
                GameManager.Instance.EnemyStartedChase();
                currentState = EnemyState.Chase;
                agent.speed = enemyStats.chaseSpeed;
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
    #endregion
    // PATROL
    #region Patroling Area
    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= enemyStats.stoppingDistance)
        {
            PickNewDestination();
        }
    }

    void PickNewDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * enemyStats.patrolRadius;
            Vector3 candidate = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y,
                transform.position.z + randomCircle.y
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
            {
                agent.speed = enemyStats.patrolSpeed;
                agent.SetDestination(hit.position);
                targetPos = hit.position;
                return;
            }
        }

        agent.SetDestination(transform.position); // fallback
    }
    #endregion
    // CHASE
    #region Chasing Player
    void Chase()
    {
        agent.SetDestination(player.position);
    }
 
    #endregion
}
