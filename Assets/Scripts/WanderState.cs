using System;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : BaseState
{
    private EnemyBase _enemyBase;

    public WanderState(EnemyBase enemyBase) : base(enemyBase.gameObject)
    {
        _enemyBase = enemyBase;
    }

    public override Type Tick()
    {
        var chaseTarget = _enemyBase.CheckForAggro();
        
        if (chaseTarget != null)
        {
            _enemyBase.SetTarget(chaseTarget);
            return typeof(ChaseState);
        }
        
        _enemyBase.IdleWander();
        return null;
    }
}
