using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField]
    private GameObject _target;

    void Start()
    {

        _agent = GetComponent<NavMeshAgent>();
        if(_agent == null)
        {
            Debug.LogError("Nav Mesh Agent is NULL.");
        }
        _agent.SetDestination(_target.transform.position);
    }

    void Update()
    {
        
    }
}
