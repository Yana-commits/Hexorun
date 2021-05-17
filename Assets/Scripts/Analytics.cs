using UnityEngine;
using System.Collections;
using Facebook.Unity;
public class Analytics : MonoBehaviour
{
    void Awake()
    {
        FB.Init(FBInitCallback);
    }
    private void FBInitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
    }
    public void OnApplicationPause(bool paused)
    {
        if (!paused)
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
        }
    }
}
