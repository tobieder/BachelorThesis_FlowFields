using UnityEngine;
using System.IO;

public static class CSVManager
{
    private static string s_DataDirectoryName = "Data";
    private static string s_DataSeparator = ",";

    /*
    private static string[] sa_DataHeaders = new string[5]
    {
        "Destination",
        "Map Size",
        "Distance",
        "Units",
        "Execution Time"
    };
    */

    #region Interactions

    public static void AppendToData(string _filename, string[] _dataHeaders, string[] _strings)
    {
        VerifyDirectory();
        VerifyFile(_filename, _dataHeaders);
        using(StreamWriter sw = File.AppendText(GetFilePath(_filename)))
        {
            string finalString = "";
            for(int i = 0; i < _strings.Length; i++)
            {
                if(finalString != "")
                {
                    finalString += s_DataSeparator;
                }
                finalString += _strings[i];
            }
            finalString += s_DataSeparator;
            sw.WriteLine(finalString);
        }
    }

    public static void CreateData(string _filename, string[] _dataHeaders)
    {
        VerifyDirectory();
        using(StreamWriter sw = File.CreateText(GetFilePath(_filename)))
        {
            string finalString = "";
            for(int i = 0; i < _dataHeaders.Length; i++)
            {
                if(finalString != "")
                {
                    finalString += s_DataSeparator;
                }
                finalString += _dataHeaders[i];
            }
            finalString += s_DataSeparator;
            sw.WriteLine(finalString);
        }
    }

    #endregion

    #region Operations

    static void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    static void VerifyFile(string _filename, string[] _dataHeaders)
    {
        string file = GetFilePath(_filename);
        if(!File.Exists(file))
        {
            CreateData(_filename, _dataHeaders);
        }
    }

    #endregion

    #region Queries

    static string GetDirectoryPath()
    {
        return Application.dataPath + "/" + s_DataDirectoryName;
    }

    static string GetFilePath(string _filename)
    {
        return GetDirectoryPath() + "/" + _filename;
    }

    #endregion
}
