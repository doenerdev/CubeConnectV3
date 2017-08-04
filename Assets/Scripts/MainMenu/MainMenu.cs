using System;
using System.Collections;
using System.Collections.Generic;
using TagFrenzy;
using UnityEngine;

public class MainMenu : Singleton<MainMenu>
{

    private MainMenuStageAndLevelSelection _stageAndLevelSelection;
    private GameObject _mainMenuScreen;

    public MainMenuStageAndLevelSelection StageAndLevelSelection
    {
        get { return _stageAndLevelSelection; }
    }

    private void CreatedStageAndLevelDataSelection(object sender, EventTextArgs args)
    {
        RaiseCreatingMainMenuComplete("Creating MainMenu complete");
    }

    public static MainMenu Create(GameObject parentCanvas)
    {
        parentCanvas.AddComponent<MainMenu>();
        MainMenu mainMenu = parentCanvas.GetComponent<MainMenu>();

        mainMenu._stageAndLevelSelection = MainMenuStageAndLevelSelection.Create(MultiTag.FindGameObjectsWithTags("MainMenuCanvas")[0].transform);
        mainMenu._stageAndLevelSelection.CreatingStageAndLevelSelectionComplete += new EventHandler<EventTextArgs>(mainMenu.CreatedStageAndLevelDataSelection);

        mainMenu._mainMenuScreen = Instantiate(Resources.Load("MainMenu/MainMenuScreen")) as GameObject;
        mainMenu._mainMenuScreen.transform.SetParent(parentCanvas.transform, false);

        return mainMenu;
    }

    private void RaiseCreatingMainMenuComplete(string message)
    {
        EventHandler<EventTextArgs> handler = CreatingMainMenuComplete;
        if (handler != null)
        {
            handler(null, new EventTextArgs(message));
        }
    }

    #region Events
    public event EventHandler<EventTextArgs> CreatingMainMenuComplete;
    #endregion Events
}
