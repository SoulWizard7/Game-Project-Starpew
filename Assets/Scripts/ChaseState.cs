using System;
using UnityEngine;

public class ChaseState : BaseState
{
    private EnemyBase _enemyBase;
    public ChaseState(EnemyBase enemyBase) : base(enemyBase.gameObject)
    {
        _enemyBase = enemyBase;
    }

    public override Type Tick()
    {
        if (_enemyBase.Target == null)
        {
            return typeof(WanderState);
        }

        if (_enemyBase.StopAggroRange()) _enemyBase.ThereIsNoMoreTarget();
        
        _enemyBase.Move();
        
        if (_enemyBase.AttackStateRequirements())
        {
            return typeof(AttackState);
        }

        return null;
    }
}
