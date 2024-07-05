using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintController : MonoBehaviour
{
    [SerializeField] CardSelectionMenu cardSelectionMenu;
    [SerializeField] HintsContainerSO hintsContainer;
    [SerializeField] TextMeshProUGUI hintText;

    int previousHintIndex = 0;

    bool firstPanelOpen = true;

    void Start()
    {
        firstPanelOpen = true;
        cardSelectionMenu.OnMenuActivated += SequentialHint;
        
    }

    void Update()
    {
        
    }

    public void RandomizeHint()
    {

        if(firstPanelOpen)
        {
            hintText.text = hintsContainer.GetHints()[1];
            previousHintIndex = 1;
            firstPanelOpen = false;
            return;
        }

        int randomInt  = Random.Range(0, hintsContainer.GetHints().Length);
        while(randomInt == previousHintIndex)
        {
            randomInt  = Random.Range(0, hintsContainer.GetHints().Length);
        }

        previousHintIndex = randomInt;
        hintText.text = hintsContainer.GetHints()[randomInt];


    }

    void SequentialHint()
    {
        if(previousHintIndex == hintsContainer.GetHints().Length - 1)
        {
            previousHintIndex = 0;
        }else
        {
            previousHintIndex++;
        }

        hintText.text = hintsContainer.GetHints()[previousHintIndex];



    }

}
