using System;
using System.Collections.Generic;
using System.Linq;
using SOScripts;
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
        private List<UtilsClass.LevelPlaceListObject> _placedLevelObjectsList;
        private List<PlacedObjectTypeSO> currentLayerPlacedObjects;
        public PlacedListSO _PlacedListSo;
        private Transform _colliderPlane;


        public int levelWidth =20;
        public int levelHeight = 20;
        public float cellSize = 1f;
        
        public event EventHandler<OnActiveLayerChangeArgs> OnActiveLayerChange;
    
        public class  OnActiveLayerChangeArgs :EventArgs
        {
            public GridBuildingSystem ActiveLayer;
            public List<PlacedObjectTypeSO> Objects;
        };

        private void Awake()
        {
            Instance = this;
            try
            {
                _floorLayer = transform.Find("GridManagerFloor").GetComponent<GridBuildingSystem>();
                _topLayer = transform.Find("GridManagerInteractable").GetComponent<GridBuildingSystem>();
                _colliderPlane = transform.Find("ColliderPlane");
                _activeLayer = _floorLayer;
                _placedLevelObjectsList = _PlacedListSo.listOfObjects;


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
            List<UtilsClass.LevelPlaceListObject> tempItemList = _placedLevelObjectsList.Where(x => x.isFloor == true).ToList();
            currentLayerPlacedObjects = tempItemList.ConvertAll((x) => x.PlacedObjectTypeSo);
            Debug .Log(currentLayerPlacedObjects.Count);
            OnActiveLayerChange?.Invoke(this,new OnActiveLayerChangeArgs{ActiveLayer = _activeLayer, Objects = currentLayerPlacedObjects});
            

        }
        





        public void SwitchLayer(int layerValue)
        {
            List<UtilsClass.LevelPlaceListObject> tempItemList;
            switch (layerValue)
            {
                case 0:
                    _topLayer.SetInteractable(false);
                    _floorLayer.SetInteractable(true);
                    
                    _activeLayer = _floorLayer;
                    _colliderPlane.position = new Vector3(0, 0, 0);
                    tempItemList = _placedLevelObjectsList.Where(x => x.isFloor == true).ToList();
                    currentLayerPlacedObjects = tempItemList.ConvertAll((x) => x.PlacedObjectTypeSo);
    
                    OnActiveLayerChange?.Invoke(this,new OnActiveLayerChangeArgs{ActiveLayer = this._activeLayer, Objects = currentLayerPlacedObjects});
                    break;
                case 1:
                    _floorLayer.SetInteractable(false);
                    _topLayer.SetInteractable(true);
                    _activeLayer = _topLayer;
                    _colliderPlane.position = new Vector3(0, 1, 0);
                    tempItemList = _placedLevelObjectsList.Where(x => x.isInteractable == true).ToList();
                    currentLayerPlacedObjects = tempItemList.ConvertAll((x) => x.PlacedObjectTypeSo);
                    OnActiveLayerChange?.Invoke(this,new OnActiveLayerChangeArgs{ActiveLayer = this._activeLayer, Objects = currentLayerPlacedObjects});
                    break;
                case 2:
                    break;
                
            }
        }

        public GridBuildingSystem GetActiveGrid() => _activeLayer;

        public void testSaveLevel()
        {
            SaveLevel("test","test","test");
        }
        
        
        public void SaveLevel(String title, String author, String desc)
        {
            transform.GetComponent<LevelSaveHandler>().WriteLevelToFile(title, author, desc,
                transform.Find("GridManagerFloor").GetComponentsInChildren<PlacedGridObject>().ToList(),
                transform.Find("GridManagerInteractable").GetComponentsInChildren<PlacedGridObject>().ToList());
        }

        public int getIndexFromSo(PlacedObjectTypeSO so)
        {
           return _placedLevelObjectsList.Find((x) => x.PlacedObjectTypeSo.nameString == so.nameString).index;
        }

    }
}
