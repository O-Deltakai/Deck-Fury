using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Anvil : MonoBehaviour
{
    [SerializeField] UpgradeManager upgradeManager;
    [SerializeField] GameObject contextPopup;

    bool playerInRange;

    void Awake()
    {
        contextPopup.SetActive(false);    
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            if(playerInRange)
            {
                OpenUpgradeMenu();
                contextPopup.SetActive(false);
            }
        }        
    }

    private void OnTriggerStay2D(Collider2D other) 
    {


        if (other.CompareTag(TagNames.Player.ToString()))
        {
            playerInRange = true;
            contextPopup.SetActive(true);
        }

    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag(TagNames.Player.ToString()))
        {
            playerInRange = false;
            contextPopup.SetActive(false);
        }            
    }

    void OpenUpgradeMenu()
    {
        upgradeManager.MoveUiIntoView();
    }


}
