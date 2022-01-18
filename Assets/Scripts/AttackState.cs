using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{

    private float _attackReadyTimer;
    private EnemyBase _enemyBase;
    
    public AttackState(EnemyBase enemyBase) : base(enemyBase.gameObject)
    {
        _enemyBase = enemyBase;
    }

    public override Type Tick()
    {
        if (_enemyBase.Target == null) return typeof(WanderState);

        if (_enemyBase.ExitAttackState())
        {
            return typeof(ChaseState);
        }
        
        _enemyBase.FireWeapon();

        return null;
    }


}
