using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform playerTransform;   // Player's transform
    public GameObject ball;             // The ball GameObject
    public Vector3 positionOffset;      // Offset for attaching the ball
    public float rotationSpeedMultiplier = 2.0f; // Adjust this value for faster rotation
    public float shootPower = 10.0f;
    public float passPower = 5.0f;

    [SerializeField] public bool isLocked = false;      // Flag to track whether the ball is locked
    private Rigidbody ballRigidbody;    // Reference to the ball's rigidbody component
    private Vector3 initialOffset;      // Initial offset between player and ball

    private float pressTime;
    private bool buttonPressed;
    private float holdTime;


    private bool isShooting = false;
    private bool isPassing = false;
    private bool isLobbing = false;
    bool canShoot;
    bool canPass;
    bool canLob;

    string currentKick;


    private void Start()
    {
        initialOffset = ball.transform.position - playerTransform.position;
        ballRigidbody = ball.GetComponent<Rigidbody>();
    }

    private void Update()
    {   
        if (isLocked)
        {
            SetPower();
            //Debug.Log(holdTime +" : "+ canShoot );

            if (holdTime == 1 && canShoot)
            {
                isShooting = true;
            }

            if (holdTime == 1 && canPass)
            {
                isPassing = true;
            }

            if (holdTime == 1 && canLob)
            {
                isLobbing = true;
            }

        }
    }
    private void FixedUpdate()
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
            if (isShooting == true)
            {
                Shoot();
                isShooting = false;
                canShoot = false;
            }

            // Passing
            if (isPassing == true)
            {
                Pass();
                isPassing = false;
                canPass = false;
            }
            if (isLobbing == true)
            {
                Lob();
                isLobbing = false;
                canLob = false;
            }
        }
    }

    public void OnShoot(InputValue value)
    {
        if (value.isPressed)
        {
            canShoot = true;
            currentKick = "shoot";
        }
        else if (canShoot)
        {
            canShoot = false;
            isShooting = true;
        }
    }
    public void OnPass(InputValue value)
    {
        if (value.isPressed)
        {
            canPass = true;
            currentKick = "pass";
        }
        else if (canPass)
        {
            canPass = false;
            isPassing = true;
        }
    }
    public void OnLob(InputValue value)
    {
        if (value.isPressed)
        {
            canLob = true;
            currentKick = "lob";
        }
        else if (canLob)
        {
            canLob = true;
            isLobbing = true;
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

        Vector3 upDirection = Camera.main.transform.up;
        Vector3 shootingDirection = Camera.main.transform.forward;
        UnlockBall();

        Debug.Log(holdTime);
        ballRigidbody.AddForce(shootingDirection * holdTime * 1500);
        ballRigidbody.AddForce(upDirection * holdTime * 1);
        holdTime = 0;
    }

    private void Pass()
    {
        Debug.Log("Pass initiated");

        // Calculate passing direction based on camera's facing direction
        Vector3 passDirection = Camera.main.transform.forward;
        UnlockBall();
        ballRigidbody.AddForce(passDirection * holdTime * 5, ForceMode.Impulse);
        holdTime = 0;

    }
    private void Lob()
    {
        Debug.Log("Log initiated");

        Vector3 passDirection = Camera.main.transform.forward;
        Vector3 upDirection = Camera.main.transform.up;
        UnlockBall();
        ballRigidbody.AddForce(passDirection * holdTime * 5, ForceMode.Impulse);
        ballRigidbody.AddForce(upDirection * holdTime * 10, ForceMode.Impulse);
        holdTime = 0;
    }

    private void UnlockBall()
    {
        isLocked = false;
        ballRigidbody.isKinematic = false;
        ball.transform.SetParent(null);
    }


    private void SetPower()
    {
        if ((canShoot || canPass || canLob) && !buttonPressed && holdTime == 0)
        {

            // Button pressed, record the time
            pressTime = Time.time;
            buttonPressed = true;


        }

        if (buttonPressed)
        {
            // Calculate how long the button has been held
            holdTime = Time.time - pressTime;

            // Limit the maximum hold time to 1 second
            holdTime = Mathf.Clamp(holdTime, 0f, 1f);

            if ((!canShoot && currentKick == "shoot")|| !canPass && currentKick == "pass" || !canLob && currentKick == "lob" || holdTime >= 1f)
            {
                // Button released or hold time exceeded 1 second
                buttonPressed = false;
                currentKick = "";
            }
        }
    }
}
