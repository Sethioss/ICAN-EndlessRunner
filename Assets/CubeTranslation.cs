using UnityEngine;

public class CubeTranslation : MonoBehaviour
{
    public float speed = 5.0f; // Vitesse de translation
    public float distance = 5.0f; // Distance de translation
    public bool returnToOrigin = true; // Si le cube doit revenir à son origine après la translation
    public bool startAtDestination = false; // Si le cube commence à son point d'arrivée ou son origine

    private float startPosition;
    private float destinationPosition;
    private bool isMovingForward = true;

    void OnEnable()
    {
        startPosition = transform.position.x;
        destinationPosition = startPosition + distance; // Translation selon l'axe Y local du cube
        if (startAtDestination)
        {
            transform.position=new Vector3(destinationPosition,transform.position.y,transform.position.z);
            //destinationPosition=transform.position.x;
            isMovingForward = false;
        }
    }

    void Update()
    {
        if (isMovingForward)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(destinationPosition,transform.position.y,transform.position.z), speed * Time.deltaTime);
            if (transform.position.x >= destinationPosition)
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
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(startPosition, transform.position.y, transform.position.z), speed * Time.deltaTime);
            if (transform.position.x <= startPosition)
            {
                isMovingForward = true;
            }
        }
    }
}