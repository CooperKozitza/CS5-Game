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
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            //Debug.Log("trigger");
            if (other.GetComponent<AddAttribute>().good)
            {
                points++;
                //Debug.Log("You have " + points + " points.");
                text.text = points.ToString();
                other.gameObject.SetActive(false);
            } else
            {
                health--;
                healthbar.size = (float)health / (float)maxHealth;
                other.gameObject.SetActive(false);
            }
        }
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
