using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facebook.Unity;
using Firebase.Auth;
using UnityEngine;

public class FacebookManager : Singleton<FacebookManager>
{

    private void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    public void LogIn(FacebookDelegate<ILoginResult> authCallback)
    {
        var perms = new List<string>() { "public_profile" };
        FB.LogInWithReadPermissions(perms, authCallback);
    }
}
