using System;
using System.Collections;
using System.Collections.Generic;
using GridBuildSystem;
using UnityEngine;

public class LevelSaveHandler : MonoBehaviour
{
    private string fileExtension = ".csv"; //file type created
    public string filePath = "Assets/LevelFiles/"; //original file path used for debugging

    public string testNameRead; //test file for debugging
 
    List<string> _toWrite; //string list (item per line ) to be written to a file
    // Update is called once per frame

    public bool WriteLevelToFile(string title, string author, string desc, List<PlacedGridObject> floor, List<PlacedGridObject> interact)
    {
        filePath = filePath+"/"; //get filepath
        _toWrite = new List<string>();
        WriteTitleBar(title, author, desc); //write titlebar
        //for each layer, write each component in order with it values
        foreach (PlacedGridObject placedGridObject in floor)
        {
            AddLevelItem(placedGridObject);
        }
        foreach (PlacedGridObject placedGridObject in interact)
        {
            AddLevelItem(placedGridObject);
        }

        WriteFile(title, author);
        return true;
    }


    private void WriteTitleBar(string title, string author, string desc)
    {
        string recordData = title + "," + author + "," + desc + ","+ System.DateTime.Now + ",\0";
        _toWrite.Add(recordData);
    }

    private void AddLevelItem(PlacedGridObject placedGridObject)
    {
        _toWrite.Add(placedGridObject.DataToString() + ",\0");
        
    }
    
    private bool WriteFile(string title, string author)
    {
        try
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filePath+ title + "_" + author + fileExtension, false))
            {
                foreach (string record in _toWrite)
                {
                    file.WriteLine(record);
                }
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
}
