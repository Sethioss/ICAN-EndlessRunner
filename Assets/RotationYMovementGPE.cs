using UnityEngine;

public class RotationYMovementGPE : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Vitesse de rotation modifiable dans l'inspector

    // Update is called once per frame
    void Update()
    {
        // Rotation de l'objet autour de l'axe X
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
