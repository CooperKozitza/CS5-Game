using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [Range(0, 5)]
    public float speed;
    public float floatiness = 1.0f;

    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed));
        if (Input.GetButton("Jump") && transform.position.y < 5)
        {
            rb.AddForce(new Vector3(0, floatiness, 0));
        }
    }
}
