using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BenchmarkMenu : MonoBehaviour
{
    public void Start()
    {
        var info = new DirectoryInfo(Application.dataPath + "/Data");
        FileInfo[] fileInfo = info.GetFiles("*.csv");

        for(int i = 0; i < fileInfo.Length; i++)
        {
            Debug.Log(fileInfo[i].Name);
        }
    }

    public void LoadMainMenu()
    {
        GameManager.instance.LoadMainMenu();
    }
}
