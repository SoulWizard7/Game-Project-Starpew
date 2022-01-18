using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlockEnemy : EnemyBase
{
    public StateMachine StateMachine => GetComponent<StateMachine>();
    
    private float _timer = 2;
    public bool _hasJumped;
    
    public bool isGrounded;
    [SerializeField]private Transform groundCheck;
    public LayerMask groundMask;

    [Header("JumpAttack")] 
    public int jumpsUntilSpin = 4;
    public float timeBetweenJumps = 3f;
    public float jumpHeight;
    public float jumpLenght;

    public float minBlastRadius = 2f;
    public float maxBlastRadius = 7f;
    public float maxBlastVelocity = 5f;

    private ParticleSystem _blastWave;
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Spin = Animator.StringToHash("Spin");
    private static readonly int StopSpin = Animator.StringToHash("StopSpin");
    
    [Header("Spin Attack")]
    public float maxRotateSpeed = 8f;
    public float rotateAcceleration = 1f;
    public bool _canStartSpinningAttack = true;
    public float _spin;
    public float _tempAcc = 0f;

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
    
    protected override void Start()
    {
        base.Start();
        _blastWave = GetComponent<ParticleSystem>();
    }
    
    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, GameSettings.IsGroundedLayerMask);
    }

    public override void Move()
    {
        if (timeBetweenJumps <= 0) return;
            
        if(!_hasJumped)
        {
            if (_timer > 0) return;
            _hasJumped = true;
            StartCoroutine(JumpAndMove(PlayerDirectionNormalized()));
        }
    }
    public override void IdleWander()
    {
        if(!_hasJumped)
        {
            if (_timer > 0) return;
            _hasJumped = true;
            StartCoroutine(JumpAndMove(IdleDirectionNormalized()));
        }
    }
    IEnumerator JumpAndMove(Vector3 normalizedDirection)
    {
        while (!isGrounded) { yield return new WaitForEndOfFrame(); }
        
        _animator.SetTrigger(Jump);
        yield return new WaitForSeconds(0.2f);
        _rb.AddForce((Vector3.up * jumpHeight + normalizedDirection) * jumpLenght, ForceMode.Impulse);
    }

    public override bool AttackStateRequirements()
    {
        if (jumpsUntilSpin <= 0 && _timer > 0) return true;
        return false;
    }

    public override Transform CheckForAggro()
    {
        if (GameSettings.BouncyAggroRadius > DistanceToPlayer()) { return _player.transform; }
        return null;
    }
    public override bool StopAggroRange()
    {
        if (DistanceToPlayer() > GameSettings.BouncyStopAggroRange) { return true; }
        return false;
    }

    public override bool ExitAttackState()
    {
        if (jumpsUntilSpin >= 1)
        {
            _canStartSpinningAttack = true;
            return true;
        }
        return false;
    }
    
    public override void FireWeapon()
    {
        //while (!isGrounded) return;
        
        SpinAttack();
        
        if (_canStartSpinningAttack)
        {
            _canStartSpinningAttack = false;
            _animator.SetTrigger(Spin);
            StartCoroutine(SpinCoroutine());
        }
    }
    
    private void SpinAttack()
    {
        _rb.AddForce(_navigator.PathToPlayerNormalized() * (_tempAcc + 2), ForceMode.Force);
        
        _spin += _tempAcc * Time.deltaTime;
        Transform _body = transform.GetChild(0);
        Quaternion rotation = Quaternion.AngleAxis( _spin * Mathf.Rad2Deg, Vector3.up);
        _body.rotation = rotation;
    }
    /*private void OnDrawGizmos()
    {
        for (int i = 0; i < _navigator._agent.path.corners.Length; i++)
        {
            Gizmos.DrawSphere(_navigator._agent.path.corners[i], .5f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(_navigator._agent.path.corners[1], transform.position);
    }*/
    
    public IEnumerator SpinCoroutine()
    {
        while (_tempAcc < maxRotateSpeed)
        {
            _tempAcc += rotateAcceleration * Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        yield return new WaitForSeconds(5f);
        while (_tempAcc > 0f)
        {
            _tempAcc -= rotateAcceleration * Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        _tempAcc = 0f;
        _spin = 0f;
        _timer = 5f;
        jumpsUntilSpin = 7;
        _animator.SetTrigger(StopSpin);
    }

    private void BlastPlayerUpwards(float lerpValue)
    {
        _pm.gotBumped = true;
        _pm._jumpSmashVelocity = PlayerDirectionNormalized() * lerpValue;
        _pm.Jump(lerpValue/2);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!_hasJumped) return;
        _rb.velocity = Vector3.zero;

        if (other.collider.CompareTag("Player"))
        {
            StartCoroutine(JumpAndMove(PlayerDirectionNormalized()));
        }
        else
        {
            _animator.SetTrigger(Jump);
            _blastWave.Play();
            float distToPlayer = Vector3.Distance(transform.position, _player.position);

            if (_player.GetComponent<PlayerMovement>().isGrounded)
            {
                if (distToPlayer > maxBlastRadius)
                {
                    //do nothing
                }
                else if (distToPlayer < minBlastRadius)
                {
                    // max blast
                    BlastPlayerUpwards(maxBlastVelocity);
                }
                else
                {
                    // calculate blast
                    float blast = Remap(minBlastRadius, maxBlastRadius,  0, maxBlastVelocity, distToPlayer);
                    BlastPlayerUpwards(blast);
                }
            }
            _timer = timeBetweenJumps;
            jumpsUntilSpin--;
            _hasJumped = false;
        }
    }
    
    static float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = Mathf.InverseLerp(iMax, iMin, v);
        return Mathf.Lerp(oMin, oMax, t);
    }
}
