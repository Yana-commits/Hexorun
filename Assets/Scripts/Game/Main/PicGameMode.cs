using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicGameMode : MonoBehaviour
{
    private GameModeState gameMode;

   


    public GameModeState PicMode(GameModeState gameMode, int level)
    {
        switch (gameMode)
        {
            case GameModeState.Endless:
             return   gameMode = GameModeState.Normal;

            case GameModeState.Arena:
             return   gameMode = GameModeState.Normal;

            case GameModeState.Normal:

              return  gameMode = ModeState(level);

            default:
                return gameMode = GameModeState.Normal;
        }
    }

    public GameModeState ModeState(int level)
    {
        if (level == 1 || level == 5 || level == 15)
        {
            gameMode = GameModeState.Endless;
        }
        else if (level == 2 || level == 10 || level == 20)
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
