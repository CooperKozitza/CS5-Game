using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPathfinding : MonoBehaviour
{ 
    public float speed;

    private Rigidbody RigidBody { get; set; }

    public Vector3 targetPosition;
    public Vector3 distanceVector;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        targetPosition = new Vector3(0, 0, 0);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Player not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = player.transform.position;
        distanceVector = targetPosition - transform.position;
        RigidBody.AddForce(new Vector3(distanceVector.x, 0, distanceVector.z).normalized * speed);
        if (RigidBody.velocity.magnitude > speed)
        {
            RigidBody.velocity = RigidBody.velocity.normalized * speed;
        }
        //rigidBody.velocity = new Vector3(Input.GetAxis("Horizontal") * speed, rigidBody.velocity.y, Input.GetAxis("Vertical") * speed);
        
    }
}
