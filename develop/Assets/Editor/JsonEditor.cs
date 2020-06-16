using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;


public class JsonEditor : EditorWindow
{
    private string className;
    private FieldInfo[] field;
    private object classT;

    [SerializeField] private IList<int> iList;
    [SerializeField] private IList<float> fList;
    [SerializeField] private IList<string> sList;

    private Type type;

    [UnityEditor.MenuItem("Tools/JsonEditor ")]
    static void Init()
    {
        JsonEditor window = (JsonEditor) GetWindow(typeof(JsonEditor), true, "JsonEditor"); //新しいウィンドウ作る
        window.Show();
    }


    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        var _className = EditorGUILayout.TextField("クラス名", className);
        if (_className == null) return;

        GUIEdit();
        if (className != _className)
        {
            type = Type.GetType(_className);
            if (type == null)
            {
                EditorGUILayout.HelpBox("この名前のクラスは存在しません", MessageType.Error);
                return;
            }
            classT = Activator.CreateInstance(type);
            className = _className;
        }


        if (type == classT.GetType())
        {
            EditorGUILayout.LabelField("true");
            field = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                   BindingFlags.DeclaredOnly);
        }
        else
        {
            EditorGUILayout.HelpBox("この名前のクラスは存在しません", MessageType.Error);
            return;
        }



        foreach (FieldInfo item in field)
        {
            Debug.Log(item.FieldType+item.Name);
            CheckType(item);
           
        }
    }

    void GUIEdit()
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        if (GUILayout.Button("保存")) Save();

        EditorGUILayout.EndHorizontal();
    }

    void Save()
    {
        var path = EditorUtility.SaveFilePanel("Save", "Assets", "default_Name", "json");
        if (!string.IsNullOrEmpty(path))
        {
            JsonConvert.SaveFile(path, classT);
        }
    }

    private void CheckType(FieldInfo fieldInfo)
    {
        if (fieldInfo.FieldType == typeof(int))
        {
            int a = (int) fieldInfo.GetValue(classT);
            a = EditorGUILayout.IntField(fieldInfo.Name, a);
            fieldInfo.SetValue(classT, a);
            return;
        }
        else if (fieldInfo.FieldType == typeof(float))
        {
            float a = (float) fieldInfo.GetValue(classT);
            a = EditorGUILayout.FloatField(fieldInfo.Name, a);
            fieldInfo.SetValue(classT, a);
            return;
        }
        else if (fieldInfo.FieldType == typeof(string))
        {
            string a = (string) fieldInfo.GetValue(classT);
            a = EditorGUILayout.TextField(fieldInfo.Name, a);
            fieldInfo.SetValue(classT, a);
            return;
        }

        else if (fieldInfo.FieldType == typeof(bool))
        {
            bool a = (bool) fieldInfo.GetValue(classT);
            a = EditorGUILayout.Toggle(fieldInfo.Name, a);
            fieldInfo.SetValue(classT, a);
            return;
        }
        else if (fieldInfo.FieldType == typeof(Vector3))
        {
            Vector3 a = (Vector3) fieldInfo.GetValue(classT);
            a = EditorGUILayout.Vector3Field(fieldInfo.Name, a);
            fieldInfo.SetValue(fieldInfo, a);
            return;
        }
        else if (typeof(List<int>).IsAssignableFrom(fieldInfo.FieldType))
        {
            var ilist = (IList<int>) fieldInfo.GetValue(classT);
            IList<int> list;
            list = new List<int> {1};
            iList = new List<int>();
            iList = ilist ?? list;
            var so = new SerializedObject(this);
            ReorderableList RLi;
            RLi = new ReorderableList((IList) iList, typeof(int));
            RLi.drawElementCallback += DrawElementI;
            RLi.drawElementBackgroundCallback += DrawElementBackground;
            EditorGUILayout.LabelField(fieldInfo.Name);
            GUILayout.BeginVertical();
            RLi.DoLayoutList();
            
            GUILayout.EndVertical();
            fieldInfo.SetValue(classT, (List<int>) iList);
            return;
        }
        else if (typeof(List<float>).IsAssignableFrom(fieldInfo.FieldType))
        {
            var flist= (IList<float>) fieldInfo.GetValue(classT);
            IList<float> list;
            list = new List<float> {1, 1, 1};
            fList = new List<float>();
            fList = flist ?? list;
            var so = new SerializedObject(this);

            ReorderableList RLf;
            RLf = new ReorderableList((IList) fList, typeof(float));
            RLf.drawElementCallback += DrawElementF;
            RLf.drawElementBackgroundCallback += DrawElementBackground;
            EditorGUILayout.LabelField(fieldInfo.Name);
            GUILayout.BeginVertical();
            RLf.DoLayoutList();
            GUILayout.EndVertical();
            fieldInfo.SetValue(classT, (List<float>) fList);
            return;
        }
        else if (typeof(List<string>).IsAssignableFrom(fieldInfo.FieldType))
        {
            var slist= (IList<string>) fieldInfo.GetValue(classT);
            IList<string> list;
            list = new List<string> {"a"};
            sList = new List<string>();
            sList = slist ?? list;
            var so = new SerializedObject(this);
            ReorderableList RLs;

            RLs = new ReorderableList((IList) sList, typeof(string));
            RLs.drawElementCallback += DrawElementS;
            RLs.drawElementBackgroundCallback += DrawElementBackground;
            EditorGUILayout.LabelField(fieldInfo.Name);
            GUILayout.BeginVertical();
            RLs.DoLayoutList();
            GUILayout.EndVertical();
            fieldInfo.SetValue(classT, (List<string>) sList);
            return;
        }
        
       
    }

    private void DrawElementI(Rect rect, int index, bool isActive, bool isFocused)
    {
        //要素を書き換えられるようにフィールドを表示
        iList[index] = EditorGUI.IntField(rect, iList[index]);
    }
    private void DrawElementF(Rect rect, int index, bool isActive, bool isFocused)
    {
        //要素を書き換えられるようにフィールドを表示
        fList[index] = EditorGUI.FloatField(rect, fList[index]);
    }
    private void DrawElementS(Rect rect, int index, bool isActive, bool isFocused)
    {
        //要素を書き換えられるようにフィールドを表示
        sList[index] = EditorGUI.TextArea(rect, sList[index]);
    }

    private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
    {
        //選択しているやつだけ色を変更する
        if (isFocused)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.red);
            tex.Apply();
            GUI.DrawTexture(rect, tex as Texture);
        }
    }
}