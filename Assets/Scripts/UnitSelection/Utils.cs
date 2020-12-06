using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    static Texture2D _whiteTexture;
    public static Texture2D whiteTexture
    {
        get
        {
            if(_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect _rect, Color _color)
    {
        GUI.color = _color;
        GUI.DrawTexture(_rect, whiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect _rect, float _thickness, Color _color)
    {
        // Top
        Utils.DrawScreenRect(new Rect(_rect.xMin, _rect.yMin, _rect.width, _thickness), _color);
        // Left
        Utils.DrawScreenRect(new Rect(_rect.xMin, _rect.yMin, _thickness, _rect.height), _color);
        // Right
        Utils.DrawScreenRect(new Rect(_rect.xMax - _thickness, _rect.yMin, _thickness, _rect.height), _color);
        // Bottom
        Utils.DrawScreenRect(new Rect(_rect.xMin, _rect.yMax - _thickness, _rect.width, _thickness), _color);
    }

    public static Rect GetScreenRect(Vector3 _screenPosition1, Vector3 _screenPosition2)
    {
        _screenPosition1.y = Screen.height - _screenPosition1.y;
        _screenPosition2.y = Screen.height - _screenPosition2.y;

        var topLeft = Vector3.Min(_screenPosition1, _screenPosition2);
        var bottomRight = Vector3.Max(_screenPosition1, _screenPosition2);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
