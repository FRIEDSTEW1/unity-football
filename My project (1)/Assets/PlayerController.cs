using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform playerTransform;   // Player's transform
    public GameObject ball;             // The ball GameObject
    public Vector3 positionOffset;      // Offset for attaching the ball
    public float rotationSpeedMultiplier = 2.0f; // Adjust this value for faster rotation
    public float shootPower = 10.0f;
    public float passPower = 5.0f;

    private bool isLocked = false;      // Flag to track whether the ball is locked
    private Rigidbody ballRigidbody;    // Reference to the ball's rigidbody component
    private Vector3 initialOffset;      // Initial offset between player and ball

    private void Start()
    {
        initialOffset = ball.transform.position - playerTransform.position;
        ballRigidbody = ball.GetComponent<Rigidbody>();
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
            ball.transform.Rotate(rotationAxis, rotationAmount * Mathf.Rad2Deg, Space.World);

            // Shooting
            if (Input.GetButtonDown("Shoot"))
            {
                Shoot();
            }

            // Passing
            if (Input.GetButtonDown("Pass"))
            {
                Pass();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLocked && collision.gameObject == ball)
        {
            LockBall();
        }
    }

    private void LockBall()
    {
        isLocked = true;
        ballRigidbody.isKinematic = true;
        ball.transform.SetParent(playerTransform);
        ball.transform.localPosition = positionOffset; // Apply the offset
    }

    private void Shoot()
    {
        Debug.Log("Shot taken");

        // Calculate shooting direction based on camera's facing direction
        Vector3 shootingDirection = Camera.main.transform.forward;
        ballRigidbody.AddForce(shootingDirection * shootPower, ForceMode.Impulse);

        UnlockBall();
    }

    private void Pass()
    {
        Debug.Log("Pass initiated");

        // Calculate passing direction based on camera's facing direction
        Vector3 passDirection = Camera.main.transform.forward;
        ballRigidbody.AddForce(passDirection * passPower, ForceMode.Impulse);

        UnlockBall();
    }

    private void UnlockBall()
    {
        isLocked = false;
        ballRigidbody.isKinematic = false;
        ball.transform.SetParent(null);
    }
}
