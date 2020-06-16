using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PoseEditor : EditorWindow
{
    private const float WINDOUSIZE_W = 100.0f;
    private const float WINDOUSIZE_H = 100.0f;
    private GUISkin m_skin;
    private GUIStyle m_style;
    private int m_index;
    private GUIStyle style;
    private Vector2 lh_ScrollPos = Vector2.zero;
    private Vector2 rh_ScrollPos = Vector2.zero;
     
    private PoseSave _save;
    private GameObject prefab;
    private PositionNames _names;
    private float[] muscles;
    private Animator _animator;
    private HumanPose _pose;
    private Parts _parts;
    private int _select;
    private Vector3 vec;
   
    private RectOffset _offset;
    private Vector3 rot;
    private Transform _transform;
    
    private bool head, body, lArm, rArm, lHand, rHand, lLeg, rLeg = false;
    private bool lthumb, rthumb, lindex, rindex, lmiddle, rmiddle, lring, rring, llittle, rlittle = false;
    
    [MenuItem("Tools/PoseEditor")]
    private static void Open()
    {
        var window = GetWindow<PoseEditor>("Pose");
        window.minSize=new Vector2(WINDOUSIZE_W,WINDOUSIZE_H);
    }

    private void OnEnable()
    {
        var path = "Assets/Editor/Texture/MySkin.guiskin";
        m_skin = AssetDatabase.LoadAssetAtPath<GUISkin>( path );
        m_style = m_skin.GetStyle( "Tab" );
        _offset=new RectOffset(10,0,0,0);
        
    }
    private void OnGUI()
    {
        style=new GUIStyle(EditorStyles.foldout);
        style.fontSize = 20;
        style.margin = _offset;
        GUI.skin = m_skin;
        var tabs = new [] { "読み込み", "編集", "プリセット" };
        m_index = GUILayout.Toolbar( m_index, tabs, m_style );
        switch (m_index)
        {
            case 0:
                OpenPrefab();
                break;
            case 1:
                Edit();
                break;
            case 2:
                break;
        }
    }
    private void EditLocalTransfrme()
    {
        using (var change = new EditorGUI.ChangeCheckScope())
        {
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                vec = EditorGUILayout.Vector3Field( "ポジション", vec);
                rot = EditorGUILayout.Vector3Field( "回転", rot);
            }
            if (change.changed)
            {
                SetMuscle();
                SetAnim();
            }
        }
    }
    private void Save()
    {
        if (GUILayout.Button("保存") && prefab != null)
        {
            _save.poseDate = muscles;
            _save.pos = vec;
            _save.rot = rot;
            _save.transPos = _transform.localPosition;
            _save.quater = _transform.localRotation;
            Undo.RecordObject(_save, "MusclesSave");
            EditorUtility.SetDirty(_save);
            AssetDatabase.SaveAssets();
        }
    }

    private void Symmetrical()
    {
        switch (_parts)
        {
            case Parts.左腕:
                if (GUILayout.Button("右腕を反転コピー") && prefab != null)
                {
                    Copy(PositionNames.左肩上げ下げ,PositionNames.左手首左右,9);
                }
                break;
            case Parts.右腕:
                if (GUILayout.Button("左腕を反転コピー") && prefab != null)
                {
                    Copy(PositionNames.右肩上げ下げ,PositionNames.右手首左右,-9);
                }
                break;
            case Parts.右足:
                if (GUILayout.Button("左足を反転コピー") && prefab != null)
                {
                    
                    Copy(PositionNames.右太もも前後,PositionNames.右足指先伸ばし,-8);
                }
                break;
            case Parts.左足 :
                if (GUILayout.Button("右足を反転コピー") && prefab != null)
                {
                    Copy(PositionNames.左太もも前後,PositionNames.左足指先伸ばし,8);
                }
                break;
            case Parts.左手:
                GUILayout.Space(10);
                using (new GUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("右親指を反転コピー ") && prefab != null)
                        {
                            Copy(PositionNames.左親指付け根伸ばし,PositionNames.左親指第1関節,20);
                        }
                        if (GUILayout.Button("右人差し指を反転コピー") && prefab != null)
                        {
                            Copy(PositionNames.左人差し指第3関節,PositionNames.左人差し指第1関節,20);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("右中指を反転コピー") && prefab != null)
                        {
                            Copy(PositionNames.左中指第3関節,PositionNames.左中指第1関節,20);
                        }
                        if (GUILayout.Button("右薬指を反転コピー") && prefab != null)
                        {
                            Copy(PositionNames.左薬指第3関節,PositionNames.左薬指第1関節,20);
                        }    
                    }
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("右小指を反転コピー") && prefab != null)
                    {
                        Copy(PositionNames.左小指第3関節,PositionNames.左小指第1関節,20);
                    }
                }
                break;
            case Parts.右手:
                GUILayout.Space(10);
                using (new GUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("左親指を反転コピー ") && prefab != null)
                        {
                            Copy(PositionNames.右親指付け根伸ばし,PositionNames.右親指第1関節,-20);
                        }
                        if (GUILayout.Button("左人差し指を反転コピー") && prefab != null)
                        {
                            Copy(PositionNames.右人差し指第3関節,PositionNames.右人差し指第1関節,-20);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("左中指を反転コピー") && prefab != null)
                        {
                            Copy(PositionNames.右中指第3関節,PositionNames.右中指第1関節,-20);
                        }
                        if (GUILayout.Button("左薬指を反転コピー") && prefab != null)
                        {
                            Copy(PositionNames.右薬指第3関節,PositionNames.右薬指第1関節,-20);
                        }    
                    }
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("左小指を反転コピー") && prefab != null)
                    {
                        Copy(PositionNames.右小指第3関節,PositionNames.右小指第1関節,-20);
                    }
                }
                break;
        }
    }

    private void Copy(PositionNames s,PositionNames e,int difference)
    {
        for (int i = (int) s; i < (int) e + 1; i++)
        {
            muscles[i] = muscles[i + difference];
        }
        SetMuscle();
        SetAnim();
    }

    private void Edit()
    {
        var parts = new[]
        {
            "頭", "胴", "左腕", "右腕", "左手", "右手", "左足", "右足"
        };
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            Save();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("編集部位選択");
            _select = GUILayout.Toolbar( _select, parts,m_style);
        }
        GUILayout.Space(10);
        
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditLocalTransfrme();
            _parts = (Parts) _select;
            switch (_parts)
                {
                    case Parts.頭:
                        Head();
                        break;
                    case Parts.胴:
                        Body();
                        break;
                    case Parts.左腕:
                        Symmetrical();
                        LeftArm();
                        break;
                    case Parts.右腕:
                        Symmetrical();
                        RightArm();
                        break;
                    case Parts.右足:
                        Symmetrical();
                        RightLeg();
                        break;
                    case Parts.左足:
                        Symmetrical();
                        LeftLeg();
                        break;
                    case Parts.右手:
                        Symmetrical();
                        RightHand();
                        break;
                    case Parts.左手:
                        Symmetrical();
                        LeftHand();
                        break;
                }
            GUILayout.Space(10);
        }
    }
    private void Head()
    {
        GUILayout.Space(20);
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
        GUILayout.Space(20);
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
        GUILayout.Space(20);
        
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
        GUILayout.Space(20);
        
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
        GUILayout.Space(20);
        
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
        GUILayout.Space(20);
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
        GUILayout.Space(20);
        lh_ScrollPos = EditorGUILayout.BeginScrollView(lh_ScrollPos);
        {
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
        EditorGUILayout.EndScrollView();
    }
    private void RightHand()
    {
        GUILayout.Space(20);
        rh_ScrollPos = EditorGUILayout.BeginScrollView(rh_ScrollPos);
        {
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
        EditorGUILayout.EndScrollView();
    }
    private void OpenPrefab()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Space(10);
            prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;
            GUILayout.Space(10);
            _save = EditorGUILayout.ObjectField("save", _save, typeof(PoseSave), true) as PoseSave;
            GUILayout.Space(10);
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                if (GUILayout.Button("読み込み") && prefab != null)
                {
                    _animator = prefab.GetComponent<Animator>();
                    _transform = prefab.transform;
                    // var view = SceneView.lastActiveSceneView;
                    // view.in2DMode = false;
                    muscles = new float[95];
                    var handrler = new HumanPoseHandler(_animator.avatar, _animator.transform);
                    HumanPose pose = new HumanPose();
                    handrler.GetHumanPose(ref pose);
                    muscles = pose.muscles;
                    vec = pose.bodyPosition;
                    rot = pose.bodyRotation.eulerAngles;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("セーブ読み込み") && prefab != null)
                {
                    muscles = _save.poseDate;
                    prefab.transform.localPosition = _save.transPos;
                    prefab.transform.localRotation = _save.quater;
                    SetMuscle();
                    SetAnim();
                }
            }
            
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
    }
}
public enum  Parts
{ 
    頭, 胴, 左腕, 右腕, 左手, 右手, 左足, 右足
}