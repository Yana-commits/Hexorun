using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSplash : MonoBehaviour
{
    [SerializeField] float timer = 2f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        yield return new WaitForSeconds(timer);
        SceneManager.LoadScene(1);
    }
}
