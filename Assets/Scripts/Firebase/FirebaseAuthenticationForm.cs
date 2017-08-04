using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facebook.Unity;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseAuthenticationForm : MonoBehaviour
{
    [Header("Overview Form")]
    [SerializeField] private GameObject _overviewForm;

    [Header("Login successfull")]
    [SerializeField] private GameObject _loginSuccessfullPanel;

    [Header("Signup successfull")]
    [SerializeField] private GameObject _signUpSuccessfullPanel;

    [Header("Loading")]
    [SerializeField] private GameObject _loadingPanel;

    [Header("Username linking")]
    [SerializeField] private GameObject _usernameRegistrationForm;
    [SerializeField] private FirebaseAuthenticationField _usernameRegistrationUsernamelInput;
    [SerializeField] private Button _usernameRegistrationRegisterButton;

    [Header("Email Login")]
    [SerializeField] private GameObject _emailLoginForm;
    [SerializeField] private FirebaseAuthenticationField _loginEmailInput;
    [SerializeField] private FirebaseAuthenticationField _loginPasswordInput;
    [SerializeField] private Text _loginErrorMessage;
    [SerializeField] private Button _loginButton;

    [Header("Email Registration")]
    [SerializeField] private GameObject _emailRegisterForm;
    [SerializeField] private FirebaseAuthenticationField _signUpEmailInput;
    [SerializeField] private FirebaseAuthenticationField _signUpUsernameInput;
    [SerializeField] private FirebaseAuthenticationField _signUpPasswordInput;
    [SerializeField] private FirebaseAuthenticationField _signUpConfirmPasswordInput;
    [SerializeField] private Button _signUpButton;
    [SerializeField] private Text _signUpErrorMessage;

    private AuthentifciationType _authentifciationType = AuthentifciationType.Firebase;

    public Text testText;

    private void Start () {
        _signUpButton.onClick.AddListener(() => SignUp());
        _loginButton.onClick.AddListener(() => LogInWithEmail());
        _usernameRegistrationRegisterButton.onClick.AddListener(() => LinkUsernameWithExternalAuthentication());

        FirebaseAuthentication.Instance.FacebookLogInComplete += new EventHandler<EventUserIDArgs>(FacebookLoginCompleteHandler);
        FirebaseAuthentication.Instance.LogInCancled += new EventHandler<EventTextArgs>(FacebookLoginCancledHandler);
        FirebaseAuthentication.Instance.LinkUsernameWithAuthAccountComplete += new EventHandler<EventTextArgs>(LinkUsernameWithAuthAccountCompleteHandler);
        FirebaseAuthentication.Instance.SignUpComplete += new EventHandler<EventTextArgs>(SignUpCompleteHandler);
        FirebaseAuthentication.Instance.SignUpCancled += new EventHandler<EventTextArgs>(SignUpCancledHandler);
        FirebaseAuthentication.Instance.LogInComplete += new EventHandler<EventTextArgs>(LoginCompleteHandler);
    }

    private void LinkUsernameWithExternalAuthentication()
    {
        testText.text = "Trying to register username " + FirebaseAuthentication.Instance.CurrentUserInfo.UserID + " " + FirebaseAuthentication.Instance.CurrentUserInfo.AuthentifciationType;
        if (_usernameRegistrationUsernamelInput.Validate())
        {
            ShowLoadingPanel();
            FirebaseAuthentication.Instance.LinkUsernameWithAuthAccount(_usernameRegistrationUsernamelInput.InputField.text, _authentifciationType);
        } 
    }

    private void LogInWithEmail()
    {
        if (_loginEmailInput.Validate() & _loginPasswordInput.Validate())
        {
            ShowLoadingPanel();
            FirebaseAuthentication.Instance.LogIn(_loginEmailInput.InputField.text, _loginPasswordInput.InputField.text);
        }
    }

    private void SignUp()
    {
        if (_signUpEmailInput.Validate() & _signUpUsernameInput.Validate() & _signUpPasswordInput.Validate() & _signUpConfirmPasswordInput.Validate())
        {
            ShowLoadingPanel();
            FirebaseAuthentication.Instance.SignUp(_signUpEmailInput.InputField.text, _signUpUsernameInput.InputField.text, _signUpPasswordInput.InputField.text);
        }
    }

    public void LoginWithFacebook()
    {
        _authentifciationType = AuthentifciationType.Facebook;
        FirebaseAuthentication.Instance.LogInWithFacebook();
        ShowLoadingPanel();
    }

    private void LinkUsernameWithAuthAccountCompleteHandler(object sender, EventTextArgs args)
    {
        ShowLoginSuccessfull();
    }

    private void FacebookLoginCompleteHandler(object sender, EventUserIDArgs args)
    {
        testText.text = args.UserID;
        FirebaseAuthentication.Instance.UserExistsInDB(args.UserID, (exists, data) =>
        {
            if (exists == false) //request a user name
            {
                ShowUsernameRegistrationForm();
            }
            else
            {
                ShowLoginSuccessfull();
            }
        });
    }

    private void FacebookLoginCancledHandler(object sender, EventTextArgs args)
    {

    }

    private void LoginCompleteHandler(object sender, EventTextArgs args)
    {
        ShowLoginSuccessfull();
    }

    private void SignUpCancledHandler(object sender, EventTextArgs args)
    {
        //ShowUsernameRegistrationForm();
    }

    private void SignUpCompleteHandler(object sender, EventTextArgs args)
    {
        ShowLoadingPanel();
    }

    public void ShowLoadingPanel()
    {
        HideAllPanels();
        _loadingPanel.SetActive(true);
    }

    public void ShowLoginSuccessfull()
    {
        HideAllPanels();
        _loginSuccessfullPanel.SetActive(true);
        Destroy(this.gameObject, 2f);
    }

    public void ShowSignUpSuccessfull()
    {
        HideAllPanels();
        _signUpSuccessfullPanel.SetActive(true);
        Invoke("ShowOverviewForm", 2f);
    }

    public void ShowEmailLoginForm()
    {
        _authentifciationType = AuthentifciationType.Firebase;
        HideAllPanels();
        _emailLoginForm.SetActive(true);
    }

    public void ShowRegisterForm()
    {
        _authentifciationType = AuthentifciationType.Firebase;
        HideAllPanels();
        _emailRegisterForm.SetActive(true);
    }

    public void ShowOverviewForm()
    {
        _authentifciationType = AuthentifciationType.Firebase;
        HideAllPanels();
        _overviewForm.SetActive(true);
    }

    public void ShowUsernameRegistrationForm()
    {
        HideAllPanels();
        _usernameRegistrationForm.SetActive(true);
    }

    public void HideAllPanels()
    {
        _loadingPanel.SetActive(false);
        _loginSuccessfullPanel.SetActive(false);
        _usernameRegistrationForm.SetActive(false);
        _overviewForm.SetActive(false);
        _emailLoginForm.SetActive(false);
        _emailRegisterForm.SetActive(false);
    }
}

public enum AuthentifciationType
{
    Firebase,
    Google,
    Facebook
}
