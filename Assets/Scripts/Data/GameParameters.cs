using System;
using UnityEngine.Events;

[Serializable]
public class GameParameters
{
    public int duration;
    public float playerSpeed;
    public float changesTime;
    public int areaFactor;
    public bool isCameraOrthographic;
    public int holes;
}

[Serializable]
public class StartGameEvent : UnityEvent<GameParameters>
{ }