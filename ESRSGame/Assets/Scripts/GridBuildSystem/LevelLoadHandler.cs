using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SOScripts;
using UnityEngine;

public class LevelLoadHandler : MonoBehaviour
{
    private string fileExtension = ".csv"; //file type created
    public string filePath = "Assets/LevelFiles/"; //original file path used for debugging

    public string testNameRead; //test file for debugging
    private List<UtilsClass.LevelPlaceListObject> _placedLevelObjectsList;
    public PlacedListSO _PlacedListSo;
    List<string> _toWrite; //string list (item per line ) to be written to a file
    // Update is called once per frame

    private void Start()
    {
        _placedLevelObjectsList = _PlacedListSo.listOfObjects;
    }

    public void ReadFile(string title, string author)
    {
        ReadFile(title+"_"+author+fileExtension);
    }
    

    public void testRead()
    {
        ReadFile("test","test");
    }
    
    public void ReadFile(string filename)
    {
        int lineNumber = 0;
        filename = filePath + filename;
        string title, author, desc, time;
        using (var reader = new StreamReader(@filename))
        {
            while (!reader.EndOfStream) //while a line still to read
            {
                var line = reader.ReadLine();

                var record = line.Split(','); //split each line by cells ( , )
                if (lineNumber == 0)//if line number is 0, the value will be diagramInstanceData data
                {
                    title = record[0];
                    author = record[1];
                    desc  = record[2];
                    time = record[3];
                    
                }
                else//if line >1 the must contain component values
                { 
                    Transform toBeBuilt = _placedLevelObjectsList.Find(x => x.index == int.Parse(record[0])).PlacedObjectTypeSo.prefab;
                    Vector3 positionVect = new Vector3(float.Parse(record[1]), float.Parse(record[2]), float.Parse(record[3]));
                    Vector3 qEulers = new Vector3(float.Parse(record[4]), float.Parse(record[5]), float.Parse(record[6]));
                    Quaternion quaternion = Quaternion.Euler(qEulers); 
                    Transform newObject = (Transform) Instantiate(toBeBuilt, positionVect, quaternion, transform);
                }
                lineNumber++;
                if (lineNumber > 1000) //if more than 1000lines , then likely to be infinite loop from reader and file is invalid
                {
                    throw new Exception("Invalid Data, while infiloop, >1000 lines");
                }
            }

        }
    }
}
   

