using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target; // Het target dat de camera moet volgen
    public float distance = 6f;
    public float horizontalSensitivity = 6.5f; // Horizontale gevoeligheid voor camera rotatie
    public float verticalSensitivity = 3.5f; // Verticale gevoeligheid voor camera rotatie
    public LayerMask collisionLayers; // Layer(s) om te controleren op collisions

    private float cameraAngleX = 0f;
    private float cameraAngleY = 50f;
    private float currentDistance;

    void Start()
    {
        currentDistance = distance;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            LookAround();
            UpdateCameraPosition();
        }
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity;

        cameraAngleX += mouseX;
        cameraAngleY -= mouseY;
        cameraAngleY = Mathf.Clamp(cameraAngleY, -25f, 60f);
    }

    void UpdateCameraPosition()
    {
        Vector3 direction = new Vector3(0, 0, -currentDistance);
        Quaternion rotation = Quaternion.Euler(cameraAngleY, cameraAngleX, 0);
        Vector3 potentialCameraPosition = target.position + rotation * direction;

        RaycastHit hit;
        if (Physics.Raycast(target.position, potentialCameraPosition - target.position, out hit, distance, collisionLayers))
        {
            currentDistance = hit.distance - 0.3f; // Trek een beetje terug zodat de camera niet tegen het object plakt
        }
        else
        {
            currentDistance = distance; // Geen obstakel, dus stel de afstand terug naar de maximale afstand
        }

        transform.position = target.position + rotation * new Vector3(0, 0, -currentDistance);
        transform.LookAt(target.position);
    }
}
