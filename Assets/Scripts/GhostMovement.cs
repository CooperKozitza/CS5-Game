using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [Range(0, 5)]
    public float speed;
    public float floatiness = 1.0f;

    private Rigidbody rigidBody { get; set; }
    private bool onGround = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = new Vector3(Input.GetAxis("Horizontal") * speed, rigidBody.velocity.y, Input.GetAxis("Vertical") * speed);
        if (Input.GetButton("Jump") && onGround)
        {
            rigidBody.AddForce(new Vector3(0, floatiness, 0));
            onGround = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            onGround = true;
        }
    }
}
