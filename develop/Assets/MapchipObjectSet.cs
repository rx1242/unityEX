using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "MyGame/MapChip", fileName = "Mapchipset")]
public class MapchipObjectSet : ScriptableObject
{

    public GameObject field;
    public GameObject forest;
    public GameObject water;
    public GameObject wilderness;
    public GameObject dontEnter;
    public GameObject mountain;
    public GameObject pBase;
    public GameObject pMainBase;
    public GameObject eBase;

}
