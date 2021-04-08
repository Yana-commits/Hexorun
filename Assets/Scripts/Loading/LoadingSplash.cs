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
        var status = SceneManager.LoadSceneAsync(1);
        status.allowSceneActivation = false;

        yield return new WaitForSeconds(timer);
        yield return new WaitUntil(() => status.progress >= 0.9f);

        status.allowSceneActivation = true;
        yield return status;
    }
}
