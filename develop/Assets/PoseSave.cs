using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable/Pose")]
public class PoseSave : ScriptableObject
{
  public float[] poseDate=new float[95];
  public Vector3 pos;
  public Vector3 rot;
  public Vector3  transPos;
  public Quaternion quater;
}
