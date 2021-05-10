using System;
using System.Collections.Generic;
using SOScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GridBuildSystem.UI
{


    public class LevelItemSelectUI : MonoBehaviour
    {
        [SerializeField]
        private Dictionary<PlacedObjectTypeSO, Transform> btnTransformDictionary;
        private Transform _btnTemplate;
        private Transform _deselectButton;
        public Sprite tempSprite;

        private void Start()
        {
            btnTransformDictionary = new Dictionary<PlacedObjectTypeSO, Transform>();
            _btnTemplate = transform.Find("btnTemplate");
            _btnTemplate.gameObject.SetActive(false);
            LevelBuilderManager.Instance.OnActiveLayerChange += OnActiveLayerChange;



        }
        
        private void OnActiveLayerChange(object sender, LevelBuilderManager.OnActiveLayerChangeArgs e)
        {
            DisplayNewLayer(e.Objects,e.ActiveLayer);

        }
        

        private void ClearUI()
        {
            foreach (Transform buttonTransform in btnTransformDictionary.Values)
            {
                Destroy(buttonTransform.gameObject);

            }
            if(_deselectButton)
            Destroy(_deselectButton.gameObject);

            btnTransformDictionary.Clear();
        }

        public void DisplayNewLayer(List<PlacedObjectTypeSO> placedObjectTypeSos, GridBuildingSystem gridBuildingSystem)
        {
            ClearUI();
            
            
            Transform btnTransformDeselect = Instantiate(_btnTemplate, transform);
            btnTransformDeselect.gameObject.SetActive(true);
            btnTransformDeselect.Find("Text").GetComponent<TextMeshProUGUI>().SetText("Deselect");
            btnTransformDeselect.Find("Image").GetComponent<Image>().sprite = tempSprite;
            btnTransformDeselect.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Deselect");
                gridBuildingSystem.ChangeItem(null);
                UpdateActiveBuildingTypeButton(null);
            });

            _deselectButton = btnTransformDeselect;
            foreach (PlacedObjectTypeSO placedObject in placedObjectTypeSos)
            {
                Transform btnTransform = Instantiate(_btnTemplate, transform);
                btnTransform.gameObject.SetActive(true);
                Debug.Log(placedObject.nameString);
                btnTransform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(placedObject.nameString);
                btnTransform.Find("Image").GetComponent<Image>().sprite = tempSprite;
                btnTransform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("Selected: " + placedObject.nameString);
                    gridBuildingSystem.ChangeItem(placedObject);
                    UpdateActiveBuildingTypeButton(placedObject);
                });
                btnTransformDictionary[placedObject] = btnTransform;
            }

            UpdateActiveBuildingTypeButton(null);

        }
        
        
        private void UpdateActiveBuildingTypeButton(PlacedObjectTypeSO placedObjectTypeSo)
        {
            foreach (Transform button in btnTransformDictionary.Values)
            {
                
                button.Find("Selected").gameObject.SetActive(false);
                
            }
            

            if (placedObjectTypeSo)
            {
                _deselectButton.Find("Selected").gameObject.SetActive(false);
                btnTransformDictionary[placedObjectTypeSo].Find("Selected").gameObject.SetActive(true);
            }
            else
            {  
                _deselectButton.Find("Selected").gameObject.SetActive(true);
            }
        }

        public void SwitchLayer(int layer)
        {
            
            LevelBuilderManager.Instance.SwitchLayer(layer);
        }
    }


}
