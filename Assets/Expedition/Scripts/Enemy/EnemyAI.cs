using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Enumeratie van mogelijke toestanden.
    public enum State
    {
        Patrol,
        Chase,
        Attack
    }

    public enum EnemyType
    {
        Melee,
        Ranged
    }

    public EnemyType enemyType = EnemyType.Melee; // Voeg een veld toe om het type vijand aan te geven

    public float detectionRadius = 10f;
    public float detectionAngle = 90f;
    public float patrolSpeed = 1.5f; // Snelheid tijdens patrouilleren
    public float chaseSpeed = 3.5f; // Snelheid tijdens achtervolgen
    private float timeSinceLastSeen = 0f;
    public float lostSightDuration = 2f; // Stel deze in naar wens
    public float attackRange = 2.0f; // Stel dit in naar de gewenste aanvalsafstand
    public float rangedAttackRange = 10.0f; // Bereik voor ranged aanvallen

    private State currentState = State.Patrol; // Begin in Patrol state
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    // Referentie naar je LinearPathBehaviour script
    [SerializeField]
    private LinearPathBehaviour path;

    // Huidige waypoint waar de vijand naartoe beweegt
    private Transform currentWaypoint;

    // Referentie naar het melee wapen script
    public MeleeWeapon meleeWeapon;

    // Referentie naar het ranged wapen script
    public RangedWeapon rangedWeapon;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = patrolSpeed;

        // Begin bij het eerste waypoint
        if (path != null)
        {
            currentWaypoint = path.GetFirstPoint();
        }
        else
        {
            Debug.LogWarning("No path set for patrol in EnemyAI");
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Vind de speler bij de start

        if (!player)
        {
            Debug.LogWarning("Player object not found! Make sure your player is tagged correctly.");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                if (Vector3.Distance(transform.position, player.position) > (enemyType == EnemyType.Melee ? attackRange : rangedAttackRange))
                {
                    Debug.Log("Player escaped attack range.");
                    animator.SetBool("IsAttacking", false);
                    if (enemyType == EnemyType.Melee)
                    {
                        meleeWeapon.EndAttack(); // Stop the attack in the melee weapon script
                    }
                    ChangeState(State.Chase);
                }
                else
                {
                    RotateTowards(player.position); // Continuously rotate towards the player during attack
                    Attack();
                }
                break;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
        DetectPlayer();
        HandleLostPlayer();
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Patrol()
    {
        agent.speed = patrolSpeed; // Zet de snelheid voor patrouilleren

        // Controleer of er een huidig waypoint is en of de vijand in de buurt is
        if (currentWaypoint != null && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Verkrijg het volgende waypoint als het huidige waypoint is bereikt
            currentWaypoint = path.GetNextPoint(currentWaypoint);
            if (currentWaypoint == null)
            {
                Debug.LogError("Failed to get next waypoint.");
                return;
            }
        }

        // Stel de bestemming van de NavMeshAgent in op het huidige waypoint
        if (currentWaypoint != null)
            agent.SetDestination(currentWaypoint.position);

        Debug.Log("Patrolling: Moving towards waypoint");
    }

    // Methode voor de Chase-toestand.
    void Chase()
    {
        if (player != null)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
    }

    // Methode voor de Attack-toestand.
    void Attack()
    {
        agent.speed = 0; // Stop moving when attacking

        // Update rotation towards the player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        animator.SetBool("IsAttacking", true); // Activate the attack animation

        if (enemyType == EnemyType.Melee)
        {
            if (!meleeWeapon.canAttack)
            {
                meleeWeapon.BeginAttack(); // Start the attack in the melee weapon script
            }
        }
        else if (enemyType == EnemyType.Ranged)
        {
            if (!rangedWeapon.isShooting)
            {
                rangedWeapon.BeginShooting(); // Start shooting in the ranged weapon script
            }
        }

        Debug.Log("Attacking Player");
    }

    // Methode om van toestand te wisselen.
    void ChangeState(State newState)
    {
        if (currentState == State.Attack && newState != State.Attack)
        {
            animator.SetBool("IsAttacking", false); // Ensure the animation is stopped when exiting the attack state
            if (enemyType == EnemyType.Melee)
            {
                meleeWeapon.EndAttack(); // Stop the attack in the melee weapon script
            }
            else if (enemyType == EnemyType.Ranged)
            {
                rangedWeapon.EndShooting(); // Stop shooting in the ranged weapon script
            }
        }

        currentState = newState;
        Debug.Log("Changed state to: " + newState);
    }

    void DetectPlayer()
    {
        if (player && Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            Debug.Log("Player within detection radius.");
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            if (angle <= detectionAngle / 2) // Check if within field of view
            {
                Debug.Log("Player within detection angle.");
                RaycastHit hit;
                int layerMask = 1 << LayerMask.NameToLayer("Player"); // Assuming the player is on a layer named "Player"

                if (Physics.Raycast(transform.position, dirToPlayer, out hit, detectionRadius, layerMask))
                {
                    if (hit.transform == player)
                    {
                        Debug.Log("Player detected by raycast.");
                        if (Vector3.Distance(transform.position, player.position) <= (enemyType == EnemyType.Melee ? attackRange : rangedAttackRange))
                        {
                            Debug.Log("Player in attack range - attacking!");
                            ChangeState(State.Attack);
                        }
                        else
                        {
                            Debug.Log("Player detected - starting chase!");
                            ChangeState(State.Chase);
                        }
                    }
                }
            }
        }
    }

    void HandleLostPlayer()
    {
        if (currentState == State.Chase)
        {
            if (player == null || Vector3.Distance(transform.position, player.position) > detectionRadius)
            {
                timeSinceLastSeen += Time.deltaTime;
                if (timeSinceLastSeen >= lostSightDuration)
                {
                    Debug.Log("Lost player, returning to patrol.");
                    ChangeState(State.Patrol);
                    currentWaypoint = path.GetClosestPoint(transform.position); // Begin vanaf dichtstbijzijnde waypoint
                }
            }
            else
            {
                timeSinceLastSeen = 0f; // Reset de timer als de speler weer wordt gezien
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color c = new Color(0.8f, 0, 0, 0.4f);
        UnityEditor.Handles.color = c;

        Vector3 rotatadForward = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, rotatadForward, detectionAngle, detectionRadius);
    }
#endif
}
