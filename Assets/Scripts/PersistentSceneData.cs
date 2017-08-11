using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentSceneData
{
    private static bool _bootstrappedApplication = false;

    public static bool BootstrappedApplication {
        get  {
            return _bootstrappedApplication;
        }
        set  {
        _bootstrappedApplication = value;
        }
    } 
    public static int CurrentStageIndex { get; set; }
    public static int CurrentLevelIndex { get; set; }
    public static MainMenu MainMenu { get; set; }
    public static GameObject MainMenuSceneContainer { get; set; }
    public static UserGeneratedLevelData CurrentUserGeneratedLevelData { get; set; }
}
