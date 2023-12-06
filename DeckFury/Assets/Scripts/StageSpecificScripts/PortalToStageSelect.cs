using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PortalToStageSelect : MonoBehaviour
{
    [SerializeField] StageStateController stageController;
    SceneLoader sceneLoader;

    public bool SkipScoringPanel = false;

    void Start() 
    {
        stageController = GameErrorHandler.NullCheck(stageController, "Stage State Controller");    
        
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(!other.CompareTag("Player")){return;}
        if(!StageStateController.Instance.SceneIsAdditive)
        {
            print("Current active scene was not loaded additively, cannot return to stage select.");
            return;
        }


        PlayerController player = GameManager.Instance.player;
        player.CanAct = false;
        player.CanInitiateMovementActions = false;
        player.GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
        StartCoroutine(DisablePlayerTimer(0.2f));

        print("Triggered portal");

        if(SkipScoringPanel)
        {
            stageController.ReturnToStageSelect();
        }else
        {
            stageController.EnterExitPortal();
        }


    }

    IEnumerator DisablePlayerTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        GameManager.Instance.player.worldTransform.gameObject.SetActive(false);
    }


}
