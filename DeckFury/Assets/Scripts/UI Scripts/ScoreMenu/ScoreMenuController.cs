using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Linq;


public class ScoreMenuController : MonoBehaviour
{
    RectTransform rectTransform;
    SpawnManager spawnManager;
    [SerializeField] ScoreManager scoreManager;

    [SerializeField] Vector3 InViewAnchor;
    [SerializeField] Vector3 OutOfViewAnchor;

    [SerializeField] GameObject scoringPanel;

    [SerializeField] List<GameObject> orderOfElementsToShow;
    [SerializeField] float delayBetweenShowingScores = 0.5f;
    [SerializeField] float delayBetweenShowingBonuses = 0.2f;


[Header("Damage Taken Element")]
    [SerializeField] GameObject damageTakenElement;
    [SerializeField] TextMeshProUGUI damageTakenValue;
    [SerializeField] TextMeshProUGUI damageTakenScore;

[Header("Time Taken Element")]
    [SerializeField] GameObject timeTakenElement;
    [SerializeField] TextMeshProUGUI timeTakenValue;
    [SerializeField] TextMeshProUGUI timeTakenScore;


[Header("Enemies Defeated Element")]
    [SerializeField] GameObject enemiesDefeatedElement;
    [SerializeField] TextMeshProUGUI enemiesDefeatedValue;
    [SerializeField] TextMeshProUGUI enemiesDefeatedScore;    

[Header("Total Score Elements")]
    [SerializeField] GameObject stageScoreElement;
    [SerializeField] TextMeshProUGUI stageScoreValue;    

    [SerializeField] GameObject totalScoreElement;
    [SerializeField] TextMeshProUGUI totalScoreValue;


[Header("Special Bonuses Elements")]
    [SerializeField] GameObject specialBonusesPanel;
    [SerializeField] GameObject bonusRewardsParent;
    [SerializeField] GameObject bonusSlotPrefab;
    [SerializeField] List<SpecialBonusSlot> specialBonusSlots;


    Coroutine CR_ScorePresentation = null;
    Coroutine CR_BonusPresentation = null;

    bool bonusPanelIsOpen = false;
    bool hasPresentedBonuses = false;
    bool finishedPresentingScore = false;

    private void Awake() 
    {
        if(!scoreManager){ scoreManager = GetComponent<ScoreManager>(); }
        scoreManager = GameErrorHandler.NullCheck(scoreManager, "ScoreManager");

        scoreManager.OnCalculatedFinalScore += UpdateGenericScores;
        scoreManager.OnCheckedSpecialBonuses += PrepareSpecialBonuses;

        rectTransform = GetComponent<RectTransform>();

        totalScoreElement.SetActive(false);


    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveIntoView()
    {
        scoringPanel.transform.DOLocalMove(InViewAnchor, 0.25f).SetUpdate(true);
        Invoke(nameof(StartScorePresentation), 0.25f);
        GameManager.currentGameState = GameManager.GameState.InMenu;
        Cursor.visible = true;
    }

    public void MoveOutOfView()
    {
        scoringPanel.transform.DOLocalMove(OutOfViewAnchor, 0.25f).SetUpdate(true);
        
    }


    void UpdateGenericScores()
    {
        SetDamageTakenText(scoreManager.HPDamageTaken, scoreManager.CalculateHPDamageTakenScore());
        SetTimeTakenText(scoreManager.TimeSpentOnStage, scoreManager.CalculateTimeTakenScore(scoreManager.TimeSpentOnStage));
        SetEnemiesDefeatedText(scoreManager.TotalEnemiesKilled, scoreManager.EnemiesKilledScore);
        SetStageScoreText(scoreManager.StageScoreTotal);
    }

    public void ContinueButton()
    {
        if(!finishedPresentingScore)
        {
            SkipScorePresentation();
            finishedPresentingScore = true;
        }else
        if(specialBonusSlots.Count > 0 && !bonusPanelIsOpen && !hasPresentedBonuses)
        {
            SkipScorePresentation();
            finishedPresentingScore = true;
            StartBonusPresentation();
        }else
        if(bonusPanelIsOpen && !hasPresentedBonuses)
        {
            SkipBonusPresentation();
        }
        else
        if(bonusPanelIsOpen && hasPresentedBonuses)
        {
            specialBonusesPanel.SetActive(false);
            scoreManager.AddBonusRewardsToScore();

            SetStageScoreText(scoreManager.StageScoreTotal);

            scoreManager.AddScoreToPlayerData(StageStateController.Instance.PlayerData);

            SetTotalScoreText(StageStateController.Instance.PlayerData.CurrentScore);
            totalScoreElement.SetActive(true);

            bonusPanelIsOpen = false;
            
        }else
        {
            if(StageStateController.Instance.isFinalStage)
            {
                StageStateController.Instance.GoToEndingStage();
            }else
            {
                StageStateController.Instance.ReturnToStageSelect();
            }
        }



    }

    void SkipScorePresentation()
    {
        if(CR_ScorePresentation != null)
        {
            StopCoroutine(CR_ScorePresentation);
            CR_ScorePresentation = null;
        }

        foreach(GameObject uiElement in orderOfElementsToShow)
        {
            uiElement.SetActive(true);
        }            
        finishedPresentingScore = true;

    }


    void SkipBonusPresentation()
    {
        if(CR_BonusPresentation != null)
        {
            StopCoroutine(CR_BonusPresentation);
            CR_BonusPresentation = null;
        }

        foreach(SpecialBonusSlot bonusSlot in specialBonusSlots)
        {
            bonusSlot.gameObject.SetActive(true);
        }
        hasPresentedBonuses = true;

    }

    public void StartScorePresentation()
    {
        
        foreach(GameObject uiElement in orderOfElementsToShow)
        {
            uiElement.SetActive(false);
        }         
        CR_ScorePresentation = StartCoroutine(ShowScoresInOrder(delayBetweenShowingScores));
    }

    IEnumerator ShowScoresInOrder(float delay)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        foreach(GameObject uiElement in orderOfElementsToShow)
        {
            uiElement.SetActive(true);
            yield return new WaitForSecondsRealtime(delay);
        }

        if(specialBonusSlots.Count == 0)
        {
            scoreManager.AddScoreToPlayerData(StageStateController.Instance.PlayerData);
            SetTotalScoreText(StageStateController.Instance.PlayerData.CurrentScore);
            totalScoreElement.SetActive(true);
        }

        finishedPresentingScore = true;
        CR_ScorePresentation = null;
    }

