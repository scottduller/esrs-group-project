using System.Collections.Generic;
using SOScripts;
using UnityEngine;

namespace GridBuildSystem
{
    public class PlacedGridObject : MonoBehaviour
    {
    
        public static PlacedGridObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir,
            PlacedObjectTypeSO placedObjectTypeSo, Transform perant = null)
        {
            Transform placedObjectTransform = Instantiate(placedObjectTypeSo.prefab, worldPosition,
                Quaternion.Euler(0, placedObjectTypeSo.GetRotationAngle(dir), 0),perant);

            PlacedGridObject placedGridObject = placedObjectTransform.GetComponent<PlacedGridObject>();
            placedGridObject._placedObjectTypeSo = placedObjectTypeSo;
            placedGridObject._origin = origin;
            placedGridObject._dir = dir;
            return placedGridObject;
        }
    
    
        private PlacedObjectTypeSO _placedObjectTypeSo;
        private Vector2Int _origin;
        private PlacedObjectTypeSO.Dir _dir;
        
        public List<Vector2Int> GetGridPositionList()
        {
            return _placedObjectTypeSo.GetGridPositionList(_origin, _dir);
        }

        public string DataToString()
        {
            return LevelBuilderManager.Instance.getIndexFromSo(_placedObjectTypeSo)+","+ UtilsClass.Vector3ToString(transform.position) +","+ UtilsClass.QuaternionToString(transform.rotation);
        }
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
        

        public override string ToString()
        {
            return _placedObjectTypeSo.nameString;
        }
    }
}
