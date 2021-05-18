using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Repositories/Skins")]
public class SkinRepository : ScriptableObject
{
    [SerializeField]
    private List<SkinList> skinLists;
    public int Count => skinLists.Count;
    public SkinList this[int index] => skinLists[index];
}

[Serializable]
public class SkinList
{
    public int id;
    public Sprite image;
    public Sprite stroke;
    public int openCoins;
}