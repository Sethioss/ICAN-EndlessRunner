using UnityEngine;

public class CubeTranslation : MonoBehaviour
{
    public float speed = 1.0f; // Vitesse de translation
    public float distance = 5.0f; // Distance de translation
    public bool returnToOrigin = true; // Si le cube doit revenir à son origine après la translation
    public bool startAtDestination = false; // Si le cube commence à son point d'arrivée ou son origine

    private Vector3 startPosition;
    private Vector3 destinationPosition;
    private bool isMovingForward = true;

    void Start()
    {
        startPosition = transform.position;
        destinationPosition = startPosition + transform.up * distance; // Translation selon l'axe Y local du cube
        if (startAtDestination)
        {
            transform.position = destinationPosition;
            isMovingForward = false;
        }
    }

    void Update()
    {
        if (isMovingForward)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationPosition, speed * Time.deltaTime);
            if (transform.position == destinationPosition)
            {
                if (returnToOrigin)
                {
                    isMovingForward = false;
                }
                else
                {
                    destinationPosition = startPosition;
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            if (transform.position == startPosition)
            {
                isMovingForward = true;
            }
        }
    }
}