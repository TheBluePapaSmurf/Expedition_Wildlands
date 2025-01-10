using UnityEngine;

namespace BDE.Expedition.PlayerControls
{
    public class AttackHandler : MonoBehaviour
    {
        [Header("Attack Settings")]
        public float normalAttackDamage = 10f;
        public float attackCooldown = 0.5f;

        public float throwAttackDamage = 15f;
        public float throwCooldown = 1.5f;
        public GameObject boomerangPrefab; // Prefab van de boemerang voor Jari
        public GameObject ricoHelmetPrefab; // Prefab van het helm wapen voor Rico
        public Transform throwPoint; // Het punt vanwaar het wapen wordt gegooid
        public Transform ricoHandBone; // Het punt vanaf de hand bone van Rico
        public float throwForce = 10f; // Kracht waarmee de helm wordt gegooid
        public float throwAngle = 45f; // Hoek waaronder de helm wordt gegooid

        private float attackTimer;
        private float throwTimer;
        private InputHandler inputHandler;
        private CharacterManager characterManager;
        private PlayerMovement playerMovement;
        private GameObject currentBoomerang;

        public bool canThrowWeapon = true;

        void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            if (inputHandler == null)
            {
                Debug.LogError("InputHandler component is missing.");
            }

            characterManager = CharacterManager.Instance;
            if (characterManager == null)
            {
                Debug.LogError("CharacterManager instance is missing.");
            }

            playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement component is missing.");
            }
        }

        void Update()
        {
            attackTimer += Time.deltaTime;
            throwTimer += Time.deltaTime;
            HandleAttacks();
        }

        private void HandleAttacks()
        {
            if (inputHandler.AttackInput && attackTimer >= attackCooldown)
            {
                NormalAttack();
                attackTimer = 0f;
            }

            if (inputHandler.ThrowAttackInput && throwTimer >= throwCooldown)
            {
                Animator animator = characterManager.GetCurrentAnimator();
                if (animator != null)
                {
                    // Trigger de gooi-animatie voor het actieve karakter
                    if (characterManager.currentCharacter == CharacterType.Jari && canThrowWeapon)
                    {
                        animator.SetTrigger("Throw");
                        throwTimer = 0f;
                    }
                    else if (characterManager.currentCharacter == CharacterType.Rico)
                    {
                        animator.SetTrigger("Throw");
                        throwTimer = 0f;
                    }
                }
            }
        }

        private void NormalAttack()
        {
            Animator animator = characterManager.GetCurrentAnimator();
            if (animator != null)
            {
                // Trigger the normal attack animation
                animator.SetTrigger("NormalAttack");
                Debug.Log("Performed Normal Attack with damage: " + normalAttackDamage);
            }
            else
            {
                Debug.LogError("Current Animator is missing.");
            }
        }

        // Functie die wordt aangeroepen door het animatie-event
        public void ThrowHelmet()
        {
            if (characterManager.currentCharacter == CharacterType.Rico)
            {
                GameObject weaponPrefab = ricoHelmetPrefab;

                if (weaponPrefab != null && ricoHandBone != null)
                {
                    // Instantiate the weapon prefab at the hand bone position
                    GameObject weapon = Instantiate(weaponPrefab, ricoHandBone.position, ricoHandBone.rotation);
                    Rigidbody rb = weapon.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // Bereken de werprichting met een boog gebaseerd op de speler's voorwaartse richting
                        Vector3 throwDirection = CalculateThrowDirection(playerMovement.GetForwardDirection(), throwAngle);
                        rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
                        Debug.Log("Performed Throw Attack with Rico's helmet.");
                    }
                    else
                    {
                        Debug.LogError("Rico's helmet prefab is missing a Rigidbody component.");
                    }
                }
                else
                {
                    Debug.LogError("Weapon prefab or hand bone is not set.");
                }
            }
        }

        // Functie die wordt aangeroepen door het animatie-event
        public void ThrowBoomerang()
        {
            if (characterManager.currentCharacter == CharacterType.Jari && canThrowWeapon)
            {
                GameObject weaponPrefab = boomerangPrefab;

                if (weaponPrefab != null && throwPoint != null)
                {
                    // Instantiate the weapon prefab at the throw point
                    GameObject weapon = Instantiate(weaponPrefab, throwPoint.position, throwPoint.rotation);
                    if (characterManager.currentCharacter == CharacterType.Jari && canThrowWeapon)
                    {
                        Boomerang boomerangScript = weapon.GetComponent<Boomerang>();
                        if (boomerangScript != null)
                        {
                            // Initialize the boomerang with the player script reference and throw direction
                            boomerangScript.Initialize(this, throwPoint.forward);
                            currentBoomerang = weapon;
                            canThrowWeapon = false;
                            Debug.Log("Performed Throw Attack with boomerang.");
                        }
                        else
                        {
                            Debug.LogError("Boomerang prefab is missing the Boomerang script.");
                        }
                    }
                }
            }
        }

        private Vector3 CalculateThrowDirection(Vector3 forward, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            Vector3 throwDirection = new Vector3(forward.x * Mathf.Cos(radians), Mathf.Sin(radians), forward.z * Mathf.Cos(radians));
            return throwDirection.normalized;
        }
    }
}
