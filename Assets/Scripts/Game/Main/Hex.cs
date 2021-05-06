using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hex : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    public Renderer Renderer => _renderer;

    public HexState State { get;  set; } = HexState.None;
    public HexItem ItemState { get; set; } = HexItem.Empty;

    private Dictionary<HexState, float[]> hexPositionByState = new Dictionary<HexState, float[]>
    {
        [HexState.None] = new[] {0f},
        [HexState.Hill] = new[] {0.5f, 1f},
        [HexState.Hole] = new[] {-3f},
        [HexState.Disable] = new[] { -3f },
        [HexState.Zone] = new[] { 0f }
    };

    public Vector3Int index;
    public bool IsTarget;
    public bool safeZone =false; 

    public bool TrySetState(HexState state)
    {
        if (IsTarget ||safeZone|| State == HexState.Disable) return false;

        State = state;

        float y = hexPositionByState[State].Random();
        transform.DOLocalMoveY(y, 0.5f);

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsTarget && other.TryGetComponent<Player>(out var player))
        {
            player.TargetSpot();
            //player.CreatePass();
            //player.DestinationReached();
            this.enabled = false;
        }
    }
   
    public void DisableHex()
    {
        State = HexState.Disable;
        transform.DOLocalMoveY(-3f, 0.5f).OnComplete(()=> 
        { 
            
            this.gameObject.SetActive(false); 
        
        }); 
        //transform.DOMove(Vector3.zero,1f).on
    }

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    UnityEditor.Handles.Label(transform.position, $"{index}");
    //}
#endif

}