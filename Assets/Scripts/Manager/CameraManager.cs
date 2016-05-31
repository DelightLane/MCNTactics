using UnityEngine;
using System.Collections;
using System;

public class CameraManager : FZ.MonoSingletone<CameraManager>
{
    public float _moveSpeed = 0.2f;

    protected override string CreatedObjectName()
    {
        return "CameraManager";
    }

    void Start()
    {
        transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
    }

    void Update()
    {        
        if (Input.mousePosition.x > Screen.width - 1)
        {
            transform.Translate(Vector3.right * _moveSpeed);
        }
        if (Input.mousePosition.x < 1)
        {
            transform.Translate(-Vector3.right * _moveSpeed);
        }
        if (Input.mousePosition.y > Screen.height - 1)
        {
            transform.position = transform.position + Vector3.forward * _moveSpeed;
        }
        if (Input.mousePosition.y < 1)
        {
            transform.position = transform.position - Vector3.forward * _moveSpeed;
        }
    }
}
