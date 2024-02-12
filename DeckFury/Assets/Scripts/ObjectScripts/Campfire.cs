using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Campfire : MonoBehaviour
{

    [SerializeField] GameObject contextPopup;

[Range(0.1f, 1)]
    [SerializeField] double baseHealAmount;

    public bool hasHealedPlayer = false;

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
            HealPlayer();
            contextPopup.SetActive(false);
        }        
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(hasHealedPlayer){ return; }

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



    void HealPlayer()
    {
        if(hasHealedPlayer){ return; }

        if(StageStateController.Instance)
        {
            if(StageStateController.Instance.PlayerData != null)
            {
                PlayerDataContainer playerData = StageStateController.Instance.PlayerData;
                PlayerController player = GameManager.Instance.player;

                if(player.CurrentHP + (int)(playerData.MaxHP * baseHealAmount) > playerData.MaxHP)
                {
                    player.CurrentHP = playerData.MaxHP;
                }else
                {
                    player.CurrentHP += (int)(playerData.MaxHP * baseHealAmount);
                }

                hasHealedPlayer = true;
            }
        }

    }



}
