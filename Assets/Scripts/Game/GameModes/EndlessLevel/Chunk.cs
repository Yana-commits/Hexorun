using UnityEngine;
using System;
using System.Linq;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Map map;
    [SerializeField] private ObstacleGenerator obstacleGenerator;
    [SerializeField] private ObstaclePresenter obstaclePresenter;

    public Map Map => map;

    public void Initialize(Transform _player, GameParameters parameters)
    {
        map.Initializie(parameters.size, parameters.theme);
        map.gameObject.SetActive(true);
        obstacleGenerator.Initialize(_player.transform, parameters.obstaclesParam);

        var list = Map.Shuffle().ToList();
        int index = 0;

        foreach (var pair in parameters.collectableItems)
        {
            for (int i = 0; i < pair.Value; index++, i++)
            {
                GameObject star = Instantiate(pair.Key, list[index].transform);
                star.transform.localPosition = Vector3.up * 0.5f;
            }
        }
    }

    public void SubcribeOnChangedHex(Action OnChaned)
    {
        OnChaned += () => {
            obstacleGenerator.Generate();
        };
    }


}
