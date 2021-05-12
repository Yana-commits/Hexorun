using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Repositories/Modes")]
public class ModesRepository : ScriptableObject
{
    [SerializeField]
    private List<ModeList> modeLists;

    public int Count => modeLists.Count;
    public ModeList this[int index] => modeLists[index];
}

[Serializable]
public class ModeList
{
    public GameModeState mode;
    public GameParameters gameParams;
}