using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FirebaseAuthenticationField : MonoBehaviour
{
    [HideInInspector] public string AuthfieldDefaultErrorMessage = "Field is not valid";
    [HideInInspector] public string AuthFieldRequiredMessage = "Field cannot be empty";

    [SerializeField] protected InputField _inputField;
    [SerializeField] protected Text _errorMessage;
    [SerializeField] protected AuthenticationProcessType _authenticationProcessType;

    protected ColorBlock _defaultColorBlock;

    public InputField InputField
    {
        get { return _inputField; }
    }

    protected void Awake()
    {
        _defaultColorBlock = _inputField.colors;
    }

    protected void SetValid(bool valid, AuthenticationFieldErrorType errorType = AuthenticationFieldErrorType.None)
    { 
        if (valid == false)
        {
            ColorBlock colorBlock = _inputField.colors;
            colorBlock.normalColor = Color.red;
            _inputField.colors = colorBlock;
            SetErrorMessageText(errorType);
            _errorMessage.enabled = true;
        }
        else
        {
            _inputField.colors = _defaultColorBlock;
            _errorMessage.enabled = false;
        }
    }

    public virtual void SetErrorMessageText(AuthenticationFieldErrorType errorType = AuthenticationFieldErrorType.None)
    {
        switch (errorType)
        {
            case AuthenticationFieldErrorType.Required:
                _errorMessage.text = AuthFieldRequiredMessage;
                break;
            default:
                _errorMessage.text = AuthfieldDefaultErrorMessage;
                break;
        }
    }

    public abstract bool Validate();
}


public enum AuthenticationProcessType
{
    SignUp,
    LogIn
}

public enum AuthenticationFieldErrorType
{
    None,
    Required,
    AlreadyExists,
    ConfirmNotMatched,
    MinimumRequirementNotMatched
}