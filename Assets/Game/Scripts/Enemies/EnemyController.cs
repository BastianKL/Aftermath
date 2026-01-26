using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using RayCastHit = UnityEngine.RaycastHit;

public enum EnemyState
{
    Patrolling,
    Following,
    Attacking
}

public class EnemyController : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Settings")]
    [SerializeField] private float patrolWaitTime = 2;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] private float detectionRange = 5;
    [SerializeField] private float viewAngle = 90;
    [SerializeField] private float losePlayerTime = 3;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float attackDamage = 10f;
    private float lastAttackTime = 0f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private EnemyState _state = EnemyState.Patrolling;
    private int _currentPatrolIndex;
    private bool _isWaiting;
    private float _timeSinceLostPlayer;
    private bool _isAttacking;

    private void Awake()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    _state = EnemyState.Following;
                    _timeSinceLostPlayer = 0;
                }

                break;

            case EnemyState.Following:
                FollowPlayer();
                if (distanceToPlayer <= attackRange && CanSeePlayer())
                {
                    _state = EnemyState.Attacking;
                    _timeSinceLostPlayer = 0;
                }

                if (!CanSeePlayer())
                {
                    _timeSinceLostPlayer += Time.deltaTime;
                    if (_timeSinceLostPlayer >= losePlayerTime)
                    {
                        _state = EnemyState.Patrolling;
                        GoToClosestPatrolPoint();
                        _timeSinceLostPlayer = 0;
                    }
                }
                else
                {
                    _timeSinceLostPlayer = 0;
                }

                break;

            case EnemyState.Attacking:
                Attack();

                // Check if player moved out of attack range
                if (distanceToPlayer > attackRange)
                {
                    _state = EnemyState.Following;
                    _agent.isStopped = false;
                    _agent.updatePosition = true;
                    _isAttacking = false;
                    _timeSinceLostPlayer = 0;
                }

                // Check if we lost sight of the player
                if (!CanSeePlayer())
                {
                    _timeSinceLostPlayer += Time.deltaTime;
                    if (_timeSinceLostPlayer >= losePlayerTime)
                    {
                        _state = EnemyState.Patrolling;
                        _agent.isStopped = false;
                        _agent.updatePosition = true;
                        _isAttacking = false;
                        GoToClosestPatrolPoint();
                        _timeSinceLostPlayer = 0;
                    }
                }
                else
                {
                    _timeSinceLostPlayer = 0;
                }

                break;
        }

        UpdateAnimations();
    }

    private void StartAttack()
    {
        _isAttacking = true;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _agent.updatePosition = false;
        _animator.SetTrigger("Attack");
        Debug.Log("Starting attack animation!");
    }

    private void Attack()
    {
        // Keep the agent completely stopped while in attack state
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _agent.updatePosition = false;

        var direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (!_isAttacking && Time.time - lastAttackTime > attackCooldown)
        {
            StartAttack();
            lastAttackTime = Time.time;
        }
    }

    private void OnAttackAnimationEnd()
    {
        _isAttacking = false;
        Debug.Log("Attack animation ended!");
    }

    // Called from Animation Event - add this to your attack animation
    // Replace the DealDamage method with this:
    private void DealDamage()
    {
        Debug.Log("DealDamage called from animation event!");
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);
        Debug.Log($"Distance to player: {distanceToPlayer}, Attack Range: {attackRange}");

        if (distanceToPlayer <= attackRange)
        {
            // Try to find PlayerHealth on the player or its parent
            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = player.GetComponentInParent<PlayerHealth>();
            }
            if (playerHealth == null)
            {
                playerHealth = player.GetComponentInChildren<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"Enemy dealt {attackDamage} damage to player!");
            }
            else
            {
                Debug.LogError($"PlayerHealth component not found on {player.name} or its children/parent!");
            }
        }
        else
        {
            Debug.Log("Player out of attack range!");
        }
    }

    private void FollowPlayer()
    {
        _agent.SetDestination(player.position);

    }

    private void Patrol()
    {
        if (_isWaiting) return;
        if (!_agent.pathPending && _agent.remainingDistance <= stopAtDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        _agent.isStopped = false;
        GoToNextPatrolPoint();
        _isWaiting = false;
    }

    private void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        _currentPatrolIndex = closestIndex;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void UpdateAnimations()
    {
        var IsWalking = _agent.velocity.sqrMagnitude > 0.1f;
        _animator.SetBool("IsWalking", IsWalking);
    }

    private bool CanSeePlayer()
    {
        bool facing = IsFacingPlayer();
        bool clear = HasClearPathToPlayer();
        Debug.Log($"CanSeePlayer: Facing={facing}, Clear={clear}, Distance={Vector3.Distance(player.position, transform.position)}");
        return facing && clear;
    }

    private bool IsFacingPlayer()
    {
        var dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= viewAngle / 2;
    }

    private bool HasClearPathToPlayer()
    {
        var direction = player.position - transform.position;
        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, direction.magnitude))
        {
            return hit.transform == player;
        }
        return true;
    }
}