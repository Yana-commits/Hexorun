using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hex : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    public Renderer Renderer => _renderer;

    public HexState State { get; private set; } = HexState.NONE;

    private Dictionary<HexState, float[]> hexPositionByState = new Dictionary<HexState, float[]>
    {
        [HexState.NONE] = new[] {0f},
        [HexState.UP] = new[] {0.5f, 1f},
        [HexState.DOWN] = new[] {-3f}
    };

    public Vector2Int index;
    public bool IsTarget;

    public bool TrySetState(HexState state)
    {
        if (IsTarget) return false;

        State = state;

        float y = hexPositionByState[State].Random();
        transform.DOLocalMoveY(y, 0.5f);

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsTarget && other.TryGetComponent<Player>(out var player))
        {
            player.DestinationReached();
            this.enabled = false;
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position, $"{index}");
    }
#endif

}