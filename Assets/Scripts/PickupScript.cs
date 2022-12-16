using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class PickupScript : MonoBehaviour
{

    public int points = 0;
    public int health;
    public int maxHealth = 10;
    public TextMeshProUGUI text;
    public Scrollbar healthbar;
    public Material startColor;
    public Material selectColor;
    public GameObject cam;
    public GameObject[] pickups;

// Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        text.text = points.ToString();
        healthbar.size = health / maxHealth;
    }
    
    // Update is called once per frame
    void Update()
    {
        CheckPickup();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            //Debug.Log("trigger");
            if (other.GetComponent<AddAttribute>().good)
            {
                //points++;
                ////Debug.Log("You have " + points + " points.");
                //text.text = points.ToString();
                //other.gameObject.SetActive(false);
            } else
            {
                health--;
                healthbar.size = (float)health / (float)maxHealth;
                other.gameObject.SetActive(false);
            }
        }
    }

    private void CheckPickup()
    {
        float length = 4;
        RaycastHit hit;
        Vector3 rayDirection = cam.transform.forward;

        if(Physics.Raycast(transform.position, rayDirection.normalized, out hit, length)) {
            GameObject pickup = hit.transform.gameObject;
            if (pickup.CompareTag("Pickup") && pickup.GetComponent<AddAttribute>().good)
            {
                pickup.GetComponent<AddAttribute>().selected = true;
                hit.transform.GetChild(0).GetComponent<MeshRenderer>().material = selectColor;
                if (Input.GetKey(KeyCode.E)) {
                    points++;
                    //Debug.Log("You have " + points + " points.");
                    text.text = points.ToString();
                    pickup.SetActive(false);
                }
            }
        }
        else
        {
            pickups = GameObject.FindGameObjectsWithTag("Pickup");
            foreach (GameObject pickup in pickups)
            {
                if (pickup.GetComponent<AddAttribute>().good == true)
                {
                    pickup.GetComponent<AddAttribute>().selected = false;
                    pickup.GetComponentInChildren<MeshRenderer>().material = startColor;
                }
            }
        }

        UnityEngine.Debug.DrawRay(transform.position, rayDirection.normalized * length, Color.black);
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
