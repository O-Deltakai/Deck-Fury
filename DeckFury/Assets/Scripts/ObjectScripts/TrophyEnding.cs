using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TrophyEnding : MonoBehaviour
{


    [SerializeField] GameObject contextPopup;
    [SerializeField] GameObject VictoryScreen;
    [SerializeField] TextMeshProUGUI totalScoreText;

    public bool hasTriggered = false;

    private void Awake() 
    {
        contextPopup.SetActive(false);    
        
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            EndGame();
            contextPopup.SetActive(false);
        }        
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(hasTriggered){ return; }

        if (other.CompareTag(TagNames.Player.ToString()))
        {
            contextPopup.SetActive(true);
        }

    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag(TagNames.Player.ToString()))
        {
            contextPopup.SetActive(false);
        }            
    }


    public void MoveVictoryScreenIntoView()
    {
        RectTransform victoryScreenRect = VictoryScreen.GetComponent<RectTransform>();
        victoryScreenRect.DOLocalMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBounce).SetUpdate(true);
        if(PersistentLevelController.Instance)
        {
            totalScoreText.text = PersistentLevelController.Instance.PlayerData.CurrentScore.ToString();
        }
    }


    void EndGame()
    {
        hasTriggered = true;
        GameManager.Instance.player.CanAct = false;
        GameManager.Instance.player.CanInitiateMovementActions = false;
        MoveVictoryScreenIntoView();

    }

    public void ReturnToMainMenu(Button button)
    {
        
        GlobalPlayerStatsManager.AddToPlayerPrefStat(GlobalPlayerStatsManager.StatKey.NumberOfCompletedRuns, 1);

        GameManager.Instance.MainMenuButton(button);
    }


}
