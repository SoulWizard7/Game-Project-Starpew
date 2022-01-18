using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 _whereCamWantsToBe;
    private Vector3 _currentVelocity = Vector3.zero;
    private GameObject _player;
    public float _smoothDampSpeed = 0.08f;

    public float _zoom;

    private void Awake()
    {
        _player = GameObject.Find("Player");
    }

    void Update()
    {
        CamFollow();
    }

    void CamFollow()
    {
        //_whereCamWantsToBe = Vector3.MoveTowards(transform.position, _player.transform.position + _player.GetComponent<PlayerMovement>().GetMove(), _zoom);

        _whereCamWantsToBe = _player.transform.position + _player.GetComponent<PlayerMovement>().GetMove();

        // WITH SMOOTHDAMP
        transform.position = Vector3.SmoothDamp(transform.position, _whereCamWantsToBe,
            ref _currentVelocity, _smoothDampSpeed);
    }
    
}
