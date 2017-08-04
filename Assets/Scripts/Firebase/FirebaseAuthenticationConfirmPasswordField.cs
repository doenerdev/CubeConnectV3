using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuthenticationConfirmPasswordField : FirebaseAuthenticationField
{

    [SerializeField] private FirebaseAuthenticationField _masterPasswordField;

    [HideInInspector] public string AuthFieldConfirmNotMatched = "Passwords do not match";

    public override void SetErrorMessageText(AuthenticationFieldErrorType errorType = AuthenticationFieldErrorType.None)
    {
        switch (errorType)
        {
            case AuthenticationFieldErrorType.Required:
                _errorMessage.text = AuthFieldRequiredMessage;
                break;
            case AuthenticationFieldErrorType.ConfirmNotMatched:
                _errorMessage.text = AuthFieldConfirmNotMatched;
                break;
            default:
                _errorMessage.text = AuthfieldDefaultErrorMessage;
                break;
        }
    }

    public override bool Validate()
    {
        switch (_authenticationProcessType)
        {
            case AuthenticationProcessType.LogIn:
                SetValid(true);            
                return true;
            case AuthenticationProcessType.SignUp:
                if (string.IsNullOrEmpty(_inputField.text))
                {
                    SetValid(false, AuthenticationFieldErrorType.Required);
                    return false;
                }

                if (_inputField.text != _masterPasswordField.InputField.text)
                {
                    SetValid(false, AuthenticationFieldErrorType.ConfirmNotMatched);
                    return false;
                }

                SetValid(true);
                return true;
        }

        SetValid(false);
        return false;
    }
}
