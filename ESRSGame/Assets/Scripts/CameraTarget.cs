using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using CodeMonkey.Utils;

public class CameraTarget : MonoBehaviour {

    public enum Axis {
        XZ,
        XY,
    }

    
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private Vector2Int zoomLimits;
    [SerializeField] private Vector2Int xRotLimits;
    [SerializeField] private float zoomSpeed; 
    [SerializeField] private Axis axis = Axis.XZ;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float rotSpeed = 1f;
    private Transform _camTransform;


    private void Start()
    {
        try
        {
            _camTransform = vCam.GetComponent<Transform>();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            throw;
        }
        
    }


    private void FixedUpdate() {
        CameraMovement();
        CameraScrolling();
        CameraRotation();
    }

    private void CameraScrolling()
    {
        float scrollVal = -Input.GetAxis("Mouse ScrollWheel");
        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView,Mathf.Clamp(vCam.m_Lens.FieldOfView +  scrollVal * zoomSpeed, zoomLimits.x, zoomLimits.y),0.1f);

    }

    private void CameraRotation()
    {
        if (Input.GetButton("Fire3"))
        {
            float rotx = -Input.GetAxis("Mouse Y");
            float rotY = Input.GetAxis("Mouse X");
            {
                Vector3 currentRot = _camTransform.rotation.eulerAngles;
                _camTransform.eulerAngles = (new Vector3(Mathf.Clamp(currentRot.x +rotx*rotSpeed,xRotLimits.x,xRotLimits.y),currentRot.y +rotSpeed  * rotY,currentRot.z));
            }
            

            {
                Vector3 currentRot = _camTransform.rotation.eulerAngles;
                _camTransform.eulerAngles = (new Vector3(currentRot.x,currentRot.y +rotSpeed  * rotY,currentRot.z));
            }
        }
    }


    private void CameraMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        moveX = Input.GetAxis("Horizontal_AD");
        moveY = Input.GetAxis("Vertical_WS");

        Vector3 moveDir;

        switch (axis) {
            default:
            case Axis.XZ:
                moveDir = new Vector3(moveX, 0, moveY).normalized;
                break;
            case Axis.XY:
                moveDir = new Vector3(moveX, moveY);
                break;
        }
        
        if (moveX != 0 || moveY != 0) {
            // Not idle
        }

        if (axis == Axis.XZ) {
            moveDir = CmUtilsClass.ApplyRotationToVectorXZ(moveDir, _camTransform.rotation.eulerAngles.y );
        }

        transform.position += moveDir * moveSpeed;
    }

}