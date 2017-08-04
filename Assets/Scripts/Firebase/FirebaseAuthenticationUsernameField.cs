using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class FirebaseAuthenticationUsernameField : FirebaseAuthenticationField
{
    [HideInInspector] public string AuthFieldMinRequierementNotMatched = "Username must be at least 4 characters and must contain only alphanumeric characters";

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

                string pattern = @"^([a-zA-Z0-9]){4,}$";
                Regex regex = new Regex(pattern);
                if (regex.IsMatch(_inputField.text) == false)
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
