using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelComplete : ResultPoPup
{

    public override void Initialize(int totalCoins, int currentCoins, int bestScore, int currentScore, SkinRepository skin)
    {
        base.Initialize(totalCoins, currentCoins, bestScore, currentScore, skin);
       
    }
}
