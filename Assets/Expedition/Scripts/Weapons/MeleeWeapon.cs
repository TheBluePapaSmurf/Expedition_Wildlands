using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [System.Serializable]
    public class AttackPoint
    {
        public float radius;
        public Vector3 offset;
        public Transform rootTransform;
    }

    public int damage = 1;
    public AttackPoint[] attackPoints = new AttackPoint[0];

    public bool canAttack = false;
    private Vector3[] originAttackPos;
    private bool hasHitPlayer = false; // Flag to track if the player has been hit

    private void FixedUpdate()
    {
        if (canAttack)
        {
            bool[] hitStatus = new bool[attackPoints.Length - 1];

            for (int i = 0; i < attackPoints.Length; i++)
            {
                AttackPoint ap = attackPoints[i];
                Vector3 worldpos = ap.rootTransform.position + ap.rootTransform.TransformVector(ap.offset);
                originAttackPos[i] = worldpos;
            }

            for (int i = 0; i < attackPoints.Length - 1; i++)
            {
                Vector3 start = originAttackPos[i];
                Vector3 end = originAttackPos[i + 1];
                Vector3 direction = (end - start).normalized;
                float distance = Vector3.Distance(start, end);
                Ray ray = new Ray(start, direction);

                // Detect hits between the points
                RaycastHit[] hits = Physics.SphereCastAll(ray, attackPoints[i].radius, distance);

                bool hitPlayer = false;
                foreach (var hit in hits)
                {
                    if (hit.collider.CompareTag("Player") && !hasHitPlayer)
                    {
                        hitPlayer = true;
                        Debug.Log("Player hit!");

                        // Apply damage to the player
                        PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damage);
                            hasHitPlayer = true; // Set the flag to true once the player is hit
                        }
                    }
                }

                hitStatus[i] = hitPlayer;
            }

            // Draw lines based on hit status
            for (int i = 0; i < attackPoints.Length - 1; i++)
            {
                Vector3 start = originAttackPos[i];
                Vector3 end = originAttackPos[i + 1];
                Color lineColor = hitStatus[i] ? Color.green : Color.red;
                Debug.DrawLine(start, end, lineColor, 0.1f);
            }
        }
    }

    public void BeginAttack()
    {
        canAttack = true;
        hasHitPlayer = false; // Reset the flag when the attack begins
        originAttackPos = new Vector3[attackPoints.Length];

        for (int i = 0; i < attackPoints.Length; i++)
        {
            AttackPoint ap = attackPoints[i];
            originAttackPos[i] = ap.rootTransform.position + ap.rootTransform.TransformDirection(ap.offset);
        }
    }

    public void EndAttack()
    {
        canAttack = false;
        hasHitPlayer = false; // Reset the flag when the attack ends
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach (AttackPoint attackPoint in attackPoints)
        {
            if (attackPoint.rootTransform != null)
            {
                Vector3 worldPosition = attackPoint.rootTransform.TransformVector(attackPoint.offset);
                Gizmos.color = new Color(1.0f, 0f, 0f, 1.0f);
                Gizmos.DrawSphere(attackPoint.rootTransform.position + worldPosition, attackPoint.radius);
            }
        }
    }
#endif
}
