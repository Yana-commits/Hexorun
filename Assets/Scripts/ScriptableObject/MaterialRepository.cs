using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static MaterialRepository;

[CreateAssetMenu(menuName = "Repositories/Materials")]
public class MaterialRepository : ScriptableObject, IEnumerable<Data>, IReadOnlyList<Data>
{
    [Serializable]
    public struct Data
    {
        public string name;
        public Sprite preview;
        public Material main;
        public Material target;
        public Material plane;
    }

    [SerializeField]
    private List<Data> _materials;
    public IReadOnlyList<Data> Materials => _materials;

    public int Count => ((IReadOnlyCollection<Data>)_materials).Count;
    public Data this[int index] => ((IReadOnlyList<Data>)_materials)[index];
    public IEnumerator<Data> GetEnumerator() => ((IEnumerable<Data>)_materials).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_materials).GetEnumerator();
}

