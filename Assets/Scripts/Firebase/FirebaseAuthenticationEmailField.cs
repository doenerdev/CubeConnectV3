using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using UnityEngine;

public class FirebaseAuthenticationEmailField : FirebaseAuthenticationField
{

    [HideInInspector] public string AuthFieldMinRequierementNotMatched = "Enter a valid email adress";

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

                if (Regex.IsMatch(_inputField.text,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase))
                {
                    SetValid(true);
                    return true;
                }
                else
                {
                    SetValid(false, AuthenticationFieldErrorType.MinimumRequirementNotMatched);
                    return false;
                }
        }

        SetValid(false);
        return false;
    }
}
