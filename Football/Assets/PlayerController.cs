using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviourPun
{
    public Transform playerTransform;   // Player's transform
    public GameObject ball;             // The ball GameObject
    public Vector3 positionOffset;      // Offset for attaching the ball
    public float rotationSpeedMultiplier = 2.0f; // Adjust this value for faster rotation
    public float shootPower = 1500.0f;
    public float passPower = 5.0f;
    public float lobUp = 20f;
    public float shootUp = 20f;
    public float maxKickForce = 10.0f;
    public float spinFactor = 1.0f;
    
    [SerializeField] public bool isLocked = false;      // Flag to track whether the ball is locked
    private Rigidbody ballRigidbody;// Reference to the ball's rigidbody component
    public Rigidbody playerRigidbody;
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
    bool canCurveLeft;
    bool canCurveRight;
    public BallPhysics bp;

    private PhotonView ballview;
    private PhotonView pView;

    private Animator animator;
    
    string currentKick;
    public float unlockVelocityThreshold = 11.5f; // Adjust this value for the velocity threshold
    public float kickBackDamping = 0.75f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        pView = GetComponent<PhotonView>();
        ball = GameObject.Find("Ball(Clone)");
        ballview = ball.GetComponent<PhotonView>();
        bp = ball.GetComponent<BallPhysics>();
        initialOffset = ball.transform.position - playerTransform.position;
        ballRigidbody = ball.GetComponent<Rigidbody>();
    }

    

    private void Update()
    {
        if(animator != null)
        {
            Debug.Log("Animator Not null", animator);

        }
        else
        {
            Debug.Log("Animator is null");
        }
        if (isLocked)
        {
            if (ball.transform.localPosition != positionOffset)
            {
                ball.transform.localPosition = positionOffset;
            }
            SetPower();

            // Check player velocity
            float playerVelocityMagnitude = playerTransform.GetComponent<Rigidbody>().velocity.magnitude;
            Debug.Log(playerVelocityMagnitude);

            if (playerVelocityMagnitude >= unlockVelocityThreshold)
            {
                // Player is moving at or above the velocity threshold, unlock and kick the ball
                UnlockAndKickBall(playerVelocityMagnitude);
            }
            else
            {
                // Player is not moving fast enough, continue normal behavior
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
    }

    private void UnlockAndKickBall(float playerVelocityMagnitude)
    {
        isLocked = false;
        ballRigidbody.isKinematic = false;
        ball.transform.SetParent(null);
        
       


        // Calculate the kick direction based on player velocity
        Vector3 kickDirection = playerTransform.forward;

        // Calculate a dynamic force multiplier based on player velocity
        float forceMultiplier = Mathf.Clamp(playerVelocityMagnitude / unlockVelocityThreshold, 0f, 1f);

        // Adjust the kick force based on the dynamic multiplier and the max kick force
        float adjustedKickForce = forceMultiplier * maxKickForce;

        // Apply the kick force to the ball
        playerRigidbody.AddForce(-playerTransform.up * adjustedKickForce, ForceMode.Impulse);
        ballRigidbody.AddForce(kickDirection * adjustedKickForce * kickBackDamping, ForceMode.Impulse);
        
 

        // Reset the hold time and button pressed flag
        holdTime = 0;
        buttonPressed = false;
        
        // Reset currentKick to empty
        currentKick = "";
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
    public void OnCurveRight(InputValue value)
    {
        if (value.isPressed)
        {
            canCurveRight = true;
        }
        else
        {
            canCurveRight = false;
        }
    }
    public void OnCurveLeft(InputValue value)
    {
        if (value.isPressed)
        {
            canCurveLeft = true;
        }
        else
        {
            canCurveLeft = false;

        }
    }

            private void OnCollisionEnter(Collision collision)
    {
        if (!isLocked && collision.gameObject == ball)
        {
            playerRigidbody.AddForce(-playerTransform.up * 10, ForceMode.Force);
            LockBall();
        }
    }
    private void LockBall()
    {
        float playerVelocityMagnitude = playerTransform.GetComponent<Rigidbody>().velocity.magnitude;
        if (playerVelocityMagnitude <= unlockVelocityThreshold)
        {
            animator.SetTrigger(Animator.StringToHash("Touching"));
        }
        isLocked = true;
      
        ballRigidbody.isKinematic = true;
        ball.transform.SetParent(playerTransform);
        ball.transform.localPosition = positionOffset;
        // Check if this player is the owner of the ball's PhotonView
        if (photonView.IsMine)
        {
           

            if (ballview != null && ballview.Owner != PhotonNetwork.LocalPlayer)
            {
                ballview.RequestOwnership();
            }

     
        }

        // Apply the offset
    }

   private void Shoot()
{
    Debug.Log("Shot taken");

        Debug.Log("Shoot initiated");

        Vector3 shootDirection = playerTransform.forward;
        Vector3 upDirection = playerTransform.up;
        animator.SetTrigger(Animator.StringToHash("Shooting"));
        UnlockBall();
        ballRigidbody.AddForce(upDirection * holdTime * shootUp, ForceMode.Impulse);
        ballRigidbody.AddForce(shootDirection * holdTime * shootPower, ForceMode.Impulse);
        if (canCurveLeft)
        {
            bp.ActivateCurve(playerTransform.right);
            ballRigidbody.AddTorque(playerTransform.right * spinFactor);
        }
        else if (canCurveRight)
        {
            bp.ActivateCurve(-playerTransform.right);
            ballRigidbody.AddTorque(-playerTransform.right * spinFactor);
        }
        else
        {
            ballRigidbody.AddTorque(playerTransform.up * spinFactor);
        }
        
        holdTime = 0;
    }





    private void Pass()
    {
        Debug.Log("Pass initiated");

        // Calculate passing direction based on camera's facing direction
        Vector3 passDirection = Camera.main.transform.forward;
        animator.SetTrigger(Animator.StringToHash("Passing"));
        UnlockBall();
        ballRigidbody.AddForce(passDirection * holdTime * passPower, ForceMode.Impulse);
        holdTime = 0;

    }
    private void Lob()
    {
        Debug.Log("Log initiated");

        Vector3 passDirection = Camera.main.transform.forward;
        Vector3 upDirection = Camera.main.transform.up;
        animator.SetTrigger(Animator.StringToHash("Lobbing"));
        UnlockBall();
        ballRigidbody.AddForce(passDirection * holdTime * passPower, ForceMode.Impulse);
        ballRigidbody.AddForce(upDirection * holdTime * lobUp, ForceMode.Impulse);
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
