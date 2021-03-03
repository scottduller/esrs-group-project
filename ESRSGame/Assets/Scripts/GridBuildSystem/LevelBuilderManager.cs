using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GridBuildSystem
{
    public class LevelBuilderManager : MonoBehaviour
    {
        
        public static LevelBuilderManager Instance { get; private set; }
        
        private GridBuildingSystem _floorLayer;
        private GridBuildingSystem _topLayer;
        private GridBuildingSystem _activeLayer;


        public int levelWidth =20;
        public int levelHeight = 20;
        public float cellSize = 1f;
        
        public event EventHandler<OnActiveLayerChangeArgs> OnActiveLayerChange;
    
        public class  OnActiveLayerChangeArgs :EventArgs
        {
            public GridBuildingSystem ActiveLayer;
        };

        private void Awake()
        {
            Instance = this;

            try
            {
                _floorLayer = transform.Find("GridManagerFloor").GetComponent<GridBuildingSystem>();
                _topLayer = transform.Find("GridManagerInteractable").GetComponent<GridBuildingSystem>();
                _activeLayer = _floorLayer;
                

            }
            
            
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }

            _floorLayer.InitializeGrid(levelWidth, levelHeight, cellSize, Vector3.zero, 1);
            _topLayer.InitializeGrid(levelWidth,levelHeight,cellSize,new Vector3(0,1,0),0);
            _floorLayer.SetInteractable(true);

        }

        private void Start()
        {
            OnActiveLayerChange?.Invoke(this,new OnActiveLayerChangeArgs{ActiveLayer = _activeLayer});

        }
        
    

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _floorLayer.SetInteractable(false);
                _topLayer.SetInteractable(true);
                _activeLayer = _topLayer;
                OnActiveLayerChange?.Invoke(this,new OnActiveLayerChangeArgs{ActiveLayer = this._activeLayer});
            }
        
        }

        public GridBuildingSystem GetActiveGrid() => _activeLayer;



    }
}
