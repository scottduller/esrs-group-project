using System;
using CodeMonkey.Utils;
using UnityEngine;

namespace GridBuildSystem
{
    public class GameGrid<TGridObject>
    {

        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;

        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int x;
            public int z;
        }
        
        private readonly int _width;
        private readonly int _height;
        
        [SerializeField]
        private readonly TGridObject[,] _gridArray;

        private readonly float _cellSize;
        private readonly Vector3 _originPosition;
        
        
        public GameGrid(int width, int height, float cellSize, Vector3 originPosition, Func<GameGrid<TGridObject>,int,int, TGridObject>  createGridObject, int showDebug = 2)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;
            _gridArray = new TGridObject[width, height];
            var debugTextArray = new TextMesh[width, height];

            _gridArray = new TGridObject[width, height];
            for (var x = 0; x < _gridArray.GetLength(0); x++)
            for (var z = 0; z < _gridArray.GetLength(1); z++) _gridArray[x, z] = createGridObject(this,x,z);


            if (showDebug==0) return;
            for (var x = 0; x < _gridArray.GetLength(0); x++)
            for (var z = 0; z < _gridArray.GetLength(1); z++)
            {
                    

                Debug.DrawLine(GetWorldPosition(x,z),GetWorldPosition(x,z+1),Color.white,100f);
                Debug.DrawLine(GetWorldPosition(x,z),GetWorldPosition(x+1,z),Color.white,100f);
                    
                if(showDebug!=2)continue;
                debugTextArray[x,z] = CmUtilsClass.CreateWorldText(_gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z)+new Vector3(cellSize,0,cellSize)*0.5f, 20,
                    Color.white, TextAnchor.MiddleCenter);
            }

            Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height),Color.white,100f);
            Debug.DrawLine(GetWorldPosition(width,0),GetWorldPosition(width,height),Color.white,100f);
            if(showDebug!=2)return;
            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.z].text = _gridArray[eventArgs.x, eventArgs.z]?.ToString();
            };
        }

        public void TriggerGridObjectChanged(int x, int z)
        {
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs {x = x, z = z});
        }

        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x,0, z) * _cellSize + _originPosition;
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPosition.x-_originPosition.x) / _cellSize);
            z = Mathf.FloorToInt((worldPosition.z-_originPosition.z) / _cellSize);

        }

        public float GetCellSize()
        {
            return _cellSize;
        }

        public void SetGridObject(int x, int z, TGridObject value)
        {
            if (x < 0 || z < 0 || x >= _width || z >= _height) return;
            _gridArray[x, z] = value;
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs{x=x,z=z});
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            int x, z;
            GetXZ(worldPosition,out x,out z);
            SetGridObject(x,z,value);

        }

        public TGridObject GetGridObject(int x, int z)
        {
            if (x < 0 || z < 0 || x >= _width || z >= _height) return default;
            return _gridArray[x, z];

        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            int x, z;
            GetXZ(worldPosition,out x, out z);
            return _gridArray[x, z];
        }
        
    }


}
