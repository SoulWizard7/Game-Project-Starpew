using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    private Animator _animator;
    
    private static readonly int SwordSlash = Animator.StringToHash("SwordSlash");

    private void Awake()
    {
        _animator = sword.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _animator.SetTrigger(SwordSlash);
        }
    }
}
