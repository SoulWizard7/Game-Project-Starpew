using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    public Transform Target { get; private set; }

    [Header("IdleWander variables")] 
    [SerializeField]protected float wanderRadius;
    protected Vector3 _idlePos;
    private float wanderTimer;
    //public LayerMask wanderLayerMask;
    
    protected Rigidbody _rb;
    protected Transform _player;
    protected EnemyNavigator _navigator;
    protected PlayerMovement _pm;
    protected Animator _animator;
    [Header("Navigator")] 
    [SerializeField]protected GameObject navigatorPrefab;

    public virtual void Awake()
    {
        _player = GameSettings.Player.transform;
        _pm = _player.GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }
    
    protected virtual void Start()
    {
        InstantiateNavigator();
        InitializeStateMachine();
    }
    
    public virtual void InitializeStateMachine(){}
    
    public void ThereIsNoMoreTarget()
    {
        Target = null;
    }
    
    protected void InstantiateNavigator()
    {
        if (navigatorPrefab == null) return;
        
        _navigator = Instantiate(navigatorPrefab, transform.position, Quaternion.identity).GetComponent<EnemyNavigator>();
        _navigator._agent = _navigator.gameObject.GetComponent<NavMeshAgent>();
        _navigator._parent = gameObject.transform;
    }
    
    public Vector3 PlayerDirectionNormalized()
    {
        Vector3 playerPos = new Vector3(_player.position.x, 0, _player.position.z);
        playerPos = (playerPos - transform.position).normalized;
        return playerPos;
    }

    protected float DistanceToPlayer()
    {
        if (Target == null) return 0f;

        return Vector3.Distance(Target.position, transform.position);
    }
    
    public void SetTarget(Transform target)
    {
        Target = target;
    }

    public virtual void FireWeapon() { }
    public virtual void Move() { }
    public virtual void IdleWander() { }
    public virtual bool AttackStateRequirements() { return false; }
    public virtual Transform CheckForAggro() { return null; }
    public virtual bool StopAggroRange() { return false; }
    public virtual bool ExitAttackState() { return false; }
    
    #region Wandering Functions
    protected void Wandering()
    {
        //if (wanderTimer < 3) _idlePos = transform.position;
        
        if (wanderTimer > 0)
        {
            wanderTimer -= Time.deltaTime;
            return;
        }
        //float distance = Vector3.Distance(_idlePos, transform.position);
        //if (distance < idleStopDistance)
        //{
        //    animator.SetBool("isIdleWalk", false);
        //}
        //else
        //{
        //    animator.SetBool("isIdleWalk", true);
        //}
        
        FindPos();
        
        if (_navigator._agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            FindPos();
        }
    }

    protected void FindPos()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        //_navigator._agent.SetDestination(newPos);
        //_idlePos = newPos;
        _idlePos = transform.InverseTransformPoint(newPos);
        wanderTimer = 5;
        //_agent.speed = idleSpeed;
        //animator.SetBool("isIdleWalk", true);
    }

    protected Vector3 IdleDirectionNormalized()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        _idlePos = transform.InverseTransformPoint(newPos);
        //wanderTimer = 5;
        return _idlePos.normalized;
    }

    protected static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
    #endregion



}
