using System;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using GridBuildSystem;
using SOScripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GridBuildSystem
{
    public class GridBuildingSystem : MonoBehaviour
    {
        [NonSerialized] private PlacedObjectTypeSO _placedObjectTypeSo;

        internal bool Interactable = false;
        private bool _dragBuilder;
        public event EventHandler OnSelectedChanged;
        public event EventHandler OnObjectPlaced;
        private bool _intialised = false;
        private GameGrid<GridObject> _grid;
        private PlacedObjectTypeSO.Dir _dir = PlacedObjectTypeSO.Dir.DOWN;
        private Vector2Int _initalClickXZ1, _intialClickXZ2;



        public void InitializeGrid(int gridWidth, int gridHeight, float cellSize, Vector3 offset, int showDebug = 0)
        {
            _grid = new GameGrid<GridObject>(gridWidth, gridHeight, cellSize, offset,
                (GameGrid<GridObject> g, int x, int z) => new GridObject(g, x, z), showDebug);
            _placedObjectTypeSo = null;
            RefreshSelectedObjectType();

            _intialised = true;

        }




        public class GridObject
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
                _grid.TriggerGridObjectChanged(_x, _z);
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
                _grid.TriggerGridObjectChanged(_x, _z);
            }

            public override string ToString()
            {
                return _x + ", " + _z + "\nT= " + _placedGridObject;
            }
        }

        private void GetIntialClick()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                _grid.GetXZ(mouseWorldPosition, out int x, out int z);
                _initalClickXZ1 = new Vector2Int(x, z);
            }

            if (Input.GetButtonDown("Fire2"))
            {
                Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                _grid.GetXZ(mouseWorldPosition, out int x, out int z);
                _intialClickXZ2 = new Vector2Int(x, z);

            }
        }

        private static List<Vector2Int> GetAllGridPositions(Vector2Int endPos, Vector2Int startPos)
        {
            Vector2Int offset = new Vector2Int(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y));
            int loopToX = Math.Abs(startPos.x - endPos.x);
            int loopToY = Math.Abs(startPos.y - endPos.y);
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            for (int x = 0; x <= loopToX; x++)
            {
                for (int y = 0; y <= loopToY; y++)
                {

                    gridPositionList.Add(new Vector2Int(x, y) + offset);
                }

            }

            return gridPositionList;
        }

        public void SetInteractable(bool interactable)
        {
            this.Interactable = interactable;
        }

        public bool GetIsDragBuilder() => _dragBuilder;

        public float GetGridYOffset() => _grid.GetWorldPosition(0, 0).y;




        private void Update()
        {
            if (_intialised && Interactable)
            {
                if (!_dragBuilder)
                {
                    SingleBuildOnClick();
                    SingleDeleteClick();
                    RotateSelected();
                }
                else
                {
                    GetIntialClick();
                    MultiBuildOnClick();
                    MultipleDeleteClick();

                }
                
            }

        }

        public void ChangeItem(PlacedObjectTypeSO placedObjectTypeSo)
        {
            _placedObjectTypeSo = placedObjectTypeSo;
            RefreshSelectedObjectType();
        }


        private void MultiBuildOnClick()
        {
            if (!_placedObjectTypeSo) return;
            if (Input.GetButtonUp("Fire1"))
            {
                Vector3 worldPos = UtilsClass.GetMouseWorldPosition();
                if(worldPos.Equals(Vector3.negativeInfinity)) return;
                _grid.GetXZ(worldPos, out var xEnd, out var zEnd);
                List<Vector2Int> gridPositionList = GetAllGridPositions(new Vector2Int(xEnd, zEnd), _initalClickXZ1);
                // gridPositionList.ForEach(x => Debug.Log(x)); 
                var canBuild = gridPositionList.All(gridPosition =>
                    _grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild());


                if (canBuild)
                {

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        int x = gridPosition.x;
                        int z = gridPosition.y;

                        Vector2Int rotationOffset = _placedObjectTypeSo.GetRotationOffset(_dir);
                        Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(x, z) +
                                                            new Vector3(rotationOffset.x, 0, rotationOffset.y) *
                                                            _grid.GetCellSize();

                        PlacedGridObject placedGridObject = PlacedGridObject.Create(placedObjectWorldPosition,
                            new Vector2Int(x, z), _dir, _placedObjectTypeSo,transform);
                        _grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedGridObject);


                        OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                    }
                }

                else
                {
                    Debug.Log("invalid");

                }
            }
        }
    





    private void SingleBuildOnClick()
        {
            if (!_placedObjectTypeSo) return;
            if (Input.GetButtonUp("Fire1") )
            {
                _grid.GetXZ(UtilsClass.GetMouseWorldPosition(), out var x, out var z);

                List<Vector2Int> gridPositionList = _placedObjectTypeSo.GetGridPositionList(new Vector2Int(x, z), _dir);

                GridObject gridObject = _grid.GetGridObject(x, z);
                Vector3 cellPosition = _grid.GetWorldPosition(x, z);
                gridPositionList.ForEach(a => Debug.Log(_grid.GetGridObject(a.x, a.y).CanBuild()));
                //testing Can Build
                bool canBuild = gridPositionList.All(gridPosition =>
                    _grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild());
                
                if (canBuild)
                {
                    Vector2Int rotationOffset = _placedObjectTypeSo.GetRotationOffset(_dir);
                    Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(x, z) +
                                                        new Vector3(rotationOffset.x, 0, rotationOffset.y) *
                                                        _grid.GetCellSize();

                    PlacedGridObject placedGridObject = PlacedGridObject.Create(placedObjectWorldPosition,
                        new Vector2Int(x, z), _dir, _placedObjectTypeSo,transform);
 

                    foreach (Vector2Int gridPosition in gridPositionList)
                        _grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedGridObject);


                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    CmUtilsClass.CreateWorldTextPopup("Cannot build here!", cellPosition);
                }



            }
        }

    private void MultipleDeleteClick()
    {
        if (Input.GetButtonUp("Fire2"))
        {
            _grid.GetXZ(UtilsClass.GetMouseWorldPosition(), out var x, out var z);
            foreach (Vector2Int pos in GetAllGridPositions(new Vector2Int(x, z), _intialClickXZ2))
            {
                GridObject gridObject = _grid.GetGridObject(pos.x, pos.y);
                PlacedGridObject placedGridObject = gridObject.GetPlacedObject();
                if (placedGridObject)
                {
                    placedGridObject.DestroySelf();
                    var gridPositionList = placedGridObject.GetGridPositionList();
                    foreach (Vector2Int gridPosition in gridPositionList)
                        _grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }

            }

        }
    }

    private void SingleDeleteClick()
        {
            if (Input.GetButtonUp("Fire2"))
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
        }

        private void RotateSelected()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _dir = PlacedObjectTypeSO.GetNextDir(_dir);
                Debug.Log(_dir);
            }


        }
        
        



        private void DeselectObjectType() {
            _placedObjectTypeSo = null; 
            RefreshSelectedObjectType();
        }

        private void RefreshSelectedObjectType()
        {
            if (_placedObjectTypeSo)
            {
                _dragBuilder = _placedObjectTypeSo.dragBuild;
            }
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }


        public Vector2Int GetGridPosition(Vector3 worldPosition) {
            _grid.GetXZ(worldPosition, out int x, out int z);
            return new Vector2Int(x, z);
        }

        public bool GetMulti()
        {
            return _dragBuilder;
        }

        public Vector3 GetMouseWorldSnappedPositionSingle() {
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

        public float GetCellSize()
        {
            return _grid.GetCellSize();
        }

        public Vector2 GetSizeMulti()
        {
            float cellSize = GetCellSize();
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            _grid.GetXZ(mousePosition, out int x, out int z);
            int xSize = _initalClickXZ1.x-x;
            int zSize = _initalClickXZ1.y-z;
            xSize += (xSize < 0 ? -1 :1);
            zSize += (zSize < 0 ? -1 : 1);
            Vector2 sizeXZ = new Vector2(xSize, zSize) *cellSize;
            if (_placedObjectTypeSo != null) {
                

                
                return sizeXZ;
            } else {
                return sizeXZ;
            }
        }

        public Quaternion GetPlacedObjectRotation() {
            if (_placedObjectTypeSo != null) {
                return Quaternion.Euler(0, _placedObjectTypeSo.GetRotationAngle(_dir), 0);
            } else {
                return Quaternion.identity;
            }
        }

        public PlacedObjectTypeSO GetPlacedObjectTypeSo() {
            return _placedObjectTypeSo;
        }
        
        





    
    
    
    }
}

[CustomEditor(typeof(GridBuildingSystem))]
public class GridBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = target as GridBuildingSystem;
        GUILayout.Label("  Interactable: "+script.Interactable.ToString());
        

 
    }
}
