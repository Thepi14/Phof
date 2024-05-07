using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GamePlayer;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent => GetComponent<NavMeshAgent>() == null ? gameObject.AddComponent<NavMeshAgent>() : GetComponent<NavMeshAgent>();
    void Start()
    {
        
    }
    void Update()
    {
        agent.destination = player.gameObject.transform.position;
    }
}
