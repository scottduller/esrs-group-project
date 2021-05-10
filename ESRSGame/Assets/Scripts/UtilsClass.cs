using System;
using System.Collections;
using System.Collections.Generic;
using SOScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UtilsClass
{
    private static Camera _mainCamera;

    public static Vector3 GetMouseWorldPosition()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;

        Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                return hit.point;
            }
        }

        return Vector3.negativeInfinity;
    }
    
    [Serializable]
    public struct LevelPlaceListObject
    {
        public int index;
        public PlacedObjectTypeSO PlacedObjectTypeSo;
        public bool isFloor;
        public bool isInteractable; 
    }
    
    public struct LevelPlacedSaveData
    {
        public int index;
        public PlacedObjectTypeSO PlacedObjectTypeSo;
        public bool isFloor;
        public bool isInteractable; 
    }

    public static string Vector3ToString(Vector3 v3)
    {
        return v3.x + "," + v3.y + "," + v3.z;
    }
    
    public static string QuaternionToString(Quaternion q)
    {
        return Vector3ToString(q.eulerAngles);
    }
    
}