using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothadder : MonoBehaviour
{
    private Cloth cloth;
    private GameObject ball;
    private PropertyName msphere;
    
    // Start is called before the first frame update
    void Start()
    {
     
        cloth = GetComponent<Cloth>();

    }

    // Update is called once per frame
    void Update()
    {
            
            ball = GameObject.Find("Ball(Clone)");
            var colliders = new ClothSphereColliderPair[1];
            colliders[0] = new ClothSphereColliderPair(ball.GetComponent<SphereCollider>());
            cloth.sphereColliders = colliders;
            
        }
        
}

