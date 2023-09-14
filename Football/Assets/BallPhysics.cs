using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public float ballRadius = 0.11f; // Radius of the ball (adjust as needed)
    public float mass = 0.9f; // Increased mass for a heavier feel
    public float airDensity = 1.225f; // Air density (adjust as needed)
    public float dragCoefficient = 0.47f; // Drag coefficient (for a sphere)
    public float spinFactor = 0.03f; // Spin factor (adjust as needed)
    public float restitution = 0.8f; // Coefficient of restitution (bounciness)

    public float curveForce = 10f; // Adjust this to control the curve strength
    public float curveDuration = 1f; // Adjust this to control how long the curve lasts

    public float groundSpinDamping = 0.98f; // Adjust to make the ball slow down faster when grounded

    public Rigidbody rb;
    private Vector3 prevVelocity;
    private Vector3 curveDirection;
    private float curveStartTime;
    private bool isGrounded;

    void Awake()
    {
        

    }

    private void Start()
    {
        rb.maxAngularVelocity = 50f; // Adjust this as needed

        // Set the mass of the ball
        rb.mass = mass;

        // Initialize curveDirection to a default value (e.g., right)
        curveDirection = Vector3.right;
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

        // Check if the curve effect is active
        if (Time.time - curveStartTime < curveDuration)
        {
            // Apply a force to make the ball curve
            rb.AddForce(curveDirection * curveForce);
        }

        // Check if the ball is grounded (you may need to adjust this logic)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, ballRadius + 0.1f);

        // Apply ground spin damping
        if (isGrounded)
        {
            rb.angularVelocity *= groundSpinDamping;
        }

        // Store the current velocity for the next frame
        prevVelocity = rb.velocity;
    }

    // Call this method to activate the curve effect
    public void ActivateCurve(Vector3 direction)
    {
        curveDirection = direction.normalized;
        curveStartTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Simulate bouncing on collision
        rb.velocity *= restitution;
    }

    void Update()
    {
       
    }
}
