using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigator : MonoBehaviour
{
    [NonSerialized] public NavMeshAgent _agent;
    [NonSerialized] public Transform _player;
    [NonSerialized] public Transform _parent;

    private void Awake()
    {
        _player = GameSettings.Player.transform;
    }

    private void Update()
    {
        _agent.SetDestination(_player.position);
        transform.position = new Vector3(_parent.position.x, 0, _parent.position.z);
        
        float XZdist = Vector3.Distance(new Vector3(_parent.position.x, 0, _parent.position.z), 
            new Vector3(_agent.path.corners[0].x, 0, _agent.path.corners[0].z));
        
        if (XZdist > 0.5f)
        {
            _agent.Warp(_parent.position);
        }
        //Debug.Log(XZdist);
    }

    public Vector3 PathToPlayerNormalized()
    {
        if (_agent.pathStatus == NavMeshPathStatus.PathInvalid
            || _agent.pathStatus == NavMeshPathStatus.PathPartial
            || _agent.path.corners.Length <= 1)
        {
            return transform.position;
        }
        return (_agent.path.corners[1] - _agent.path.corners[0]).normalized;
    }
    
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(_agent.pathEndPosition, .5f);
        for (int i = 0; i < _agent.path.corners.Length; i++)
        {
            Gizmos.DrawSphere(_agent.path.corners[i], .5f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(_agent.path.corners[1], transform.position);
    }
    
}
