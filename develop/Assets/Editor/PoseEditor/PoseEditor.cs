using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PoseEditor : EditorWindow
{
    private const float WINDOUSIZE_W = 100.0f;
    private const float WINDOUSIZE_H = 100.0f;
    private PoseSave _save;
    private GameObject prefab;
    private PositionNames _names;
    private float[] muscles;
    private Animator _animator;
    private HumanPose _pose;
    private Parts _parts;
    private int _select;
    private Vector3 vec;
    private GUIStyle style;
    private RectOffset _offset;
    private Vector3 rot;
    private Transform _transform;
    private bool head, body, lArm, rArm, lHand, rHand, lLeg, rLeg = false;
    private bool lthumb, rthumb, lindex, rindex, lmiddle, rmiddle, lring, rring, llittle, rlittle = false;
    [MenuItem("Tool/tes")]
    private static void Open()
    {
        var window = GetWindow<PoseEditor>("View");
        window.minSize=new Vector2(WINDOUSIZE_W,WINDOUSIZE_H);
    }
    
    private void OnGUI()
    {
        _offset=new RectOffset(10,0,0,0);
        style=new GUIStyle(EditorStyles.foldout);
        
        style.fontSize = 20;
        style.margin = _offset;
        
       OpenPrefab();
       using (var change = new EditorGUI.ChangeCheckScope())
       {
           vec = EditorGUI.Vector3Field(new Rect(150.0f, 85.0f, 320.0f, 20.0f), "pos", vec);
           rot= EditorGUI.Vector3Field(new Rect(150.0f, 115.0f, 320.0f, 20.0f), "rot", rot);
           
           if (change.changed)
           {
               SetMuscle();
               SetAnim();
           }
       }
       
       if (GUI.Button(new Rect(150.0f, 45.0f, 120.0f, 20.0f), "保存") && prefab != null)
       {
           _save.poseDate = muscles;
           _save.pos = vec;
           _save.rot = rot;
           _save.transPos = _transform.localPosition;
           _save.quater = _transform.localRotation;
           Undo.RecordObject(_save,"MusclesSave");
           EditorUtility.SetDirty(_save);
           AssetDatabase.SaveAssets();
       }
       if (GUI.Button(new Rect(230.0f, 45.0f, 120.0f, 20.0f), "セーブ読み込み") && prefab != null)
       {
            muscles=_save.poseDate;
            prefab.transform.localPosition = _save.transPos;
            prefab.transform.localRotation = _save.quater;
            SetMuscle();
            SetAnim();
       }
       
       var parts = new[]
       {
           "頭", "胴", "左腕", "右腕", "左手", "右手", "左足", "右足"
       };
       _select = GUI.Toolbar(new Rect(0.0f, 80.0f, 450.0f, 20.0f), _select, parts);
       _parts = (Parts) _select;

       //Head();
       switch (_parts)
       {
           case Parts.頭:
               Head();
               break;
           case Parts.胴:
               Body();
               break;
           case Parts.左腕:
               LeftArm();
               break;
           case  Parts.右腕:
               RightArm();
               break;
           case Parts.右足:
               RightLeg();
               break;
           case Parts.左足:
               LeftLeg();
               break;
           case Parts.右手:
               RightHand();
               break;
           case  Parts.左手:
               LeftHand();
               break;
       }
    }
    
    
    private void Head()
    {
        GUILayout.Space(80);
        head = EditorGUILayout.Foldout(head, "頭",style);
        if (head)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
             GUILayout.Space(10);
             SliderEdit(PositionNames.首うなづき前後,PositionNames.顎左右);
             EditorGUI.indentLevel--;
        }
    }

    
    private void Body()
    {
        GUILayout.Space(80);
        body = EditorGUILayout.Foldout(body, "胴",style);
        if (body)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(10);
            SliderEdit(PositionNames.背骨前後,PositionNames.UpperChestTwistLeftRight);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
        }
    }
    private void LeftArm()
    {
        GUILayout.Space(80);
        
        lArm = EditorGUILayout.Foldout(lArm, "左腕",style);
        if (lArm)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(10);
            SliderEdit(PositionNames.左肩上げ下げ,PositionNames.左手首左右);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
        }
    }
    private void RightArm()
    {
        GUILayout.Space(80);
        
        rArm = EditorGUILayout.Foldout(rArm, "右腕",style);
        if (rArm)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(10);
            SliderEdit(PositionNames.右肩上げ下げ,PositionNames.右手首左右);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
        }
    }
    
    private void RightLeg()
    {
        GUILayout.Space(80);
        
        rArm = EditorGUILayout.Foldout(rArm, "右足",style);
        if (rArm)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(10);
            SliderEdit(PositionNames.右太もも前後,PositionNames.右足指先伸ばし);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
        }
    }
    private void LeftLeg()
    {
        GUILayout.Space(80);
        lLeg = EditorGUILayout.Foldout(lLeg, "左足",style);
        if (lLeg)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.左太もも前後,PositionNames.左足指先伸ばし);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
        }
    }
    private void LeftHand()
    {
        GUILayout.Space(80);
        lthumb = EditorGUILayout.Foldout(lthumb, "親指",style);
        if (lthumb)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.左親指付け根伸ばし,PositionNames.左親指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(5);
        }
        GUILayout.Space(10);
        lindex = EditorGUILayout.Foldout(lindex, "人差し指",style);
        if (lindex)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.左人差し指第3関節,PositionNames.左人差し指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
        GUILayout.Space(10);
        lmiddle = EditorGUILayout.Foldout(lmiddle, "中指",style);
        if (lmiddle)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.左中指第3関節,PositionNames.左中指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
        GUILayout.Space(10);
        lring = EditorGUILayout.Foldout(lring, "薬指",style);
        if (lring)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.左薬指第3関節,PositionNames.左薬指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
        GUILayout.Space(10);
        llittle = EditorGUILayout.Foldout(llittle, "小指",style);
        if (llittle)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.左小指第3関節,PositionNames.左小指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
    }
    private void RightHand()
    {
        GUILayout.Space(80);
        lthumb = EditorGUILayout.Foldout(lthumb, "親指",style);
        if (lthumb)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.右親指付け根伸ばし,PositionNames.右親指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
        GUILayout.Space(10);
        lindex = EditorGUILayout.Foldout(lindex, "人差し指",style);
        if (lindex)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.右人差し指第3関節,PositionNames.右人差し指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
        GUILayout.Space(10);
        lmiddle = EditorGUILayout.Foldout(lmiddle, "中指",style);
        if (lmiddle)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.右中指第3関節,PositionNames.右中指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
        GUILayout.Space(10);
        lring = EditorGUILayout.Foldout(lring, "薬指",style);
        if (lring)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.右薬指第3関節,PositionNames.右薬指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
        GUILayout.Space(10);
        llittle = EditorGUILayout.Foldout(llittle, "小指",style);
        if (llittle)
        {
            EditorGUI.indentLevel++;EditorGUI.indentLevel++;
            GUILayout.Space(5);
            SliderEdit(PositionNames.右小指第3関節,PositionNames.右小指第1関節);
            EditorGUI.indentLevel--;EditorGUI.indentLevel--;
            GUILayout.Space(10);
        }
    }
    private void OpenPrefab()
    {
        prefab=EditorGUILayout.ObjectField("Prefab",prefab,typeof(GameObject),true)as GameObject;
        _save=EditorGUILayout.ObjectField("save",_save,typeof(PoseSave),true)as PoseSave;
        if (GUI.Button(new Rect(5.0f, 45.0f, 120.0f, 20.0f), "読み込み")&& prefab!=null)
        {
            _animator = prefab.GetComponent<Animator>();
            _transform = prefab.transform;
            var view = SceneView.lastActiveSceneView;
            view.in2DMode = false;
            muscles=new float[95];
           var handrler=new HumanPoseHandler(_animator.avatar,_animator.transform);
           HumanPose pose=new HumanPose();
           handrler.GetHumanPose(ref pose);
           muscles = pose.muscles;
           vec = pose.bodyPosition;
           rot = pose.bodyRotation.eulerAngles;
        }
        
    }

    void SliderEdit(PositionNames s, PositionNames g)
    {
        using (var change = new EditorGUI.ChangeCheckScope())
        {
            for (int i = (int)s; i < (int)g+1; i++)
            {
                GUILayout.Space(10);
                muscles[i] =EditorGUILayout.Slider(((PositionNames)i).ToString(),muscles[i], -1, 1,GUILayout.Width(400));
            }
            if (change.changed)
            {
                SetMuscle();
                SetAnim();
            }
        }
    }
    private void SetMuscle()
    {
        _pose.muscles = muscles;
        
        _save.poseDate = muscles;
        _save.pos = vec;
        _save.rot = rot;
        Undo.RecordObject(_save,"MusclesSave");
        EditorUtility.SetDirty(_save);
       
    }

    private void SetAnim()
    {
        _pose.bodyPosition = _save.pos;
        _pose.bodyRotation = Quaternion.Euler(_save.rot);;
        var target=new HumanPoseHandler(_animator.avatar,_animator.transform);
        target.SetHumanPose(ref _pose);
        //target.GetHumanPose(ref _pose);
        //vec = _pose.bodyPosition;
    }
}
public enum  Parts
{ 
    頭, 胴, 左腕, 右腕, 左手, 右手, 左足, 右足
}