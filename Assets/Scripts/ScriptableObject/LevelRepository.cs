using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Repositories/Levels")]
public class LevelRepository : ScriptableObject, IEnumerable<GameParameters>, IReadOnlyList<GameParameters>
{
    [SerializeField]
    private List<GameParameters> _parameters;


    public IReadOnlyList<GameParameters> Parameters => _parameters;

    public GameParameters this[int index] => Parameters[index];
    public int Count => ((IReadOnlyCollection<GameParameters>)_parameters).Count;
    public IEnumerator<GameParameters> GetEnumerator() => ((IEnumerable<GameParameters>)_parameters).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_parameters).GetEnumerator();

}


public enum PatternEnum
{
    Wall1,
    Wall2,
    Wall3,
    Path1,
}