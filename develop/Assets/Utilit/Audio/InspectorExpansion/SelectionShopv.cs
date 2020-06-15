using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
#endif
/// <summary>
/// インスペクターからResources/SE内のaudioclipをpopupで選択できる拡張
/// stringのフィールにattribute SelectBGMをつけて使用する
/// /// </summary>
class SelectionShopvAttribute : PropertyAttribute { }
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SelectionShopvAttribute))]

public class SelectionShopv : PropertyDrawer
{
    List<string> list;
    //List<string> AllSEName
    //{
    //    get
    //    {
    //        List<string> SENames = new List<string>();


    //        foreach (var s in Resources.LoadAll("Audio/SE"))
    //        {
    //            string n = s.ToString();
    //            SENames.Add(n.Replace(" (UnityEngine.AudioClip)", ""));
    //        }
    //        return SENames;
    //    }
    //    //[Resources/SE]フォルダからBGMを探す

    //}
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (list == null)//リストがnullだったら新しく生成
            list = new List<string>(Load());
        var buttonPos = position;
        buttonPos.xMax = buttonPos.xMax - 10;
        buttonPos.yMax = buttonPos.yMax - 15;
        buttonPos.y += 20;
        buttonPos.height -= 15;
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

        return EditorGUIUtility.singleLineHeight * 4;
    }
    private List<string> Load()
    {
        List<string> BGMNames = new List<string>();
        //Resources/BGMからSceneのPathを読み込む  

        foreach (var s in Resources.LoadAll("Audio/Shop"))
        {

            BGMNames.Add(s.name);
        }
        return BGMNames;
    }
}

#endif



