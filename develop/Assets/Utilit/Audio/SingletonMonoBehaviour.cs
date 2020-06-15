using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 使用するときはスクリプト内の注意事項を読むこと
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    //使用する際は継承先のAwakeに以下のコードを書き込むこと
    //  if (this != Instance)
    //    {
    //    Destroy(this);
    //    
    //    Debug.LogError(
    //       "継承先のスクリプト名" +
    //        " は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました." +
    //        " アタッチされているGameObjectは " + Instance.gameObject.name + " です.");
    //    return;
    //}
    private static T instance;
  
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "is nothing");
                }
            }

            return instance;
        }
    }
}
