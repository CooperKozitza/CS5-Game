using System;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

    public enum States
    {
        idle,
        chase,
        patrol,
        hurt,
        hide,
    }

    public States State { get; set; }

    public GameObject player;

    NavMeshMovement NavMeshMovement;

    // Start is called before the first frame update
    void Start()
    {
        State = States.idle;
        player = GameObject.FindGameObjectWithTag("Player");
        NavMeshMovement = player.GetComponent<NavMeshMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        if (State == States.idle)
        {
            Idle();
        } else if (State == States.chase)
        {
            Chase();
        } else if (State == States.patrol)
        {
            Patrol();
        } else if (State == States.hurt)
        {
            Hurt();
        } else if (State == States.hide)
        {
            Hide();
        }
    }

    private void Hide()
    {
        throw new NotImplementedException();
    }

    private void Hurt()
    {
        throw new NotImplementedException();
    }

    private void Patrol()
    {
        throw new NotImplementedException();
    }

    void Idle()
    {
        if (NavMeshMovement.seePlayer)
        {
            State = States.chase;
        } else
        {
            //idle
        }
    }

    void Chase()
    {
        if (NavMeshMovement.onGoal)
        {
            State = States.idle;
        } else
        {
            //chase
        }
    }
}