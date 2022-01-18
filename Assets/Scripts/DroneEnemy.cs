using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEnemy : EnemyBase
{
    public StateMachine StateMachine => GetComponent<StateMachine>();
    
    private Vector3? _destination;
    private float stopDistance = 1f;
    private float turnSpeed = 1f;
    //private readonly LayerMask _layerMask = LayerMask.NameToLayer("Walls");
    public LayerMask _layerMask;
    private float _rayDistance = 3.5f;
    private Quaternion _desiredRotation;
    private Vector3 _direction;
    

    public override void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(WanderState), new WanderState(this)},
            {typeof(ChaseState), new ChaseState(this)},
            {typeof(AttackState), new AttackState(this)}
        };
        GetComponent<StateMachine>().SetStates(states);
    }
    
    public override void IdleWander()
    {
        if (_destination.HasValue == false || Vector3.Distance(transform.position, _destination.Value) <= stopDistance)
        {
            FindRandomDestination();
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, _desiredRotation, Time.deltaTime * turnSpeed);

        if (IsForwardBlocked())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, 0.2f);
        }
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * GameSettings.DroneSpeed);
        }

        Debug.DrawRay(transform.position, _direction * _rayDistance, Color.red);
        while (IsPathBlocked())
        {
            FindRandomDestination();
        }
        
        bool IsForwardBlocked()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            return Physics.SphereCast(ray, 0.5f, _rayDistance, _layerMask);
        }
        bool IsPathBlocked()
        {
            Ray ray = new Ray(transform.position, _direction);
            return Physics.SphereCast(ray, 0.5f, _rayDistance, _layerMask);
        }

        void FindRandomDestination()
        {
            Vector3 testPosition = (transform.position + (transform.forward * 4)) +
                                   new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0,
                                       UnityEngine.Random.Range(-4.5f, 4.5f));
            _destination = new Vector3(testPosition.x, 1f, testPosition.z);

            _direction = Vector3.Normalize(_destination.Value - transform.position);
            _direction = new Vector3(_direction.x, 0f, _direction.z);
            _desiredRotation = Quaternion.LookRotation(_direction);
            Debug.Log("Got Direction");
        }
    }
    
    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngel = Quaternion.AngleAxis(5, Vector3.up);

    public override Transform CheckForAggro()
    {
        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (int i = 0; i < 24; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, GameSettings.DroneAggroRadius))
            {
                var player = hit.collider.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    return player.transform;
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * GameSettings.DroneAggroRadius, Color.white);
            }

            direction = stepAngel * direction;
        }
        return null;
    }
    
    public override void Move()
    {
        transform.LookAt(_navigator.PathToPlayerNormalized());
        transform.Translate(_navigator.PathToPlayerNormalized() * Time.deltaTime * GameSettings.DroneSpeed);
    }
    
    //var distance = Vector3.Distance(transform.position, _enemyBase.Target.transform.position);
    public override bool AttackStateRequirements()
    {
        if (GameSettings.DroneAttackRange > Vector3.Distance(transform.position, _player.position))
        {
            return true;
        }
        return false;
    } 
    public override bool StopAggroRange()
    {
        if (DistanceToPlayer() > GameSettings.BouncyStopAggroRange) return true;
        return false;
    }

    public override void FireWeapon()
    {
        Debug.Log("FIRE");
    }
}
