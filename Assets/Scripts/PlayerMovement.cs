using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float maxSpeed;
    public float floatiness = 1.0f;

    private Rigidbody RigidBody { get; set; }
    private bool onGround = false;

    // Start is called before the first frame update
    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RigidBody.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * speed);

        if (Physics.Raycast(transform.position, Vector3.down, 1F))
        {
            onGround = true;
            Debug.DrawRay(transform.position, Vector3.down, Color.yellow);
        }
        else
        {
            onGround = false;
        }

        if (Input.GetButton("Jump") && onGround)
        {
            RigidBody.AddForce(new Vector3(0, floatiness, 0));
        }

        if (RigidBody.velocity.magnitude > maxSpeed)
        {
            RigidBody.velocity = RigidBody.velocity.normalized * maxSpeed;
        }

    }

}

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Floor"))
//        {
//           onGround = true;
//        }
//    }

//    private void OnCollisionExit(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Floor"))
//        {
//            onGround = false;
//        }
//    }
//}