    public void StartBonusPresentation()
    {
        specialBonusesPanel.SetActive(true);
        bonusPanelIsOpen = true;

        foreach(SpecialBonusSlot bonusSlot in specialBonusSlots)
        {
            bonusSlot.gameObject.SetActive(false);
        }

        CR_BonusPresentation = StartCoroutine(ShowBonusesInOrder(delayBetweenShowingBonuses));
    }

    IEnumerator ShowBonusesInOrder(float delay)
    {
        if(specialBonusSlots.Count == 0)
        {
            CR_BonusPresentation = null; 
            yield break; 
        }


        yield return new WaitForSecondsRealtime(0.25f);
        foreach(SpecialBonusSlot bonusSlot in specialBonusSlots)
        {
            bonusSlot.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(delay);
        }
        hasPresentedBonuses = true;
        CR_BonusPresentation = null;
    }

    void PrepareSpecialBonuses(List<BonusScoreItemSO> bonusScoreItems)
    {
        if(bonusScoreItems.Count == 0) { return; }

        var sortedBonusItems = bonusScoreItems.OrderByDescending(item => item.BaseScore).ToList();

        foreach(var bonusScoreItem in sortedBonusItems)
        {
            SpecialBonusSlot bonusSlot = Instantiate(bonusSlotPrefab, bonusRewardsParent.transform).GetComponent<SpecialBonusSlot>();
            bonusSlot.currentBonusItem = bonusScoreItem;
            bonusSlot.SetElementsToScoreItem();
            specialBonusSlots.Add(bonusSlot);
        }

    }




    public void SetDamageTakenText(int damageTaken, int scoreValue)
    {
        damageTakenValue.text = damageTaken.ToString();

        damageTakenScore.text = "+" + scoreValue.ToString();
    }

    public void SetTimeTakenText(float timeTaken, int scoreValue)
    {

        float roundedTime = Mathf.Round(timeTaken * 10f) / 10f;

        timeTakenValue.text = roundedTime.ToString("F1");

        timeTakenScore.text = "+" + scoreValue.ToString();
    }

    public void SetEnemiesDefeatedText(int enemiesDefeated, int scoreValue)
    {
        enemiesDefeatedValue.text = enemiesDefeated.ToString();

        enemiesDefeatedScore.text = "+" + scoreValue.ToString();
    }

    public void SetStageScoreText(int scoreValue)
    {
        stageScoreValue.text = scoreValue.ToString();
    }
    
    public void SetTotalScoreText(int scoreValue)
    {
        totalScoreValue.text = scoreValue.ToString();
    }
    public void EnableTotalScoreText()
    {
        totalScoreElement.SetActive(true);
    }


}
