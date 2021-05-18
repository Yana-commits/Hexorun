using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Game.Main;

public class Player : MonoBehaviour
{
    private readonly string SpeedKey = "Speed";

   public Rigidbody rigidbody;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerSkinController secondAnimator;
    [SerializeField] private CinemachineVirtualCamera vcam;
    private CinemachineTransposer cineTransposer;

    public float speed;
    private Joystick joystick;
    private GameParameters parameters;
    public float passSpeed = 0.01f;
    public bool passKlue;
    public Vector3 thronePlace;
    private GameModeState gameState = GameModeState.NormalWithBonus;

    private float target = 40f;
    private float targetOffset = 2.5f;
    public float zoomSpeed = 3f;

    public bool isFastRun = false;

    private PlayerState playerState = PlayerState.None;

    public event Action<PlayerState> OnStateChanged;
    public event Action OnPassActivated;

    [SerializeField] private Bounds mapBounds;

    private string AnimatorTrigger
    {
        set
        {
            animator.SetTrigger(value);
            secondAnimator.SetTrigger(value);
        }
    }

    private void SetAnimatorFloat(string key, float value)
    {
        animator.SetFloat(key, value);
        secondAnimator.SetFloat(key, value);
    }

    private void Start()
    {
        cineTransposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
    }

    public void Initializie(Joystick joystick, GameParameters parameters)
    {
        this.joystick = joystick;
        this.parameters = parameters;
    }

    public void SetGamePlaySettings(float speed, Bounds bounds)
    {
        this.speed = speed;
        this.mapBounds = bounds;
        mapBounds.Expand(new Vector3(1, 0, Mathf.Sqrt(3)) * -Map.hexRadius);
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Playing)
        {
            if (!isFastRun)
            {
                Move(joystick.Direction);

            }
            else
            {
                MovePass(joystick.Direction);

            }
            ClampPosition();
        }
    }

    public void Move(Vector2 direction)
    {
        Vector3 velocity = new Vector3(direction.x, 0, direction.y) * speed;

        if (velocity.magnitude > 0)
            rigidbody.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        SetAnimatorFloat(SpeedKey, velocity.magnitude);
        velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = velocity;
    }
    public void MovePass(Vector2 direction)
    {
        Vector3 velocity = new Vector3(direction.x, 0, passSpeed) * speed;

        if (velocity.magnitude > 0)
            rigidbody.rotation = Quaternion.LookRotation(velocity, Vector3.up);

        SetAnimatorFloat("Pass", velocity.magnitude);

        velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = velocity;
    }

    private void ClampPosition()
    {
        Vector3 position = rigidbody.position;
        position.x = Mathf.Clamp(position.x, mapBounds.min.x, mapBounds.max.x);
        position.z = Mathf.Clamp(position.z, mapBounds.min.z, mapBounds.max.z);
        rigidbody.position = position;
    }

    public void TargetSpot()
    {
  
        OnPassActivated?.Invoke();
    }
    public void DestinationReached()
    {
        playerState = PlayerState.Win;
        OnStateChanged?.Invoke(playerState);
    }
  
    public void Fall()
    {
        playerState = isFastRun ? PlayerState.Fall : PlayerState.Fall;
        OnStateChanged?.Invoke(playerState);
    }
    public void StartPlaying()
    {
        playerState = PlayerState.Playing;
        this.enabled = true;
    }

    public IEnumerator PassFall(Action callback)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("111");
        callback?.Invoke();
    }

    public IEnumerator Winner(Action callback)
    {
        rigidbody.velocity = Vector3.zero;
        AnimatorTrigger = "Win";
        yield return new WaitForSeconds(6);
        callback?.Invoke();
    }

    public IEnumerator BigWinner(Action callback)
    {
        rigidbody.velocity = Vector3.zero;
        AnimatorTrigger = "Jump";

        transform.DOMove(thronePlace, 0.5f);
        transform.position = thronePlace;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(Zoom());
       
        yield return new WaitForSeconds(5f);
        Debug.Log("444");
        callback?.Invoke();
    }

    private IEnumerator Zoom()
    {
        float elapsedTime = 0;

        while (elapsedTime <= 4)
        {
            
            elapsedTime += Time.deltaTime;
            float fov = vcam.m_Lens.FieldOfView;
            vcam.m_Lens.FieldOfView = Mathf.Lerp(fov, target, zoomSpeed * Time.deltaTime);

            float offsetY = cineTransposer.m_FollowOffset.y;
            cineTransposer.m_FollowOffset.y = Mathf.Lerp(offsetY, targetOffset, zoomSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void StopPlayer()
    {
        playerState = PlayerState.None;
        rigidbody.velocity = Vector3.zero;
        animator.SetFloat(SpeedKey, 0);
    }

    public IEnumerator Looser(Action callback)
    {
        rigidbody.velocity = Vector3.zero;
        AnimatorTrigger = "Loose";
        yield return new WaitForSeconds(5);
        callback?.Invoke();
    }
    public IEnumerator Reload(Action callback)
    {
        yield return new WaitForSeconds(0.5f);
        callback?.Invoke();
    }
}
