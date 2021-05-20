using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaMode : Mode
{
    private Player player;
    private HUD hud;
    private Chunk chunk;

    [SerializeField] float crushTime = 6;

    private float generatorTime;
    private float changeTime;
    private bool isCrush = false;

    [SerializeField]
    private CinemachineVirtualCamera vcam1;
    [SerializeField]
    private CinemachineVirtualCamera dollyCam;

    public override void Initialized(Player _player,HUD hud)
    {
        player = _player;
        this.hud = hud;
        hud.SetArenaPanel();

        generatorTime = crushTime;
        changeTime = gameParameters.changesTime;

        chunk = Instantiate(chunkPrefab,this.transform);
        chunk.Initialize(player.transform, new HexShape(), gameParameters);
        chunk.GeneratePointItem();
        chunk.Map.SetArenaTarget();

        var startPosZone = new Vector2Int(0, -gameParameters.size.y + 1);
        chunk.Map.SetSafeZoneArena(startPosZone);
        var hex = chunk.Map[startPosZone];
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos, Quaternion.identity);
        player.SetGamePlaySettings(gameParameters.playerSpeed, chunk.Map.Bounds);
        player.gameObject.SetActive(true);
        StartCoroutine(DollyCam());
    }

    public IEnumerator DollyCam()
    {
        yield return new WaitForSeconds(1);
        dollyCam.gameObject.SetActive(true);
        yield return new WaitForSeconds(10);
        dollyCam.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameState.GamePlayState != GameplayState.Play)
            return;

        generatorTime -= Time.deltaTime;
        changeTime -= Time.deltaTime;

        if (generatorTime > 0)
        {
            if (changeTime <= 0)
            {
                ChangedHexState(KindOfMapBehavor.DiffMoove);
                changeTime = gameParameters.changesTime;
            }          
            CalculateCrush(); //update crush hud
        }
        else
        {
            if (!isCrush)
            {
                changeTime = gameParameters.changesTime;
                isCrush = true;
                ChangedHexState(KindOfMapBehavor.AllDown);
            }

            if (changeTime <= 0)
            {
                changeTime = gameParameters.changesTime;
                generatorTime = crushTime;
                isCrush = false;
                ChangedHexState(KindOfMapBehavor.DiffMoove);
            }
        }

    }

    public void CalculateCrush()
    {
        var val = generatorTime / crushTime;
        hud.UpDateCrashTimer(val,generatorTime);
    }
  
    public override void ChangedHexState(KindOfMapBehavor mapBehavor)
    {
        chunk.ChangeHexes(mapBehavor);
    }

}

