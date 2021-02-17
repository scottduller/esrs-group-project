using System;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using SOScripts;
using UnityEngine;

namespace GridBuildSystem
{
    public class GridBuildingSystem : MonoBehaviour
    {
        [NonSerialized]  private PlacedObjectTypeSO _placedObjectTypeSo;
        [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSoList;

        public event EventHandler OnSelectedChanged;
        public event EventHandler OnObjectPlaced;
    
        private GameGrid<GridObject> _grid;
        private PlacedObjectTypeSO.Dir _dir = PlacedObjectTypeSO.Dir.DOWN;
        private void Awake()
        {
            var gridWidth = 10;
            var gridHeight = 10;
            var cellSize = 1f;
            _grid = new GameGrid<GridObject>(gridWidth,gridHeight,cellSize,Vector3.zero,(GameGrid<GridObject> g , int x, int z) => new GridObject(g,x,z),1);
            _placedObjectTypeSo = placedObjectTypeSoList[0];
        }

    
        public class  GridObject
        {
            private readonly GameGrid<GridObject> _grid;
            private readonly int _x;
            private readonly int _z;
            private PlacedGridObject _placedGridObject;

            public GridObject(GameGrid<GridObject> grid, int x, int z)
            {
                _grid = grid;
                _x = x;
                _z = z;
            
            }

            public void SetPlacedObject(PlacedGridObject placedGridObject)
            {
                _placedGridObject = placedGridObject;
                _grid.TriggerGridObjectChanged(_x,_z);
            }

            public PlacedGridObject GetPlacedObject()
            {
                return _placedGridObject;
            }
        

            public bool CanBuild()
            {
                return _placedGridObject == null;
            }

            public void ClearPlacedObject()
            {
                _placedGridObject = null;
                _grid.TriggerGridObjectChanged(_x,_z);
            }

            public override string ToString()
            {
                return _x + ", " + _z + "\nT= " + _placedGridObject;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _grid.GetXZ(UtilsClass.GetMouseWorldPosition(), out var x, out var z);

                var gridPositionList = _placedObjectTypeSo.GetGridPositionList(new Vector2Int(x, z), _dir);

                GridObject gridObject = _grid.GetGridObject(x, z);
                Vector3 cellPosition = _grid.GetWorldPosition(x, z);

                //testing Can Build
                var canBuild = gridPositionList.All(gridPosition =>
                    _grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild());


                if (canBuild)
                {
                    Vector2Int rotationOffset = _placedObjectTypeSo.GetRotationOffset(_dir);
                    Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(x, z) +
                                                        new Vector3(rotationOffset.x, 0, rotationOffset.y) *
                                                        _grid.GetCellSize();

                    PlacedGridObject placedGridObject = PlacedGridObject.Create(placedObjectWorldPosition,
                        new Vector2Int(x, z), _dir, _placedObjectTypeSo);


                    foreach (Vector2Int gridPosition in gridPositionList)
                        _grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedGridObject);


                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    CmUtilsClass.CreateWorldTextPopup("Cannot build here!", cellPosition);
                }



            }

            if (Input.GetMouseButtonDown(1))
            {
                GridObject gridObject = _grid.GetGridObject(UtilsClass.GetMouseWorldPosition());
                PlacedGridObject placedGridObject = gridObject.GetPlacedObject();
                if (placedGridObject)
                {
                    placedGridObject.DestroySelf();
                    var gridPositionList = placedGridObject.GetGridPositionList();
                    foreach (Vector2Int gridPosition in gridPositionList)
                        _grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }

            }


            if (Input.GetKeyDown(KeyCode.R))
            {
                _dir = PlacedObjectTypeSO.GetNextDir(_dir);
                Debug.Log(_dir);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) _placedObjectTypeSo = placedObjectTypeSoList[0]; RefreshSelectedObjectType();

            if (Input.GetKeyDown(KeyCode.Alpha2)) _placedObjectTypeSo = placedObjectTypeSoList[1]; RefreshSelectedObjectType();
            if (Input.GetKeyDown(KeyCode.Alpha0)) DeselectObjectType();

        }



        private void DeselectObjectType() {
            _placedObjectTypeSo = null; RefreshSelectedObjectType();
        }

        private void RefreshSelectedObjectType() {
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }


        public Vector2Int GetGridPosition(Vector3 worldPosition) {
            _grid.GetXZ(worldPosition, out int x, out int z);
            return new Vector2Int(x, z);
        }

        public Vector3 GetMouseWorldSnappedPosition() {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            _grid.GetXZ(mousePosition, out int x, out int z);

            if (_placedObjectTypeSo != null) {
                Vector2Int rotationOffset = _placedObjectTypeSo.GetRotationOffset(_dir);
                Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * _grid.GetCellSize();
                return placedObjectWorldPosition;
            } else {
                return mousePosition;
            }
        }

        public Quaternion GetPlacedObjectRotation() {
            if (_placedObjectTypeSo != null) {
                return Quaternion.Euler(0, _placedObjectTypeSo.GetRotationAngle(_dir), 0);
            } else {
                return Quaternion.identity;
            }
        }

        public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
            return _placedObjectTypeSo;
        }





    
    
    
    }
}
