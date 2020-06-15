using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class JsonConvert
{
    public static string ToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }
    public static T FromJson<T>(string str)
    {
        return JsonUtility.FromJson<T>(str);
    }
    public static byte[] ToByte(object obj)
    {
        var str = ToJson(obj);
        byte[] dgram = Encoding.UTF8.GetBytes(str);
        return dgram;
    }
    public static T FromJsonByte<T>(byte[]dgram)
    {
        string tex = Encoding.UTF8.GetString(dgram);
        return FromJson<T>(tex);
    }
    public static void SaveFile<T>(string filePath, object obj)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(ToJson(obj));
            }
        }
    }
    public static object LoadFile<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {//ファイルがない場合FALSE.
            Debug.Log("FileEmpty!");
            T t = default(T);
            return t;
        }
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                T date = JsonUtility.FromJson<T>(sr.ReadToEnd());

                return date;

            }
        }
    }
}
