using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject player;
    Vector2 rotation = Vector2.zero;
    public float sensitivity = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        rotation.x += Input.GetAxis("Mouse X") * sensitivity;
        rotation.y = rotation.y > 90 ? 90 : rotation.y < -90 ? -90 : rotation.y + Input.GetAxis("Mouse Y") * sensitivity;
        transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up) * Quaternion.AngleAxis(rotation.y, Vector3.left);
        player.transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);
    }
}

