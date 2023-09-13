using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public float ballRadius = 0.11f; // Radius of the ball (adjust as needed)
    public float mass = 0.45f; // Mass of the ball (adjust as needed)
    public float airDensity = 1.225f; // Air density (adjust as needed)
    public float dragCoefficient = 0.47f; // Drag coefficient (for a sphere)
    public float spinFactor = 0.1f; // Spin factor (adjust as needed)
    public float restitution = 0.8f; // Coefficient of restitution (bounciness)

    private Rigidbody rb;
    private Vector3 prevVelocity;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 50f; // Adjust this as needed

        // Set the mass of the ball
        rb.mass = mass;
    }

    void FixedUpdate()
    {
        // Calculate air resistance
        Vector3 velocity = rb.velocity;
        float speed = velocity.magnitude;
        Vector3 dragForce = -0.5f * airDensity * speed * speed * dragCoefficient * Mathf.PI * ballRadius * ballRadius * velocity.normalized;
        rb.AddForce(dragForce);

        // Apply spin effect
        Vector3 relativeVelocity = rb.velocity - prevVelocity;
        Vector3 spinTorque = Vector3.Cross(relativeVelocity, rb.angularVelocity) * spinFactor;
        rb.AddTorque(spinTorque);

        // Store the current velocity for the next frame
        prevVelocity = rb.velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Simulate bouncing on collision
        rb.velocity *= restitution;
    }
}
