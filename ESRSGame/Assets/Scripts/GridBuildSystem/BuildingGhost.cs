using System;
using System.Runtime.CompilerServices;
using SOScripts;
using UnityEngine;

namespace GridBuildSystem
{
    public class BuildingGhost : MonoBehaviour
    {
        public static BuildingGhost Instance { get; private set; }
        
        private Transform _visual;
        private PlacedObjectTypeSO _placedObjectTypeSo;
        private GridBuildingSystem gridBuildingSystem = null;
        private float GridYOffset;

        private void Awake()
        {
            Instance = this;

        }

        private void Start()
        {
            gridBuildingSystem = LevelBuilderManager.Instance.GetActiveGrid();
            LevelBuilderManager.Instance.OnActiveLayerChange+= InstanceOnOnActiveLayerChange;
            gridBuildingSystem.OnSelectedChanged += Instance_OnSelectedChanged;
            GridYOffset = gridBuildingSystem.GetGridYOffset();
            RefreshVisual();
            
        }

        private void InstanceOnOnActiveLayerChange(object sender, LevelBuilderManager.OnActiveLayerChangeArgs e)
        {
            if(gridBuildingSystem) gridBuildingSystem.OnSelectedChanged -= Instance_OnSelectedChanged;

            RefreshVisual();
            gridBuildingSystem = e.ActiveLayer;
            gridBuildingSystem.OnSelectedChanged += Instance_OnSelectedChanged;
            GridYOffset = gridBuildingSystem.GetGridYOffset(); 
        }


        private void Instance_OnSelectedChanged(object sender, EventArgs e)
        {
            RefreshVisual();
        }

        private void Update()
        {
            if (!gridBuildingSystem) return;
            Vector3 targetPosition = gridBuildingSystem.GetMouseWorldSnappedPositionSingle();
            targetPosition.y = 0.01f + GridYOffset;

            transform.position = targetPosition;

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

            PlacedObjectTypeSO placedObjectTypeSO = gridBuildingSystem.GetPlacedObjectTypeSo();

            if (placedObjectTypeSO != null)
            {
                _visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
                _visual.parent = transform;
                
                _visual.localEulerAngles = Vector3.zero;
                
                if (Input.GetButton("Fire1") && gridBuildingSystem.GetIsDragBuilder())
                {
                    float gridCellSize = gridBuildingSystem.GetCellSize();
                    Vector2 sizeXZ = gridBuildingSystem.GetSizeMulti();
                    Vector3 newLocal = new Vector3(sizeXZ.x < 0 ? gridCellSize : 0, 0 , sizeXZ.y < 0 ? gridCellSize : 0);
                    Vector3 visualSize = new Vector3(sizeXZ.x, _visual.localScale.y, sizeXZ.y);
                    _visual.localPosition = newLocal;
                    _visual.localScale = visualSize;
                }
                else
                {
                    _visual.localPosition = Vector3.zero;
                }

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