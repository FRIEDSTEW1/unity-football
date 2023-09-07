using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public FirstPersonController controller;
    public GameObject Camera;
    public PlayerController playerController;
    // Start is called before the first frame update
   public void IsLocalPlayer()
    {
        controller.enabled = true;
        playerController.enabled = true;
        Camera.SetActive(true);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
