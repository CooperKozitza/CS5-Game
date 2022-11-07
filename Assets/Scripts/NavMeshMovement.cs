using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour
{
    public Vector3 goal;

    private GameObject player;

    NavMeshAgent agent;

    public bool seePlayer;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("Player not found");
        }

        goal = transform.position;
        agent.destination = goal;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayDirection = player.transform.position - transform.position;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, rayDirection.normalized, out hit, rayDirection.magnitude))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                goal = hit.transform.position;
                Debug.Log("see player");
                seePlayer = true;
                Debug.DrawRay(transform.position, rayDirection.normalized * hit.distance, Color.yellow);
            }
            else
            {
                if (seePlayer) {
                    //goal = hit.point;
                    seePlayer = false;
                }
                Debug.DrawRay(transform.position, rayDirection.normalized * hit.distance, Color.magenta);
                Debug.DrawRay(transform.position, goal - transform.position, Color.blue);
            }
        }

        if (Vector3.Distance(goal, transform.position) > 0.1f)
        {
            agent.destination = goal;
        }
    }
}
