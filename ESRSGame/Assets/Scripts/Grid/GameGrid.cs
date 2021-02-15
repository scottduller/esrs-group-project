using System;
using CodeMonkey.Utils;
using UnityEngine;


namespace Grid
{
    public class GameGrid<TGridObject>
    {

        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;

        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int x;
            public int z;
        }
        
        private int width;
        private int height;
        private TGridObject[,] gridArray;
        private TextMesh[,] debugTextArray;
        private float cellSize;
        private Vector3 originPosition;
        
        
        public GameGrid(int width, int height, float cellSize, Vector3 originPosition, Func<GameGrid<TGridObject>,int,int, TGridObject, bool showDebug = true)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            gridArray = new TGridObject[width, height];
            debugTextArray = new TextMesh[width, height];

            gridArray = new TGridObject[width, height];
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    gridArray[x, z] = CreatGridObject();
                }
                
            }
            
            
            showDebug = true;
            if (!showDebug) return;
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    debugTextArray[x,z] = CmUtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z)+new Vector3(cellSize,0,cellSize)*0.5f, 20,
                        Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x,z),GetWorldPosition(x,z+1),Color.white,100f);
                    Debug.DrawLine(GetWorldPosition(x,z),GetWorldPosition(x+1,z),Color.white,100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height),Color.white,100f);
            Debug.DrawLine(GetWorldPosition(width,0),GetWorldPosition(width,height),Color.white,100f);

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
            };
        }

        private Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x,0, z) * cellSize + originPosition;
        }

        private void GetXZ(Vector3 worldPostion, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPostion.x-originPosition.x) / cellSize);
            z = Mathf.FloorToInt((worldPostion.z-originPosition.z) / cellSize);

        }
        
        public void SetGridObject(int x, int z, TGridObject value)
        {
            if (x < 0 || z < 0 || x >= width || z >= height) return;
            gridArray[x, z] = value;
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
            if (x < 0 || z < 0 || x >= width || z >= height) return default(TGridObject);
            return gridArray[x, z];

        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            int x, z;
            GetXZ(worldPosition,out x, out z);
            return gridArray[x, z];
        }
        
    }


}
