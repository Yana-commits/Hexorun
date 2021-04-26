using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using static Hexagonal;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Map map;
    [SerializeField] private ObstacleGenerator obstacleGenerator;
    [SerializeField] private ObstaclePresenter obstaclePresenter;
    [SerializeField] GameObject pointsPrefab;
    public Map Map => map;

    public void Initialize(Transform _player, IShape shape ,GameParameters parameters)
    {
        map.Initializie(parameters.size, shape, parameters.theme);
        map.gameObject.SetActive(true);      
        obstacleGenerator.Initialize(_player.transform, shape, parameters.obstaclesParam);

        var list = Map.Shuffle().ToList();
        int index = 0; 
        foreach (var pair in parameters.collectableItems)
        {
            for (int i = 0; i < pair.Value; index++, i++)
            {
                GameObject star = Instantiate(pair.Key, list[index].transform);
                star.transform.localPosition = Vector3.up * 0.5f;
                list[index].ItemState = HexItem.Fill;
            }
        }
    }

    public void GeneratePointItem()
    {
        var list = map.Where(x => x.ItemState == HexItem.Empty).ToList();
        foreach (var item in list)
        {
            GameObject points = Instantiate(pointsPrefab, item.transform);
            points.transform.localPosition = Vector3.up * 0.1f;
            item.ItemState = HexItem.Fill;
        }
    }

    public void ChangeHexes(KindOfMapBehavor mapBehavor)
    {
        obstacleGenerator.Generate(mapBehavor);
    }

    public void ChangeChunkTheme(MaterialRepository.Data _data)
    {
        Map.SetThemeWithDelay(_data);
    }

}
