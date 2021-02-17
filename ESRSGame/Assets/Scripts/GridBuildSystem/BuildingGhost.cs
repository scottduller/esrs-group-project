using System;
using SOScripts;
using UnityEngine;

namespace GridBuildSystem
{
    public class BuildingGhost : MonoBehaviour
    {
        private Transform _visual;
        private PlacedObjectTypeSO _placedObjectTypeSo;
        public GridBuildingSystem gridBuildingSystem;

        private void Start()
        {
            RefreshVisual();

            gridBuildingSystem.OnSelectedChanged += Instance_OnSelectedChanged;
        }

        private void Instance_OnSelectedChanged(object sender, EventArgs e)
        {
            RefreshVisual();
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = gridBuildingSystem.GetMouseWorldSnappedPosition();
            targetPosition.y = 0.1f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

            transform.rotation = Quaternion.Lerp(transform.rotation, gridBuildingSystem.GetPlacedObjectRotation(),
                Time.deltaTime * 15f);
        }

        private void RefreshVisual()
        {
            if (_visual != null)
            {
                Destroy(_visual.gameObject);
                _visual = null;
            }

            PlacedObjectTypeSO placedObjectTypeSO = gridBuildingSystem.GetPlacedObjectTypeSO();

            if (placedObjectTypeSO != null)
            {
                _visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
                _visual.parent = transform;
                _visual.localPosition = Vector3.zero;
                _visual.localEulerAngles = Vector3.zero;
                SetLayerRecursive(_visual.gameObject, 11);
            }
        }

        private void SetLayerRecursive(GameObject targetGameObject, int layer)
        {
            targetGameObject.layer = layer;
            foreach (Transform child in targetGameObject.transform) SetLayerRecursive(child.gameObject, layer);
        }
    }
}