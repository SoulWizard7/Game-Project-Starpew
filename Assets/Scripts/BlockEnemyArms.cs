using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEnemyArms : MonoBehaviour
{
    public float smashForce = 10;

    private EnemyBase parent;

    private void Awake()
    {
        parent = transform.parent.GetComponentInParent<EnemyBase>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            StartCoroutine(SmashPlayer(pm));
        }
    }

    IEnumerator SmashPlayer(PlayerMovement pm)
    {
        pm._getSmashedVelocity = parent.PlayerDirectionNormalized() * smashForce;
        yield return new WaitForSeconds(0.3f);
        pm._getSmashedVelocity = Vector3.zero;
    }
}
