using UnityEngine;

public class RotateModelToCameraY : MonoBehaviour
{
    public Transform cameraTransform; // Drag ta caméra ici
    public Transform modelTransform;  // Drag ton modèle 3D ici
    public float rotationSpeed = 10f;

    void Update()
    {
        // Prendre la direction horizontale de la caméra
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            Quaternion smoothedRotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Appliquer la rotation seulement sur Y
            modelTransform.rotation = Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f);
        }
    }
}