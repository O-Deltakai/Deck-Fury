using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    GameManager gameManager;
    SceneLoader sceneLoader;

    [SerializeField] Button tutorialButton;
    [SerializeField] Button playButton;
    [SerializeField] GameObject developerNote;

    void Start()
    {
        tutorialButton.interactable = false;
        playButton.interactable = false;
        developerNote.SetActive(false);
        StartCoroutine(WaitBeforeFindingGameManager());
    }

    IEnumerator WaitBeforeFindingGameManager()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        gameManager = FindObjectOfType<GameManager>();
        sceneLoader = gameManager.GetComponent<SceneLoader>();
        tutorialButton.interactable = true;
        playButton.interactable = true;
    }

    public void LoadTutorial()
    {
        sceneLoader.LoadScene(SceneNames.TutorialStage_New);
    }

    public void LoadDefaultBattleScene()
    {
        sceneLoader.LoadScene("DefaultBattleScene (Do not modify)");
    }

    public void LoadSceneByName(string sceneName)
    {
        sceneLoader.LoadScene(sceneName);
    }

    public void LoadStageSelect()
    {
        GameManager.Instance.NextSceneIsNotInBattle = true;
        sceneLoader.LoadScene(SceneNames.StageSelectionScene.ToString());
    }

    public void ShowDeveloperNote()
    {
        if(!GameManager.DeveloperNoteHasBeenShown)
        {
            developerNote.SetActive(true);
            GameManager.DeveloperNoteHasBeenShown = true;
        }else
        {
            LoadStageSelect();
        }


    }

    public void Options()
    {
        GameManager.Instance.PauseMenu.OpenOptionsExternal();
    }


}
