using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    private GameGrid<GridObject> _grid;
    private void Awake()
    {
        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        _grid = new GameGrid<GridObject>(gridWidth,gridHeight,cellSize,Vector3.zero,(GameGrid<GridObject> g , int x, int z) => new GridObject(g,x,z));
    }
    
    public class  GridObject
    {
        private GameGrid<GridObject> _grid;
        private int x;
        private int z;

        public GridObject(GameGrid<GridObject> grid, int x, int z)
        {
            this._grid = grid;
            this.x = x;
            this.z = z;
        }
    }
}
