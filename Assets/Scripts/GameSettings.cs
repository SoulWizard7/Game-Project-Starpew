using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Header("PLAYER SETTINGS")]
    [SerializeField] private GameObject player;
    public static GameObject Player => Instance.player;
    
    [Header("ENEMY SETTINGS")]
    [SerializeField] private LayerMask isGroundedLayerMask;
    public static LayerMask IsGroundedLayerMask => Instance.isGroundedLayerMask;
    
    [Header("DRONE SETTINGS")]
    [SerializeField] private float droneSpeed = 2f;
    public static float DroneSpeed => Instance.droneSpeed;
    
    [SerializeField] private float droneAggroRadius = 4f;
    public static float DroneAggroRadius => Instance.droneAggroRadius;
    
    [SerializeField] private float droneAttackRange = 3f;
    public static float DroneAttackRange => Instance.droneAttackRange;

    [SerializeField] private GameObject droneProjectilePrefab;
    public static GameObject DroneProjectilePrefab => Instance.droneProjectilePrefab;
    
    
    [Header("BOUNCY ENEMY SETTINGS")]
    [SerializeField] private float bouncyEnemySpeed = 2f;
    public static float BouncyEnemySpeed => Instance.bouncyEnemySpeed;
    
    [SerializeField] private float bouncyEnemyAttackRange = 3f;
    public static float BouncyEnemyAttackRange => Instance.bouncyEnemyAttackRange;
    
    [SerializeField] private float bouncyAggroRadius = 4f;
    public static float BouncyAggroRadius => Instance.bouncyAggroRadius;
    
    [SerializeField] private float bouncyStopAggroRange = 3f;
    public static float BouncyStopAggroRange => Instance.bouncyStopAggroRange;
    

    public static GameSettings Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;
    }
}
