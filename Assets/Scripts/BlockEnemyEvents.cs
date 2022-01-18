using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEnemyEvents : MonoBehaviour
{
    void EventSpinn()
    {
        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        StartCoroutine(GetComponent<BlockEnemy>().SpinCoroutine());
    }
}
