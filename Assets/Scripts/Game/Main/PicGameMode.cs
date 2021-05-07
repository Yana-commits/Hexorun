using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicGameMode : MonoBehaviour
{
   
    public GameModeState PicMode(GameModeState gameMode, int level)
    {
        
        switch (gameMode)
        {
            case GameModeState.Endless:
             return  GameModeState.Normal;
                
            case GameModeState.Arena:
             return  GameModeState.Normal;

            case GameModeState.Normal:
               
                return  ModeState(level);

            default:
                return  GameModeState.Normal;
        }
    }

    public GameModeState ModeState(int level)
    {
        GameModeState gameMode;

        if (level == 0 || level == 4 || level == 14)
        {
            gameMode = GameModeState.Endless;
        }
        else if (level == 1 || level == 9 || level == 19)
        {
            gameMode = GameModeState.Arena;
        }
        else
        {
            gameMode = GameModeState.Normal;
        }
        return gameMode;
    }
}
