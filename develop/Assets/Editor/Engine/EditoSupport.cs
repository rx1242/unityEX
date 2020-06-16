using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class EditoSupport 
{
    public static void Label(Rect rec,string str)
    {
        EditorGUI.LabelField(rec,str);
    }

    public static void Label(string str)
    {
        EditorGUILayout.LabelField(str);
    }

    public static bool Button(Rect rec,string str)
    {
       return GUI.Button(rec, str);
    }

    public static bool FoldOut(bool flag, string label,GUIStyle style)
    {
        return EditorGUILayout.Foldout(flag, label, style);
    }
    public static void Space(float pixels)
    {
        GUILayout.Space(pixels);
    }
    public static void AddIndent(int a)
    {
        for (int i = 0; i < a; i++)
        {
            EditorGUI.indentLevel++;
        }
    }
    public static void DecreaseIndent (int a)
    {
        for (int i = 0; i < a; i++)
        {
            EditorGUI.indentLevel--;
        }
    }

    public static void SaveAseets()
    {
        AssetDatabase.SaveAssets();
    }
    /*public static T  GetObject<T>(string str, object obj, Type type, bool flag)
    {
        var a = EditorGUILayout.ObjectField(str, obj, type, true)as );
        return a;
    }*/
    
    
}
