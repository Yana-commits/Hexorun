using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    protected Joystick joystick;
    private Rigidbody rigidbody;
    public float speed;
   private Animator animator;
    public float xMin, xMax, zMin, zMax;
    void Start()
    {

        Init();
    }

    void FixedUpdate()
    {
        if (Controller.Instance.gameState == GameState.Play)
        {
            Vector3 velocity = new Vector3(joystick.Horizontal * speed, rigidbody.velocity.y, joystick.Vertical * speed);

            rigidbody.velocity = velocity;
            
            if (joystick.Direction.magnitude > 0)
                rigidbody.rotation = Quaternion.LookRotation(rigidbody.velocity, Vector3.up);
            
            rigidbody.position = new Vector3
                (
                Mathf.Clamp(rigidbody.position.x, xMin, xMax),
               rigidbody.position.y,
                Mathf.Clamp(rigidbody.position.z, zMin, zMax)
                );
            
            animator.SetFloat("Speed", Mathf.Abs(rigidbody.velocity.magnitude));
        }

        if (rigidbody.position.y <= -2)
        {
            Fall();
        }
    }

    public void Init()
    {
        joystick = FindObjectOfType<Joystick>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        speed = Controller.Instance.gameParameters.playerSpeed;
    }
    public void Win()
    {
        StartCoroutine(Winner());
    }
    private IEnumerator Winner()
    {
        Debug.Log("Win!");
        speed = 0;
        rigidbody.velocity = Vector3.zero;
        animator.SetTrigger("Win");
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Loose()
    {
        StartCoroutine(Looser());
    }
    private IEnumerator Looser()
    {
        Debug.Log("Loose!");
        speed = 0;
        animator.SetTrigger("Loose");
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Fall()
    {
        Debug.Log("Loose!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
