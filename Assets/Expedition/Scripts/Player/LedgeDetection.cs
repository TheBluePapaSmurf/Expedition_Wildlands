using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    public Transform ledgeCheck;
    public float ledgeDistance = 0.5f;
    public LayerMask ledgeMask;

    private bool isLedgeDetected = false;

    void Update()
    {
        DetectLedge();
    }

    void DetectLedge()
    {
        RaycastHit hit;
        if (Physics.Raycast(ledgeCheck.position, transform.forward, out hit, ledgeDistance, ledgeMask))
        {
            isLedgeDetected = true;
        }
        else
        {
            isLedgeDetected = false;
        }

        Debug.DrawRay(ledgeCheck.position, transform.forward * ledgeDistance, Color.red);
    }

    public bool IsLedgeDetected()
    {
        return isLedgeDetected;
    }
}
