using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private readonly int SpeedKeyHash = Animator.StringToHash("Speed");

    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Animator animator;

    public float speed;
    private Joystick joystick;
    public float passSpeed = 0.01f;
    private bool passKlue = true;

    public event Action<PlayerState> stateChanged;
    private PlayerState playerState = PlayerState.None;

    public event Action forPass;

    [SerializeField] private Bounds mapBounds;

    public void Initializie(Joystick joystick)
    {
        this.joystick = joystick;
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
            if (passKlue)
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
        animator.SetFloat(SpeedKeyHash, velocity.magnitude);

        velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = velocity;
    }
    public void MovePass(Vector2 direction)
    {
        Vector3 velocity = new Vector3(direction.x, 0, passSpeed) * speed;

        if (velocity.magnitude > 0)
            rigidbody.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        animator.SetFloat(SpeedKeyHash, velocity.magnitude);

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

    public void DestinationReached()
    {
        playerState = PlayerState.Win;
        stateChanged?.Invoke(playerState);

    }
    public void CreatePass()
    {
        forPass?.Invoke();
        passKlue = false;
    }
    public void Fall()
    {
        playerState = PlayerState.Fall;
        stateChanged?.Invoke(playerState);
    }
    public void StartPlaying()
    {
        playerState = PlayerState.Playing;
        this.enabled = true;
    }


    public IEnumerator Winner(Action callback)
    {
        rigidbody.velocity = Vector3.zero;
        animator.SetTrigger("Win");
        yield return new WaitForSeconds(6);
        callback?.Invoke();
    }

    public void StopPlayer()
    {
        playerState = PlayerState.None;
        rigidbody.velocity = Vector3.zero;
        animator.SetFloat(SpeedKeyHash, 0);  
    }

    public IEnumerator Looser(Action callback)
    {
        rigidbody.velocity = Vector3.zero;
        animator.SetTrigger("Loose");
        yield return new WaitForSeconds(5);
        callback?.Invoke();
    }
    public IEnumerator FallDown(Action callback)
    {
        yield return new WaitForSeconds(0.5f);
        callback?.Invoke();
    }
}
