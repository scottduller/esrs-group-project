using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Grid;
using SOScripts;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private LevelTileSo _levelTileSo;
    
    private GameGrid<GridObject> _grid;
    private LevelTileSo.Dir dir = LevelTileSo.Dir.DOWN;
    private void Awake()
    {
        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        _grid = new GameGrid<GridObject>(gridWidth,gridHeight,cellSize,Vector3.zero,(GameGrid<GridObject> g , int x, int z) => new GridObject(g,x,z));
    }

    
    public class  GridObject
    {
        private readonly GameGrid<GridObject> _grid;
        private  int x;
        private  int z;
        private Transform _transform;

        public GridObject(GameGrid<GridObject> grid, int x, int z)
        {
            this._grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetTransform(Transform transform)
        {
            this._transform = transform;
            _grid.TriggerGridObjectChanged(x,z);
        }

        public bool canBuild()
        {
            return _transform == null;
        }

        public void ClearTransform()
        {
            _transform = null;
        }

        public override string ToString()
        {
            return x + ", " + z + "\nT= " + _transform;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _grid.GetXZ(UtilsClass.GetMouseWorldPosition(), out int x, out int z);

            List<Vector2Int> gridPositionList = _levelTileSo.GetGridPositionList(new Vector2Int(x, z), LevelTileSo.Dir.DOWN);
            
            GridObject gridObject = _grid.GetGridObject(x, z);
            Vector3 cellPosition = _grid.GetWorldPosition(x, z);
            
            //testing Can Build
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (!_grid.GetGridObject(gridPosition.x,gridPosition.y).canBuild())
                {
                    canBuild = false;
                    break;
                }

                
            }
            
            
            
            if (canBuild)
            {

                Transform buildTransform = Instantiate(_levelTileSo.prefab,cellPosition, Quaternion.identity,transform);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    Debug.Log(gridPosition);
                    _grid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(buildTransform);
                    
                }
            }
            else
            {
                CmUtilsClass.CreateWorldTextPopup("Cannot build here!",cellPosition);
            }
            
            

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
           
        }
    }
}
