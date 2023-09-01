using UnityEngine;

public class FootballHandler : MonoBehaviour
{
    public Transform playerTransform;   // Player's transform
    public GameObject football;         // The football GameObject
    public Vector3 positionOffset;      // Offset for attaching the ball
    public float rotationSpeedMultiplier = 2.0f; // Adjust this value for faster rotation

    private bool isLocked = false;      // Flag to track whether the football is locked
    private Rigidbody footballRigidbody; // Reference to the football's rigidbody component
    private Vector3 initialOffset;      // Initial offset between player and ball

    private void Start()
    {
        initialOffset = football.transform.position - playerTransform.position;
        footballRigidbody = football.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (!isLocked && collision.gameObject == football)
        {
         
            isLocked = true;
            footballRigidbody.isKinematic = true;
            football.transform.SetParent(playerTransform);
            football.transform.localPosition = positionOffset; // Apply the offset

        }
    }

    private void Update()
    {
        if (isLocked)
        {
            // Calculate the rotation based on the player's movement
            Vector3 playerVelocity = playerTransform.GetComponent<Rigidbody>().velocity;
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, playerVelocity.normalized);
            float rotationAmount = playerVelocity.magnitude * Time.deltaTime * rotationSpeedMultiplier;

            // Apply rotation to the ball
            football.transform.Rotate(rotationAxis, rotationAmount * Mathf.Rad2Deg, Space.World);
        }
    }
}
