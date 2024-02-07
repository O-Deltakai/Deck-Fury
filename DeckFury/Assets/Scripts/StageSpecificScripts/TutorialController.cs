using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    GameObject mainCameraObject;

    SceneLoader sceneLoader;

    public delegate void ChangeTutorialPhaseEvent(int phase);
    public event ChangeTutorialPhaseEvent OnChangeTutorialPhase;

    public delegate void ChangeDialogueIndex(int index);
    public event ChangeDialogueIndex OnChangeDialogue;

    public delegate void ReachedEndOfDialogueEvent();
    public event ReachedEndOfDialogueEvent OnReachEndOfDialogue;



    public List<float> cameraXPositionForPhase = new List<float>();

    public List<Vector3> cameraPositionForPhase = new List<Vector3>();

    public List<BoxCollider2D> phaseTransitionTriggers = new List<BoxCollider2D>();
    public int currentPhase = 0;

    [SerializeField] PlayerController player;
    [SerializeField] CardPoolManager cardPoolManager;
    [SerializeField] CardSelectionMenu cardSelectionMenu;
    [SerializeField] EnergyController energyController;
    [SerializeField] Button loadCardMagazineButton; 
    [SerializeField] GameObject TutorialDialogueBox;
    [SerializeField] TextMeshProUGUI tutorialDialogueText;
    [SerializeField] GameObject PressEnterToContinue;
    [SerializeField] GameObject PressBackspaceToRewind;

[Header("Tutorial Box Anchors")]
    [SerializeField] Vector2 cardSelectTutorialBoxAnchorPoint = new Vector2(588, 270);
    [SerializeField] Vector2 tutorialBoxStartingAnchor = new Vector2(-670, 157);
    [SerializeField] Vector2 belowMagazineAnchor;
    [SerializeField] Vector2 belowMagazineAnchorLeft;
    [SerializeField] Vector2 previewEnemyAnchor;
    [SerializeField] Vector2 dashTutorialAnchor = new Vector2(607, 376);
    [SerializeField] Vector2 phase5TutorialAnchor;

[Header("Dialogue Text")]
    [SerializeField] HintsContainerSO phase0Dialogue;
    [SerializeField] HintsContainerSO phase1Dialogue;
    [SerializeField] HintsContainerSO phase2Dialogue;
    [SerializeField] HintsContainerSO phase3Dialogue;
    [SerializeField] HintsContainerSO cardSelectionTutorialDialogue;
    [SerializeField] HintsContainerSO synergyTutorialDialogue;

    [SerializeField] HintsContainerSO phase4Dialogue;

    [SerializeField] AimpointController playerAimpointController;

[Header("Phase 2 Elements")]
    [SerializeField] StageEntity phase2TrainingRobot;
    [SerializeField] Image previewStageHighlighterBox;
    [SerializeField] Image magazineIndicatorHighlighterBox;


