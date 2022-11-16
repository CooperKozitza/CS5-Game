using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{

    int points;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            Debug.Log("trigger");
            if (other.GetComponent<AddAttribute>().good)
            {
                points++;
                Debug.Log("You have " + points + " points.");
                other.gameObject.SetActive(false);
            } else
            {
                Debug.Log("You die!");
                other.gameObject.SetActive(false);
            }
        }
    }
}
