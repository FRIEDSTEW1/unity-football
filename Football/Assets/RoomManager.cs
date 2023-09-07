using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public GameObject ball;
    [Space]
    public Transform Spawnpoint;
    public Transform BallSpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("test", null, null);
        Debug.Log("In a room");
        
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("In the room");
        GameObject _player = PhotonNetwork.Instantiate(player.name, Spawnpoint.position, Quaternion.identity);
        
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        GameObject _ball = PhotonNetwork.Instantiate(ball.name, BallSpawnPoint.position, Quaternion.identity);
    }
}
