using UnityEngine;
using System.Collections;

namespace BDE.Expedition.PlayerControls
{
    public class AttackHandler : MonoBehaviour
    {
        [Header("Attack Settings")]
        public float normalAttackDamage = 10f;
        public float attackCooldown = 0.5f;
        public float throwAttackDamage = 15f;
        public float throwCooldown = 1.5f;

        [Header("Weapon Prefabs")]
        public GameObject boomerangPrefab;
        public GameObject ricoHelmetPrefab;

        [Header("Throw Settings")]
        public Transform throwPoint;
        public Transform ricoHandBone;
        public float throwForce = 10f;
        public float throwAngle = 45f;

        private InputHandler inputHandler;
        private CharacterManager characterManager;
        private PlayerMovement playerMovement;

        private float attackTimer;
        private float throwTimer;
        private GameObject currentBoomerang;

        // Make this field public so other scripts can access it
        [HideInInspector]
        public bool canThrowWeapon = true;

        private bool isAttacking = false;

        // Object pooling for projectiles
        private ObjectPool<GameObject> helmetPool;
        private const int poolSize = 5;

        void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            playerMovement = GetComponent<PlayerMovement>();

            // Initialize object pool for helmets
            if (ricoHelmetPrefab != null)
            {
                helmetPool = new ObjectPool<GameObject>(CreateHelmet, OnGetHelmet, OnReleaseHelmet, poolSize);
            }
        }

        void Start()
        {
            characterManager = CharacterManager.Instance;

            if (inputHandler == null)
                Debug.LogError("InputHandler component is missing.");
            if (characterManager == null)
                Debug.LogError("CharacterManager instance is missing.");
        }

        void Update()
        {
            UpdateTimers();
            HandleAttackInputs();
        }

        void UpdateTimers()
        {
            if (attackTimer > 0)
                attackTimer -= Time.deltaTime;
            if (throwTimer > 0)
                throwTimer -= Time.deltaTime;
        }

        void HandleAttackInputs()
        {
            if (inputHandler.AttackInput && CanPerformNormalAttack())
            {
                PerformNormalAttack();
            }

            if (inputHandler.ThrowAttackInput && CanPerformThrowAttack())
            {
                PerformThrowAttack();
            }
        }

        bool CanPerformNormalAttack()
        {
            return !isAttacking && attackTimer <= 0 && playerMovement.IsGrounded; // Fixed logic - Can only attack when grounded
        }

        bool CanPerformThrowAttack()
        {
            return !isAttacking && throwTimer <= 0 &&
                   (characterManager.currentCharacter == CharacterType.Rico ||
                    (characterManager.currentCharacter == CharacterType.Jari && canThrowWeapon));
        }

        void PerformNormalAttack()
        {
            StartCoroutine(NormalAttackSequence());
        }

        void PerformThrowAttack()
        {
            StartCoroutine(ThrowAttackSequence());
        }

        IEnumerator NormalAttackSequence()
        {
            isAttacking = true;
            attackTimer = attackCooldown;

            var animator = characterManager.GetCurrentAnimator();
            if (animator != null)
            {
                animator.SetTrigger("NormalAttack");

                // Wait for animation to finish
                yield return new WaitForSeconds(0.1f); // Small delay before checking animation state

                while (animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack") &&
                       animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    yield return null;
                }
            }

            isAttacking = false;
        }

        IEnumerator ThrowAttackSequence()
        {
            isAttacking = true;
            throwTimer = throwCooldown;

            var animator = characterManager.GetCurrentAnimator();
            if (animator != null)
            {
                animator.SetTrigger("Throw");

                // Wait for animation event to call weapon throw
                yield return new WaitForSeconds(0.1f);

                while (animator.GetCurrentAnimatorStateInfo(0).IsName("Throwing") &&
                       animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    yield return null;
                }
            }

            isAttacking = false;
        }

        public void ThrowBoomerang()
        {
            if (characterManager.currentCharacter == CharacterType.Jari && canThrowWeapon && boomerangPrefab != null)
            {
                GameObject weapon = Instantiate(boomerangPrefab, throwPoint.position, throwPoint.rotation);
                var boomerangScript = weapon.GetComponent<Boomerang>();

                if (boomerangScript != null)
                {
                    boomerangScript.Initialize(this, throwPoint.forward);
                    currentBoomerang = weapon;
                    canThrowWeapon = false;
                }
            }
        }

        public void ThrowHelmet()
        {
            if (characterManager.currentCharacter == CharacterType.Rico && ricoHandBone != null)
            {
                GameObject weapon = helmetPool.Get();
                weapon.transform.position = ricoHandBone.position;
                weapon.transform.rotation = ricoHandBone.rotation;

                var rb = weapon.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 throwDirection = CalculateThrowDirection(playerMovement.GetForwardDirection(), throwAngle);
                    rb.linearVelocity = Vector3.zero; // Reset velocity (Unity 6 syntax)
                    rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);

                    // Auto-return helmet to pool after a delay
                    StartCoroutine(ReturnHelmetToPool(weapon, 3f));
                }
            }
        }

        Vector3 CalculateThrowDirection(Vector3 forward, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            return new Vector3(
                forward.x * Mathf.Cos(radians),
                Mathf.Sin(radians),
                forward.z * Mathf.Cos(radians)
            ).normalized;
        }

        IEnumerator ReturnHelmetToPool(GameObject helmet, float delay)
        {
            yield return new WaitForSeconds(delay);
            helmetPool.Release(helmet);
        }

        // Object pool methods
        GameObject CreateHelmet()
        {
            return Instantiate(ricoHelmetPrefab);
        }

        void OnGetHelmet(GameObject helmet)
        {
            helmet.SetActive(true);
        }

        void OnReleaseHelmet(GameObject helmet)
        {
            helmet.SetActive(false);
        }

        public void SetCanThrowWeapon(bool canThrow)
        {
            canThrowWeapon = canThrow;
        }

        // Property for cleaner access (optional - you can use this instead of direct field access)
        public bool CanThrowWeapon
        {
            get { return canThrowWeapon; }
            set { canThrowWeapon = value; }
        }
    }

    // Simple object pool implementation
    public class ObjectPool<T>
    {
        private readonly System.Collections.Generic.Queue<T> pool = new System.Collections.Generic.Queue<T>();
        private readonly System.Func<T> createFunc;
        private readonly System.Action<T> onGet;
        private readonly System.Action<T> onRelease;

        public ObjectPool(System.Func<T> createFunc, System.Action<T> onGet, System.Action<T> onRelease, int preloadCount)
        {
            this.createFunc = createFunc;
            this.onGet = onGet;
            this.onRelease = onRelease;

            for (int i = 0; i < preloadCount; i++)
            {
                var item = createFunc();
                onRelease(item);
                pool.Enqueue(item);
            }
        }

        public T Get()
        {
            T item = pool.Count > 0 ? pool.Dequeue() : createFunc();
            onGet(item);
            return item;
        }

        public void Release(T item)
        {
            onRelease(item);
            pool.Enqueue(item);
        }
    }
}
