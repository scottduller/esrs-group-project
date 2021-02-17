using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsClass
{
    private static Camera _mainCamera;

    public static Vector3 GetMouseWorldPosition()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;

        Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity)) return hit.point;

        return Vector3.negativeInfinity;
    }
}