[Header("Phase 3 Elements")]
    [SerializeField] DeckSO phase3Deck;
    [SerializeField] DeckSO finishedTutorialDeck;
    [SerializeField] Image energyBarHighlighterBox;
    [SerializeField] GameObject previewStageButton;


    [Header("Phase 5 Elements")]
    [SerializeField] DeckSO phase5Deck;

    HintsContainerSO currentDialogue;

    int currentDialogueIndex = 0;

    [SerializeField] bool canCycleDialogue = true;
    bool finishedTutorial = false;

    IEnumerator WaitBeforeFindingSceneLoader()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        sceneLoader = GameManager.Instance.GetComponent<SceneLoader>();
        mainCameraObject = Cinemachine.CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.VirtualCameraGameObject;
        mainCameraObject.transform.position = new Vector3(0, 0.05f, -10);
    }

    private void Start() 
    {

        currentDialogue = phase0Dialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[0];
        TutorialDialogueBox.transform.localPosition = new Vector3(tutorialBoxStartingAnchor.x, tutorialBoxStartingAnchor.y, 0);
        TutorialDialogueBox.SetActive(false);

        playerAimpointController.gameObject.SetActive(false);
        player.CanFireBasicShot = false;

        cardSelectionMenu.BeginActivated = false;
        cardSelectionMenu.CanBeOpened = false;

        energyController.DisableEnergyBar();

        FocusModeController.Instance.HideFocusModeUI();

        PressBackspaceToRewind.SetActive(false);

        previewStageButton.SetActive(false);
        cardSelectionMenu.canUsePreviewButton = false;

        OnChangeTutorialPhase += TimerChangeTutorialPhase;
        StartCoroutine(WaitBeforeFindingSceneLoader());
    }

    IEnumerator FlashHighlighterBox(Image highlighterBox, int numOfFlashes)
    {
        int numberOfFlashes = numOfFlashes;

        for(int i = 0; i < numberOfFlashes; i++) 
        {
            highlighterBox.DOFade(1, 0.5f).SetUpdate(true);

            yield return new WaitForSecondsRealtime(0.5f);

            highlighterBox.DOFade(0, 0.5f).SetUpdate(true);

            yield return new WaitForSecondsRealtime(0.5f);

        }

      
    }

    public void NextTutorialDialogue()
    {
        if(!canCycleDialogue){return;}

        currentDialogueIndex++;
        if(currentDialogueIndex == currentDialogue.HintList.Length - 1)
        {
            OnReachEndOfDialogue?.Invoke();
        }
        if(currentDialogueIndex >= currentDialogue.HintList.Length)
        {
            currentDialogueIndex = currentDialogue.HintList.Length - 1;
            PressEnterToContinue.SetActive(false);
            
            OnChangeDialogue?.Invoke(currentDialogueIndex);
            return;
        }

        tutorialDialogueText.text = currentDialogue.HintList[currentDialogueIndex];
        PressEnterToContinue.SetActive(true);
        PressBackspaceToRewind.SetActive(true);


        if(currentDialogueIndex == currentDialogue.HintList.Length -1 )
        {
            PressEnterToContinue.SetActive(false);
        }        

        OnChangeDialogue?.Invoke(currentDialogueIndex);

    }

    public void PreviousTutorialDialogue()
    {
        if(!canCycleDialogue){return;}

        currentDialogueIndex--;
        if(currentDialogueIndex < 0)
        {
            currentDialogueIndex = 0;
            PressBackspaceToRewind.SetActive(false);
            return;
        }

        tutorialDialogueText.text = currentDialogue.HintList[currentDialogueIndex];
        PressBackspaceToRewind.SetActive(true);
        PressEnterToContinue.SetActive(true);


        if(currentDialogueIndex == 0)
        {
            PressBackspaceToRewind.SetActive(false);
        }        

    }

    public void SetTutorialDialogue(int index)
    {
        if(index < 0)
        {
            currentDialogueIndex = 0;
            tutorialDialogueText.text = currentDialogue.HintList[currentDialogueIndex];
            
            PressBackspaceToRewind.SetActive(false);
            PressEnterToContinue.SetActive(true);
        }else
        if(index > currentDialogue.HintList.Length - 1)
        {
            currentDialogueIndex = currentDialogue.HintList.Length - 1;
            tutorialDialogueText.text = currentDialogue.HintList[currentDialogueIndex];

            PressBackspaceToRewind.SetActive(true);
            PressEnterToContinue.SetActive(false);
        }else
        {
            currentDialogueIndex = index;
            tutorialDialogueText.text = currentDialogue.HintList[currentDialogueIndex];

        }

    }


    private void Update() 
    {
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            NextTutorialDialogue();
        }
        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            PreviousTutorialDialogue();
        }         
    }


    void InitiatePhase_0()//Movement tutorial
    {
        playerAimpointController.gameObject.SetActive(false);
        player.CanFireBasicShot = false;
        cardSelectionMenu.CanBeOpened = false;

        currentDialogueIndex = 0;
        currentDialogue = phase0Dialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[0];
    }

    void InitiatePhase_1()//Basic attack and aiming tutorial
    {
        currentDialogueIndex = 0;
        currentDialogue = phase1Dialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[0];
        playerAimpointController.gameObject.SetActive(true);
        player.CanFireBasicShot = true;
    }

    void InitiatePhase_2()//Card Selection and Card tutorial
    {

        currentDialogueIndex = 0;
        currentDialogue = phase2Dialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[0];
        canCycleDialogue = false;



        cardSelectionMenu.CanBeOpened = true;
        loadCardMagazineButton.gameObject.SetActive(false);

        cardSelectionMenu.OnMenuActivated += InitiateCardSelectTutorial;
        cardSelectionMenu.OnMenuDisabled += CloseCardSelectTutorial;

        PressEnterToContinue.gameObject.SetActive(false);
    }


    void InitiatePhase_3()//Combat and card synergy tutorial
    {

        //Move tutorial box into correct position
        TutorialDialogueBox.transform.DOLocalMove(
             new Vector3(cardSelectTutorialBoxAnchorPoint.x, cardSelectTutorialBoxAnchorPoint.y, 0),
            0.5f).SetUpdate(true).SetEase(Ease.InOutSine);   

        energyController.EnableEnergyBar();
        cardPoolManager.SetDefaultDeck(phase3Deck);

        cardSelectionMenu.OnMenuActivated -= InitiateCardSelectTutorial;
        cardSelectionMenu.OnMenuDisabled -= CloseCardSelectTutorial;

        cardSelectionMenu.OnMenuActivated += InitiateSynergyTutorial;
        cardSelectionMenu.OnMenuDisabled += CloseSynergyTutorial;
        loadCardMagazineButton.gameObject.SetActive(true);

        cardSelectionMenu.ActivateMenu();
        player.CurrentHP = 500;
        player.ShieldHP = 500;

        player.OnHPChanged += RegeneratePlayerHP;            
    }

    void InitiateSynergyTutorial()//Synergy tutorial for phase 3
    {
        canCycleDialogue = false;

        currentDialogueIndex = 0;
        currentDialogue = synergyTutorialDialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[0];

        PressEnterToContinue.SetActive(false);
        PressBackspaceToRewind.SetActive(false);

        cardSelectionMenu.OnSelectSpecificCard += ClickedCardSynergyTutorial;
        cardSelectionMenu.OnUnpreviewStage += TalkAboutSynergy;
        cardSelectionMenu.OnPreviewStage += TalkAboutEnemyDescriptionPanel;
        OnReachEndOfDialogue += EnableLoadMagazineButton;   

    }

    void TalkAboutEnemyDescriptionPanel()
    {
        TutorialDialogueBox.transform.DOLocalMove(
             new Vector3(previewEnemyAnchor.x, previewEnemyAnchor.y, 0),
            0.5f).SetUpdate(true).SetEase(Ease.InOutSine);          
        SetTutorialDialogue(3);        
    }

    void CloseSynergyTutorial()
    {
        OnReachEndOfDialogue -= EnableLoadMagazineButton; 
        TutorialDialogueBox.transform.DOLocalMove(
             new Vector3(cardSelectTutorialBoxAnchorPoint.x, cardSelectTutorialBoxAnchorPoint.y, 0),
            0.5f).SetUpdate(true).SetEase(Ease.InOutSine);


        canCycleDialogue = true;

        currentDialogueIndex = 0;
        currentDialogue = phase3Dialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[0];    
        StartCoroutine(FlashHighlighterBox(energyBarHighlighterBox, 3));

        PressEnterToContinue.SetActive(true);
        PressBackspaceToRewind.SetActive(false);

        //OnReachEndOfDialogue += DisableTutorialBox;
    }

    void ClickedCardSynergyTutorial(CardSO card)
    {
        if(finishedTutorial){return;}

        cardSelectionMenu.canUsePreviewButton = true;
        previewStageButton.SetActive(true);

        HighLightPreviewStageButton();

        SetTutorialDialogue(1);

        TutorialDialogueBox.transform.DOLocalMove(
             new Vector3(belowMagazineAnchor.x, belowMagazineAnchor.y, 0),
            0.5f).SetUpdate(true).SetEase(Ease.InOutSine);      

        cardSelectionMenu.OnSelectSpecificCard -= ClickedCardSynergyTutorial;
    }
    void TalkAboutSynergy()
    {
        TutorialDialogueBox.transform.DOLocalMove(
             new Vector3(cardSelectTutorialBoxAnchorPoint.x, cardSelectTutorialBoxAnchorPoint.y, 0),
            0.5f).SetUpdate(true).SetEase(Ease.InOutSine);          
        SetTutorialDialogue(2);
    }


    void InitiatePhase_4() //Tutorial for dashing
    {
        cardSelectionMenu.OnMenuActivated -= InitiateSynergyTutorial;
        cardSelectionMenu.OnMenuDisabled -= CloseSynergyTutorial;

        previewStageButton.SetActive(true);

        cardSelectionMenu.OnSelectSpecificCard -= ClickedCardSynergyTutorial;
        cardSelectionMenu.OnUnpreviewStage -= TalkAboutSynergy;

        currentDialogueIndex = 0;
        currentDialogue = phase4Dialogue;
        tutorialDialogueText.text = currentDialogue.HintList[0];    

        TutorialDialogueBox.transform.DOLocalMove(
             new Vector3(dashTutorialAnchor.x, dashTutorialAnchor.y, 0),
            0.5f).SetUpdate(true).SetEase(Ease.InOutSine);          

    }



    void RegeneratePlayerHP(int beforeDamageValue, int afterDamageValue)
    {
        if(afterDamageValue <= 150)

        player.ShieldHP = 500;   
        player.CurrentHP = 500;
    }
    void DisableTutorialBox()
    {
        TutorialDialogueBox.gameObject.SetActive(false);
        cardPoolManager.SetDefaultDeck(finishedTutorialDeck);
        finishedTutorial = true;
    }


    void InitiateCardSelectTutorial()//Card select tutorial for phase 2
    {
        canCycleDialogue = true;

        currentDialogueIndex = 0;
        currentDialogue = cardSelectionTutorialDialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[0];
        TutorialDialogueBox.SetActive(true);

        PressEnterToContinue.SetActive(true);
        PressBackspaceToRewind.SetActive(false);

        loadCardMagazineButton.gameObject.SetActive(true);

        OnReachEndOfDialogue += EnableLoadMagazineButton;

        //Move tutorial box into correct position
        TutorialDialogueBox.transform.DOLocalMove(
             new Vector3(cardSelectTutorialBoxAnchorPoint.x, cardSelectTutorialBoxAnchorPoint.y, 0),
            0.5f).SetUpdate(true).SetEase(Ease.InOutSine);    
        
        cardSelectionMenu.OnSelectSpecificCard += MoveBoxToMagazineAnchor;
    }
    void MoveBoxToMagazineAnchor(CardSO card)
    {
        TutorialDialogueBox.transform.DOLocalMove(
             new Vector3(belowMagazineAnchorLeft.x, belowMagazineAnchorLeft.y, 0),
            0.5f).SetUpdate(true).SetEase(Ease.InOutSine);    
        cardSelectionMenu.OnSelectSpecificCard -= MoveBoxToMagazineAnchor;
        SetTutorialDialogue(1);
    }
    void HighLightPreviewStageButton(int index)
    {
        if(index == 3)
        {
            StartCoroutine(FlashHighlighterBox(previewStageHighlighterBox, 3));
        }   
    }
    void HighLightPreviewStageButton()
    {
        StartCoroutine(FlashHighlighterBox(previewStageHighlighterBox, 3));  
    }

    void EnableLoadMagazineButton()
    {
        loadCardMagazineButton.gameObject.SetActive(true);
    }

    void CloseCardSelectTutorial()
    {
        OnReachEndOfDialogue -= EnableLoadMagazineButton;
        TutorialDialogueBox.SetActive(false);

        StartCoroutine(FlashHighlighterBox(magazineIndicatorHighlighterBox, 3));

        currentDialogueIndex = 1;
        currentDialogue = phase2Dialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[currentDialogueIndex];

        canCycleDialogue = false;
        PressEnterToContinue.SetActive(false);
        PressBackspaceToRewind.SetActive(false);

        phase2TrainingRobot.OnDestructionEvent += Phase2RobotDestroyed;
        OnChangeDialogue -= HighLightPreviewStageButton;


    }
    void Phase2RobotDestroyed(StageEntity entity, Vector3Int deathPosition)
    {
        currentDialogueIndex = 2;
        currentDialogue = phase2Dialogue;   
        tutorialDialogueText.text = currentDialogue.HintList[currentDialogueIndex];
    }

    void ChangeTutorialPhase(int phase)
    {
        switch (phase) {
            case 0:
                InitiatePhase_0();
            break;

            case 1:
                InitiatePhase_1();
            break;

            case 2:
                InitiatePhase_2();
            break;

            case 3:
                InitiatePhase_3();
            break;

            case 4:
                InitiatePhase_4();
            break;

            default :
                
                break;
        }

    }

    void TimerChangeTutorialPhase(int phase)
    {
        if(phase == 3)
        {
            print("Starting timer for phase 3");
            TutorialDialogueBox.gameObject.SetActive(false);
            StartCoroutine(TimerBeforeInitiatePhase(1f, phase));
            return;
        }

        ChangeTutorialPhase(phase);

    }
    IEnumerator TimerBeforeInitiatePhase(float duration, int phase)
    {
        yield return new WaitForSeconds(duration);
        ChangeTutorialPhase(phase);
        TutorialDialogueBox.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(!other.CompareTag("Player")){return;}
        
        currentPhase++;

        if(currentPhase >= 5)
        {
            sceneLoader.LoadScene(SceneNames.MainMenu.ToString());
            return;
        }

        print("trigger for phase: " + currentPhase);

        phaseTransitionTriggers[currentPhase].enabled = false;
        PressBackspaceToRewind.SetActive(false);
        PressEnterToContinue.SetActive(true);

        OnChangeTutorialPhase?.Invoke(currentPhase);

        mainCameraObject.transform.DOMove(cameraPositionForPhase[currentPhase], 1f).SetEase(Ease.InOutSine);

        //mainCamera.transform.DOMoveX(cameraXPositionForPhase[currentPhase], 1f).SetEase(Ease.InOutSine);   

    }


}
