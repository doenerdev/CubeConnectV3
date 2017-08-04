using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;

public class FirebaseUserInfo
{
    private string _userID;
    private string _emailAddress;
    private string _username;
    private AuthentifciationType _authentifciationType;
    private FirebaseUser _firebaseUser;

    public string UserID
    {
        get { return _userID; }
    }
    public string EmailAddress
    {
        get { return _emailAddress; }
    }
    public string Username
    {
        get { return _username; }
    }
    public AuthentifciationType AuthentifciationType
    {
        get { return _authentifciationType; }
    }
    public FirebaseUser FirebaseUser
    {
        get { return _firebaseUser; }
    }

    public FirebaseUserInfo() { }

    public FirebaseUserInfo(string userID, string emailAddress, string username, AuthentifciationType authentifciationType)
    {
        _userID = userID;
        _emailAddress = emailAddress;
        _username = username;
        _authentifciationType = authentifciationType;
    }

    public FirebaseUserInfo(string userID, string emailAddress, string username, AuthentifciationType authentifciationType, FirebaseUser user)
    {
        _userID = userID;
        _emailAddress = emailAddress;
        _username = username;
        _authentifciationType = authentifciationType;
        _firebaseUser = user;
    }

    public void SetFirebaseUser(FirebaseUser user)
    {
        _firebaseUser = user;
    }

    public bool IsLoggedIn()
    {
#if UNITY_EDITOR
        return true;
#endif
#if UNITY_ANDROID
        
#endif

        return false;
    }
}