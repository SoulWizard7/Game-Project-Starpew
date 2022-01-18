using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float force;
    public bool swordHitbox;

    private void OnTriggerStay(Collider other)
    {
        if (swordHitbox)
        {
            if (other.CompareTag("Enemy"))
            {
                Rigidbody rb = other.attachedRigidbody;
                //rb.AddForce(-other.GetComponent<BouncyEnemy>().PlayerDirection() * force, ForceMode.Impulse);
                Vector3 playerDir = -other.GetComponent<EnemyBase>().PlayerDirectionNormalized();

                rb.velocity = new Vector3(playerDir.x, 0.5f, playerDir.z) * force;
            }
        }
    }
}
