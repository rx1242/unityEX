using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
/// <summary>
/// CSVを読み取って　ステージを生成する
/// 
/// 制作　中野
/// </summary>
public class Create : EditorWindow
{
    MapchipObjectSet mapchip;
    int lineLength;//行
    int rowLength;//列
    int count;//設置個数
    GameObject[] mapChipObj;
    private string[,] csvDateArray;//これの二重配列に区切られたデータが格納される[行][列]で読み取れる         
    ReorderableList obj;
    bool node = false;

    GameObject wrapper;// 複製のためのラッパーオブジェクト これの子オブジェクトとして生成される
    Renderer rend;
    Renderer[] rends;
    [UnityEditor.MenuItem("Tools/Stagecreate ")]

    static void Init()
    {
        Create window = (Create)GetWindow(typeof(Create), true, "Duplicate Objects");//新しいウィンドウ作る
        window.Show();

    }

 
    
    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        ReadCsv();
        mapChipObj = new GameObject[9];
        rends = new Renderer[9];
        EditorGUILayout.LabelField("読み取ったCSVの行" + rowLength + "  列" + lineLength);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        mapchip = (MapchipObjectSet)EditorGUILayout.ObjectField("平原のオブジェクト", mapchip, typeof(MapchipObjectSet), true);
       
        
        EditorGUILayout.EndVertical();
        if (mapchip == null) return;
        objectlist();

        if (EditorGUI.EndChangeCheck())// 以前の変更ラッパーを破棄します・新しいものを作成する前にチェック
        {
            Rollback();
            Apply();
        }
        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        GUIStyle nodeStyle1 = node ? (GUIStyle)"flow node hex 2 on" : (GUIStyle)"flow node hex 2";
        GUIStyle nodeStyle2 = node ? (GUIStyle)"flow node hex 6 on" : (GUIStyle)"flow node hex 6";
        if (GUILayout.Button("　 決定 　", nodeStyle1)) Close();
        if (GUILayout.Button("キャンセル", nodeStyle2))//巻き戻して終了
        {
            Rollback();
            Close();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ReadCsv()//CSV読み取り
    {
        if (GUILayout.Button("読み込み"))//Editorウィンドウの読み込みボタンが押されたら
        {
            var path = EditorUtility.OpenFilePanel("aaa", "", "csv");//エクスプローラー開いてファイルのパスを取得

            if (path == "")
            {
                return;
            }
            StreamReader sr = new StreamReader(path);//読み込んだファイルをストリームに置き換える

            string strStream = sr.ReadToEnd();//ストリームをstringに変える

            System.StringSplitOptions options = System.StringSplitOptions.RemoveEmptyEntries; // StringSplitOptionを設定 "カンマとカンマに何もなかったら格納しないことにする"

            string[] line = strStream.Split(new char[] { '\r', '\n' }, options);//行で分ける

            char[] spilter = new char[1] { ',' };//区切る文字を設定　この場合カンマ区切りなのでカンマを設定する

            lineLength = line.Length;// 行の数設定
            rowLength = line[0].Split(spilter, options).Length;

            csvDateArray = new string[lineLength, rowLength];//データ格納用配列の初期化

            for (int i = 0; i < lineLength; i++)
            {
                string[] sDate = line[i].Split(spilter, options);
                for (int j = 0; j < rowLength; j++)
                {
                    csvDateArray[i, j] = sDate[j];
                }
            }
            Debug.Log(lineLength);
            Debug.Log(rowLength);
        }

    }
    private void objectlist()
    {
        mapChipObj[0] = mapchip.field;
        mapChipObj[1] = mapchip.forest;
        mapChipObj[2] = mapchip.water;
        mapChipObj[3] = mapchip.wilderness;
        mapChipObj[4] = mapchip.mountain;
        mapChipObj[5] = mapchip.dontEnter;
        mapChipObj[6] = mapchip.pBase;
        mapChipObj[7] = mapchip.pMainBase;
        mapChipObj[8] = mapchip.eBase;
       
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i] = mapChipObj[i].GetComponentInChildren<Renderer>();
        }
    }
    private void Apply()
    {
        wrapper = new GameObject("stage");//新しいラッパー これの子オブジェクトに生成されるぞ
        
        GameObject obj;
        Renderer rend;
        for (int i = 0; i <= lineLength; i++)
        {
            for (int j = 0; j < rowLength; j++)
            {
                int caseNum = int.Parse(csvDateArray[i, j]);
                obj = mapChipObj[caseNum];
                rend = obj.GetComponentInChildren<Renderer>();
                //GameObjectとしてクローンを作成
                GameObject clone = Instantiate(obj, new Vector3(0, 0, 0), obj.transform.rotation) as GameObject;
                //クローンの間隔等の設定

                Vector3 offset = new Vector3(rend.bounds.size.x * obj.transform.position.x + j,
                                            rend.bounds.size.y * obj.transform.position.y,
                                            rend.bounds.size.z *obj.transform.position.z + i)
                                            ;
                clone.transform.Translate(offset);//間隔設定を適用
                clone.transform.SetParent(wrapper.transform);
                count++;

            }


        }
    }
    private void Rollback()
    {
        DestroyImmediate(wrapper);//巻き戻し
    }
    void OnDestroy()
    {
        if (count == 0) Rollback();
    }
}


