using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
#endif
/// <summary>
/// インスペクターからResources/BGM内のaudioclipをpopupで選択できる拡張
/// stringのフィールにattribute SelectBGMをつけて使用する
/// /// </summary>
class SelectBGMAttribute : PropertyAttribute { }
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SelectBGMAttribute))]

public class SelectionBGM : PropertyDrawer
{

    List<string> list;
    //List<string> AllBgmName
    //{
    //    get
    //    {
    //        List<string> BGMNames = new List<string>();
    //        //Resources/BGMからSceneのPathを読み込む  

    //        foreach (var s in Resources.LoadAll("Audio/BGM"))
    //        {
    //            string n = s.ToString();
    //            BGMNames.Add(n.Replace(" (UnityEngine.AudioClip)", ""));
    //        }
    //        return BGMNames;
    //    }
    //    //[Resources/BGM]フォルダからBGMを探す

    //}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (list == null)//リストがnullだったら新しく生成
            list = new List<string>(Load());
        var buttonPos = position;
        buttonPos.xMax = buttonPos.xMax - 10;
        buttonPos.yMax = buttonPos.yMax - 15;
        buttonPos.y += 20;
        if (GUI.Button(buttonPos, "音源データベース更新"))
        {
            if (list == Load())//BGMが増えてなかったら更新しない
                return;
            else
                list = Load();
        }

        var selectedIndex = list.FindIndex(item => item.Equals(property.stringValue));
        if (selectedIndex == -1)
        {
            selectedIndex = list.FindIndex(item => item.Equals(list[0]));
        }

        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, list.ToArray());

        property.stringValue = list[selectedIndex];
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        return EditorGUIUtility.singleLineHeight * 3;
    }

    private List<string> Load()
    {


        List<string> BGMNames = new List<string>();
        //Resources/BGMからSceneのPathを読み込む  

        foreach (var s in Resources.LoadAll("Audio/BGM"))
        {
            string n = s.ToString();
            BGMNames.Add(n.Replace(" (UnityEngine.AudioClip)", ""));
        }
        return BGMNames;
    }

}
#endif



