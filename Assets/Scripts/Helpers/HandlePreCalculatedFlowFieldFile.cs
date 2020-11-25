using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HandlePreCalculatedFlowFieldFile : MonoBehaviour
{
    public TextAsset input;

    int width, height;

    public Vector3[,] GetFlowFieldVectors()
    {
        string[] lines;
        Vector3[,] items;

        lines = input.text.Split("\n"[0]);
        height = lines.Length;
        width = lines[0].Split(";"[0]).Length;

        items = new Vector3[height, width];

        for(int i = 0; i < lines.Length; i++)
        {
            string[] temp = lines[i].Split(";"[0]);
            for(int j = 0; j < temp.Length; j++)
            {
                string[] values = temp[j].Replace(" ", "").Split(',');
                items[j, i] = new Vector3(float.Parse(values[0]), 0.0f, float.Parse(values[1]));
            }
        }


        return items;
    }

    public int GetWidth()
    {
        string[] lines;

        lines = input.text.Split("\n"[0]);
        width = lines[0].Split(";"[0]).Length;
        return width; 
    }
    public int GetHeight() 
    {
        height = input.text.Split("\n"[0]).Length;
        return height; 
    }
}
