using UnityEngine;
using BDE.Expedition.PlayerControls;

public class Boomerang : MonoBehaviour
{
    public float speed = 30f;
    public float maxRange = 20f; // Maximale afstand voordat de boomerang terugkeert
    public float hoverTime = 2.0f; // Tijd die de boomerang blijft zweven op max afstand
    public Vector3 rotationAxis = Vector3.forward; // Standaard rotatie om de z-as
    public float rotationSpeed = 360f; // Rotatiesnelheid in graden per seconde
    public Transform boomerangMesh; // Assign this in the Inspector
    public float acceleration = 10f; // Hoe snel de boomerang versnelt
    public float rotationAcceleration = 300f; // Hoe snel de rotatie toeneemt
    public int damageAmount = 1; // Schade die de boomerang veroorzaakt

    public bool IsHovering
    {
        get { return isHovering; }
    }

    private Vector3 startPosition;
    private Vector3 throwDirection; // Richting waarin de boemerang wordt gegooid
    private Vector3 playerPosition;
    private bool isReturning = false;
    private bool isHovering = false;
    private float hoverTimer;
    private float currentSpeed = 0f; // Startwaarde van de snelheid
    private float currentRotationSpeed = 0f; // Startwaarde van de rotatiesnelheid
    private GameObject player;
    private AttackHandler attackHandler;

    void Start()
    {
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player"); // Zorg ervoor dat je speler de tag "Player" heeft
    }

    public void Initialize(AttackHandler handler, Vector3 direction)
    {
        attackHandler = handler;
        player = handler.gameObject;
        throwDirection = direction; // Sla de werprichting op
        transform.rotation = Quaternion.LookRotation(throwDirection); // Stel de rotatie in op de werprichting
    }

    void Update()
    {
        // Update rotatiesnelheid
        if (currentRotationSpeed < rotationSpeed)
        {
            currentRotationSpeed += rotationAcceleration * Time.deltaTime;
            currentRotationSpeed = Mathf.Min(currentRotationSpeed, rotationSpeed);
        }

        // Pas rotatie toe
        boomerangMesh.Rotate(rotationAxis, currentRotationSpeed * Time.deltaTime, Space.Self);

        // Hover logic
        if (isHovering)
        {
            hoverTimer -= Time.deltaTime;
            if (hoverTimer <= 0)
            {
                isHovering = false;
                isReturning = true;
                playerPosition = player.transform.position;
            }
        }
        else if (isReturning)
        {
            ReturnToPlayer();
        }
        else
        {
            // Beweging en versnelling
            if (currentSpeed < speed)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, speed);
            }

            // Pas beweging toe in de richting waarin de boemerang is gegooid
            transform.position += throwDirection * currentSpeed * Time.deltaTime;

            // Check of de maximale afstand is bereikt en start hoveren
            if (Vector3.Distance(startPosition, transform.position) >= maxRange && !isHovering)
            {
                isHovering = true;
                hoverTimer = hoverTime;
            }
        }
    }

    void ReturnToPlayer()
    {
        // Richting naar de speler
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

        // Beweeg naar de speler
        transform.position += directionToPlayer * currentSpeed * Time.deltaTime;

        // Vernietig de boomerang als deze dicht genoeg bij de speler is
        if (Vector3.Distance(transform.position, playerPosition) < 1f)
        {
            if (attackHandler != null)
            {
                attackHandler.canThrowWeapon = true;
            }
            Destroy(gameObject);
        }
    }

    // Detecteer botsing met een andere collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }
}
