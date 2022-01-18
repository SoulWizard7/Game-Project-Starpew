using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BouncyEnemy : EnemyBase
{
    public float bounceForwardForce = 1f;
    public float bounceUpwardForce = 1f;
    public float timeUntilNextBounce = 2.5f;
    private float timer = 0;
    public float smashForce = 10;

    private bool _exitAttackState;
    private bool isGrounded;

    public StateMachine StateMachine => GetComponent<StateMachine>();

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

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public override void IdleWander()
    {
        Wandering();
        _rb.AddForce(_idlePos * GameSettings.BouncyEnemySpeed, ForceMode.Force);
    }

    public override void Move()
    {
        Debug.Log(_rb.velocity.magnitude);
        
        float extraForce = 0;
        if (_rb.velocity.magnitude < 0.8f)
        {
            extraForce = 1.2f;
        }
        else
        {
            extraForce = 0;
        }
        
        _rb.AddForce(_navigator.PathToPlayerNormalized() * (GameSettings.BouncyEnemySpeed + extraForce), ForceMode.Force);

    }
    
    public override bool AttackStateRequirements()
    {
        if (_navigator._agent.path.corners.Length <= 2 && timer <= 0 && GameSettings.BouncyEnemyAttackRange > Vector3.Distance(transform.position, _player.position))
        {
            _exitAttackState = false;
            
            return true;
        }
        return false;
    }

    public override void FireWeapon()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.55f, GameSettings.IsGroundedLayerMask);
        
        if (isGrounded)
        {
            _rb.AddForce((Vector3.up * bounceUpwardForce + PlayerDirectionNormalized()) * bounceForwardForce, ForceMode.Impulse);
            timer = timeUntilNextBounce;
            _exitAttackState = true;
        }
    }

    public override Transform CheckForAggro()
    {
        if (GameSettings.BouncyAggroRadius > Vector3.Distance(transform.position, _player.position)) { return _player.transform; }
        return null;
    }

    public override bool StopAggroRange()
    {
        if (DistanceToPlayer() > GameSettings.BouncyStopAggroRange) { return true; }
        return false;
    }
    
    public override bool ExitAttackState()
    {
        if (_exitAttackState)
        {
            return true;
        }
        return false;
    }
    
/*
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(_agent.pathEndPosition, .5f);
        for (int i = 0; i < _navigator._agent.path.corners.Length; i++)
        {
            Gizmos.DrawSphere(_navigator._agent.path.corners[i], .5f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(_navigator._agent.path.corners[1], transform.position);
    }
*/
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            PlayerMovement pm = other.collider.GetComponent<PlayerMovement>();
            StartCoroutine(SmashPlayer(pm));
        }
    }

    IEnumerator SmashPlayer(PlayerMovement pm)
    {
        pm._getSmashedVelocity = PlayerDirectionNormalized() * smashForce;
        yield return new WaitForSeconds(0.3f);
        pm._getSmashedVelocity = Vector3.zero;
    }

    
    
    
}