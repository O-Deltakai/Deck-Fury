using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeStageButton : MonoBehaviour
{

    PersistentLevelController levelController;
    SceneLoader sceneLoader;
    SceneNames sceneName = SceneNames.PracticeStage;

    [SerializeField] GameObject descriptionPanel;


    // Start is called before the first frame update
    void Start()
    {
        levelController = PersistentLevelController.Instance;
        sceneLoader = GameManager.Instance.GetComponent<SceneLoader>();

    }


    public void EnterPracticeStage()
    {
        GameManager.Instance.NextSceneIsNotInBattle = false;
        sceneLoader.LoadSceneAdditive(sceneName.ToString());
    }

    public void ToggleDescriptionPanel(bool condition)
    {
        if(condition)
        {
            descriptionPanel.SetActive(true);
        }else
        {
            descriptionPanel.SetActive(false);
        }
    }


}
