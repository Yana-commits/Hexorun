using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Repositories/Levels")]
public class LevelRepository : ScriptableObject, IEnumerable<GameParameters>
{
    [SerializeField]
    private List<GameParameters> _parameters;
    public IReadOnlyList<GameParameters> Parameters => _parameters;

    public IEnumerator<GameParameters> GetEnumerator() => ((IEnumerable<GameParameters>)_parameters).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_parameters).GetEnumerator();
}
