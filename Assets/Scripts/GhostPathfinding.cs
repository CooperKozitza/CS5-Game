using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPathfinding : MonoBehaviour
{
    [Range(0, 5)]
    public float speed;

    private Rigidbody rigidBody { get; set; }

    public Vector3 targetPosition;
    public Vector3 distanceVector;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        targetPosition = new Vector3(0, 0, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceVector = transform.position - targetPosition;
        rigidBody.AddForce(new Vector3(distanceVector.x, 0, distanceVector.y));
        if (rigidBody.velocity.magnitude > 10)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * 10;
        }
        //rigidBody.velocity = new Vector3(Input.GetAxis("Horizontal") * speed, rigidBody.velocity.y, Input.GetAxis("Vertical") * speed);
        
    }
}
