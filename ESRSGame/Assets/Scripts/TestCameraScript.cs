using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TestCameraScript : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private float panSpeed = 3f;

    private void Awake()
    {
        mainCamera = Camera.main;
        
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal_LeftRight"), 0, Input.GetAxis("Vertical_DownUp"));

        transform.position += moveVector * panSpeed;
    }
}
