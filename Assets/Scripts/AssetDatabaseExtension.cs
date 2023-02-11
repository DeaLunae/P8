#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEditor;

public static class AssetDatabaseExtension
{
    [MenuItem("Cheats/Gen Icon")]
    public static void GenerateIcon()
    {
        Texture2D tex = AssetPreview.GetAssetPreview(Selection.activeGameObject);
        var nameofobject = AssetDatabase.GetAssetPath(Selection.activeGameObject);
        var lastIndexOfPeriod = nameofobject.LastIndexOf(".", StringComparison.Ordinal);
        if (lastIndexOfPeriod == -1)
        {
            return;
        }
        tex = FloodFill(tex, new Vector2Int(0,0),Color.clear);
        byte[] bytes = tex.EncodeToPNG();
        // For testing purposes, also write to a file in the project folder

        System.IO.File.WriteAllBytes(nameofobject[..nameofobject.LastIndexOf(".",StringComparison.Ordinal)] + ".png", bytes);
        AssetDatabase.ImportAsset(nameofobject[..nameofobject.LastIndexOf(".",StringComparison.Ordinal)] + ".png");
        
    }

    public static Texture2D FloodFill(Texture2D texture, Vector2Int point, Color replacementColor)
    {
        Stack<Vector2Int> pixels = new Stack<Vector2Int>();
        var targetColor = texture.GetPixel(point.x, point.y);
        pixels.Push(point);
 
        while (pixels.Count > 0)
        {
            Vector2Int a = pixels.Pop();
            if (a.x < texture.width && a.x >= 0 && 
                a.y < texture.height && a.y >= 0)//make sure we stay within bounds
            {
                if (texture.GetPixel(a.x, a.y) == targetColor)
                {
                    texture.SetPixel(a.x, a.y, replacementColor);
                    pixels.Push(new Vector2Int(a.x - 1, a.y));
                    pixels.Push(new Vector2Int(a.x + 1, a.y));
                    pixels.Push(new Vector2Int(a.x, a.y - 1));
                    pixels.Push(new Vector2Int(a.x, a.y + 1));
                }
            }
        }
        return texture;
    }
}
#endif