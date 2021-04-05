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

    private float speed;
    private Joystick joystick;

    public event Action<PlayerState> stateChanged;

    [SerializeField] private Bounds mapBounds;

    public void Initializie(float speed, Bounds bounds, Joystick joystick)
    {
        this.joystick = joystick;
        this.speed = speed;
        this.mapBounds = bounds;
        //TODO: refactoring
        mapBounds.Expand(new Vector3(1, 0, Mathf.Sqrt(3)) * -Map.hexRadius);
    }

    private void FixedUpdate()
    {
        Move(joystick.Direction);
        ClampPosition();
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

    private void ClampPosition()
    {
        Vector3 position = rigidbody.position;
        position.x = Mathf.Clamp(position.x, mapBounds.min.x, mapBounds.max.x);
        position.z = Mathf.Clamp(position.z, mapBounds.min.z, mapBounds.max.z);
        rigidbody.position = position;
    }

    public void DestinationReached() 
        => stateChanged?.Invoke(PlayerState.Win);
    public void Fall() 
        => stateChanged?.Invoke(PlayerState.Fall);

    public IEnumerator Winner(Action callback)
    {
        rigidbody.velocity = Vector3.zero;
        animator.SetTrigger("Win");
        yield return new WaitForSeconds(6);
        callback?.Invoke();
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
