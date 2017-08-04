using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuthenticationPasswordField : FirebaseAuthenticationField {

    [HideInInspector] public string AuthFieldMinRequierementNotMatched = "Password must be at least 6 characters";

    public override void SetErrorMessageText(AuthenticationFieldErrorType errorType = AuthenticationFieldErrorType.None)
    {
        switch (errorType)
        {
            case AuthenticationFieldErrorType.Required:
                _errorMessage.text = AuthFieldRequiredMessage;
                break;
            case AuthenticationFieldErrorType.MinimumRequirementNotMatched:
                _errorMessage.text = AuthFieldMinRequierementNotMatched;
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
                if (string.IsNullOrEmpty(_inputField.text))
                {
                    SetValid(false, AuthenticationFieldErrorType.Required);
                    return false;
                }

                SetValid(true);
                return true;
            case AuthenticationProcessType.SignUp:
                if (string.IsNullOrEmpty(_inputField.text))
                {
                    SetValid(false, AuthenticationFieldErrorType.Required);
                    return false;
                }

                if (_inputField.text.Length < 6)
                {
                    SetValid(false, AuthenticationFieldErrorType.MinimumRequirementNotMatched);
                    return false;
                }

                SetValid(true);
                return true;
        }

        SetValid(false);
        return false;
    }
}
