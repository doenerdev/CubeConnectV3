using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using Facebook.Unity;
using Firebase.Database;

public class FirebaseAuthentication : Singleton<FirebaseAuthentication>
{
    public Text errorText;

    public const string DESKTOP_USER_ID = "XYYZ";
    public const string DESKTOP_USER_USERNAME = "DesktopUser";

    private FirebaseAuth _authenticationInstance;
    private FirebaseAuthenticationForm _currentAuthenticationForm;
    private FirebaseUserInfo _currentUserInfo;

    public FirebaseUserInfo CurrentUserInfo
    {
        get { return _currentUserInfo; }
    }

    private void Awake()
    {
        base.Awake();
        _authenticationInstance = FirebaseAuth.DefaultInstance;

        if (_authenticationInstance.CurrentUser != null)
        {
            //_currentUserInfo = new FirebaseUserInfo(_authenticationInstance.CurrentUser.UserId, _authenticationInstance.CurrentUser.Email, _authenticationInstance.CurrentUser.); //TODO
        }

        DontDestroyOnLoad(this);
    }

    public void RequireAuthenticationForm(Action<object> callback)
    {
        GameObject authFormGO = Instantiate(Resources.Load("Firebase/AuthenticationFormCanvas")) as GameObject;
        _currentAuthenticationForm = authFormGO.GetComponent<FirebaseAuthenticationForm>();
    }

    public bool IsUserLoggedIn()
    {
        if (_currentUserInfo == null || _currentUserInfo.FirebaseUser == null)
        {
            return false;
        }

        bool signedIn = _currentUserInfo.FirebaseUser != _authenticationInstance.CurrentUser && _authenticationInstance.CurrentUser != null;
        return (!signedIn && _currentUserInfo.FirebaseUser != null) == false && _currentUserInfo.FirebaseUser.IsEmailVerified;
    }

    public void LinkUsernameWithAuthAccount(string username, AuthentifciationType authentifciationType)
    {
        if (_currentUserInfo.AuthentifciationType != authentifciationType || string.IsNullOrEmpty(_currentUserInfo.UserID)) //early out
        {
            return;
        }

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Users").Child(_currentUserInfo.UserID).Child("Username").SetValueAsync(username).ContinueWith((Task task) =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                FB.LogOut();
                RaiseLogInCancled("Facebook Login cancled");
            }
            else if (task.IsCompleted)
            {
                _currentUserInfo = new FirebaseUserInfo(_currentUserInfo.UserID, _currentUserInfo.EmailAddress, username, _currentUserInfo.AuthentifciationType);
                RaiseLinkingUsernameWithExternalAuthenticationComplete("Username linking complete");
            }
        });
    }

    public void SignUp(string email, string username, string password)
    {
        bool faulted = false;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            //Error handling
            return;
        }

        _authenticationInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith((Task<FirebaseUser> task) =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                RaiseSignUpCancled("SignUp cancled");
                faulted = true;
                return;
            }

            FirebaseUser newUser = task.Result; // Firebase user has been created.
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("Users").Child(newUser.UserId).Child("Username").SetValueAsync(username).ContinueWith(usernameTask =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    RaiseSignUpCancled("Sing up cancled");
                    faulted = true;
                }
            });

            if (faulted) return;

            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

            FirebaseUser user = _authenticationInstance.CurrentUser;
            if (user != null)
            {
                user.SendEmailVerificationAsync().ContinueWith(verificationTask => {
                    if (verificationTask.IsCanceled || verificationTask.IsFaulted)
                    {
                        RaiseSignUpCancled("Sing up cancled");
                        faulted = true;
                    }
                    if (verificationTask.IsCompleted)
                    {
                        RaiseSignUpComplete("Sign Up Complete");                                                         
                    }
                });
            }
        });
    }

    public void LogIn(string email, string password)
    {
        _authenticationInstance.SignInWithEmailAndPasswordAsync(email, password).ContinueWith((Task<FirebaseUser> task) =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync canceled.");
                RaiseLogInCancled("Login cancled");
                return;
            }
            
            FirebaseUser user = task.Result;

            if (user.IsEmailVerified == false) //check if the email is verified
            {
                _authenticationInstance.SignOut();
                RaiseLogInCancled("Verify Email first");
                return;
            }

            _currentUserInfo = new FirebaseUserInfo(user.UserId, user.Email, null, AuthentifciationType.Facebook);
            RaiseLogInComplete(user.UserId);
        });
    }

    public void LogInWithFacebook()
    {
        FacebookManager.Instance.LogIn(FacebookLoginComplete);
    }

    public void FacebookLoginComplete(ILoginResult loginResult)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = AccessToken.CurrentAccessToken;
            Debug.Log(aToken.UserId);

            Credential credential = FacebookAuthProvider.GetCredential(aToken.TokenString);
            _authenticationInstance.SignInWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled || task.IsFaulted)
                {
                    errorText.text = "Login Failed";
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    FB.LogOut();
                    RaiseLogInCancled("Facebook Login cancled");
                    return;
                }

                if (task.IsCompleted)
                {
                    FirebaseUser newUser = task.Result;
                    _currentUserInfo = new FirebaseUserInfo(newUser.UserId, newUser.Email, null, AuthentifciationType.Facebook);

                    RaiseFacebookLogInComplete(newUser.UserId);
                    Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                    return;
                }
            });
        }
        else
        {
            errorText.text = "Login Failed";
            Debug.Log("User canceled login");
            RaiseLogInCancled("Facebook Login cancled");
        }
    }

    public void UserExistsInDB(string userID, Action<bool, DataSnapshot> callback)
    {
        Debug.Log("Checking Existance:" + userID);
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Ref" + reference);
        reference.Child("Users").Child(userID).GetValueAsync().ContinueWith((Task<DataSnapshot> task) =>
        {
            if (task.Result.Value != null)
            {
                Debug.Log("Exists");
                callback(true, task.Result);
            }
            else
            {
                Debug.Log("NOooo");
                callback(false, null);
            }
        });
    }

    public void RaiseFacebookLogInComplete(string userID)
    {
        EventHandler<EventUserIDArgs> handler = FacebookLogInComplete;
        if (handler != null)
        {
            handler(null, new EventUserIDArgs(userID));
        }
    }

    public void RaiseLogInComplete(string message)
    {
        EventHandler<EventTextArgs> handler = LogInComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    public void RaiseLogInCancled(string message)
    {
        EventHandler<EventTextArgs> handler = LogInCancled;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    public void RaiseLinkingUsernameWithExternalAuthenticationComplete(string message)
    {
        EventHandler<EventTextArgs> handler = LinkUsernameWithAuthAccountComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    public void RaiseSignUpCancled(string message)
    {
        EventHandler<EventTextArgs> handler = SignUpCancled;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    public void RaiseSignUpComplete(string message)
    {
        EventHandler<EventTextArgs> handler = SignUpComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    #region Events
    public event EventHandler<EventTextArgs> LinkUsernameWithAuthAccountComplete;
    public event EventHandler<EventUserIDArgs> FacebookLogInComplete;
    public event EventHandler<EventTextArgs> LogInComplete;
    public event EventHandler<EventTextArgs> LogInCancled;
    public event EventHandler<EventTextArgs> SignUpCancled;
    public event EventHandler<EventTextArgs> SignUpComplete;
    #endregion Events
}

public enum AuthenticationMessageType
{
    Error,
    Success
}