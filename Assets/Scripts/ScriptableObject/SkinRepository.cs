using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Repositories/Skins")]
public class SkinRepository : ScriptableObject
{
    [SerializeField]
    private List<SkinList> skinLists;
}

[Serializable]
public class SkinList
{
    public int id;
    public Sprite image;
    public Sprite stroke;
    public int openCoins;
}