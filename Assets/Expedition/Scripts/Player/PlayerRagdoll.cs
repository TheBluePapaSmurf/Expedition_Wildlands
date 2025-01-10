using UnityEngine;

public class PlayerRagdoll : MonoBehaviour
{
    public Rigidbody[] ragdollBodies;
    public Collider[] ragdollColliders;
    public Animator animator;
    public Transform characterTransform; // Voeg dit toe

    private static PlayerRagdoll _instance;
    public static PlayerRagdoll Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Disable all ragdoll parts at the start
        SetRagdollState(false);
    }

    public void SetRagdollState(bool state)
    {
        animator.enabled = !state;

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = state;
        }
    }
}
