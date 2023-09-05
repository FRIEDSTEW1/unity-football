using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BallSync : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private GameObject owningPlayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetOwningPlayer(GameObject player)
    {
        owningPlayer = player;
    }

    public void ClearOwningPlayer()
    {
        owningPlayer = null;
    }

    private void Update()
    {
        Debug.Log(owningPlayer);
        if (photonView.IsMine)
        {
            // Handle player control here using your existing player movement script
            if (owningPlayer != null)
            {
                // If the player owns the ball, set isKinematic = true
                rb.isKinematic = true;
            }
            else
            {
                // If the player doesn't own the ball, set isKinematic = false
                rb.isKinematic = false;
            }
        }
        else
        {
            // Smoothly interpolate the ball's position and rotation
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
        }
    }
    [PunRPC]
    private void UpdateBallPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

  

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send position and rotation data as usual
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);

            // Send owning player info as a boolean
            stream.SendNext(owningPlayer != null);
        }
        else
        {
            // Receive position and rotation data
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            // Receive owning player info and synchronize it
            bool isOwned = (bool)stream.ReceiveNext();
            owningPlayer = isOwned ? photonView.Owner.TagObject as GameObject : null;

            // Update kinematic state
            rb.isKinematic = isOwned;
        }
    }
}
