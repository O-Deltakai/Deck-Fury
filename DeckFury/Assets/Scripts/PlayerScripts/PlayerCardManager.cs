using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles what cards the player has access to - what's in the player's card magazine
//Also handles the usage of cards
public class PlayerCardManager : MonoBehaviour
{

#region Event Handlers

    //Event that is triggered whenever a card is removed from the magazine
    public delegate void RemoveCardEventHandler();
    public event RemoveCardEventHandler OnRemoveCard;

    //Event that is triggered whenever the magazine is loaded
    public delegate void LoadMagazineEventHandler();
    public event LoadMagazineEventHandler OnLoadMagazine;

    //Event that is triggered whenever a card is used and inputs the specific card that was used
    public delegate void UseSpecificCardEventHandler(CardObjectReference card);
    public static event UseSpecificCardEventHandler OnUseSpecificCard;

    //Event that is trigged when a card gets used in general, has no input unlike OnUseSpecificCard
    public delegate void UseCardEventHandler();
    public event UseCardEventHandler OnUseCard;

#endregion


    PlayerAnimationController animationController;
    PlayerController player;

    //The list of cards the player can use in battle. Is loaded during the Card Selection Menu.
    [field:SerializeField] public List<CardObjectReference> CardMagazine{get; private set;} = new List<CardObjectReference>(); 

    [Tooltip("How long the player needs to wait before they can use a new card by default.")]
    [SerializeField] float defaultCooldown = 0.10f;

    public bool CanUseCards = true;

    private void Awake()
    {
        animationController = GetComponent<PlayerAnimationController>();    
        player = GetComponent<PlayerController>();
    }

    void Start()
    {
        
    }

    //Clears the cardMagazine and then loads a given cardLoad into the cardMagazine
    public void LoadCardMagazine(List<CardObjectReference> cardLoad)
    {
        CardMagazine.Clear();

        foreach(CardObjectReference card in cardLoad)
        {
            CardMagazine.Add(card);
        }

        //Raise LoadMagazineEvent event if at least one other object is subscribed
        OnLoadMagazine?.Invoke();


    }

    //Loads a single card into the bottom of the magazine - returns if the card magazine is already full
    public void LoadOneCard(CardObjectReference card)
    {
        if(CardMagazine.Count >= 5){return;}
        CardMagazine.Add(card);
        OnLoadMagazine?.Invoke();
    }

    /// <summary>
    /// A gatekeeper method that checks if the next card to be used uses an Animation Event or not and calls the correct method to use said card.
    /// </summary>
    public void TriggerCard()
    {
        if(!CanUseCards)
        {return;}
        CardObjectReference cardToUse = CardMagazine[0];

        //If cardToUse has UsesAnimationEvent set to true, use the animationController to play said animation, otherwise call the UseCardOneShot method.
        if(!cardToUse.cardSO.UsesAnimationEvent)
        {
            UseCardOneShot();
        }else
        {
            AnimationClip cardAnimation = cardToUse.cardSO.PlayerAnimation;
            animationController.PlayOneShotAnimationReturnIdle(cardToUse.cardSO.PlayerAnimation);
            CanUseCards = false;

            //The extra 0.05f is just a fail-safe to make sure that the player absolutely cannot use another card whilst the first animation
            //is still playing. Without this, its unlikely but possible for a player to click fast enough to play another card whilst first one is
            //still going due to float rounding issues.
            StartCoroutine(CardUseCooldown(cardAnimation.length + 0.05f));
            StartCoroutine(RemoveCardFromMagazine(CardMagazine[0], cardAnimation.length + 0.05f) );   

        }

        OnUseCard?.Invoke();
        OnUseSpecificCard?.Invoke(cardToUse);

    }


    //Uses the first card in the magazine and then immediately removes it from the magazine.
    //Only use this method for cards that do not use a player animation to trigger its effect
    //The card can still use an animation, just that the effect of the card will be independent from the animation.
    void UseCardOneShot()
    {
        if(!CanUseCards)
        {return;}

        CardEffect cardToUse = CardMagazine[0].effectPrefab.GetComponent<CardEffect>();

        cardToUse.gameObject.SetActive(true);
        cardToUse.ActivateCardEffect();

        //If an animation is set even though UseAnimationEvent is false, it will just play the animation but the card effect
        //will trigger immediately
        if(cardToUse.cardSO.PlayerAnimation != null)
        {
            animationController.PlayOneShotAnimationReturnIdle(cardToUse.cardSO.PlayerAnimation);
            CanUseCards = false;

            StartCoroutine(CardUseCooldown(cardToUse.cardSO.PlayerAnimation.length + 0.05f));
            StartCoroutine(RemoveCardFromMagazine(CardMagazine[0], cardToUse.cardSO.PlayerAnimation.length + 0.05f));  

            return; 
        }

        //Trigger card use cooldown, preventing the next card from being used until cooldown is complete
        CanUseCards = false;

        StartCoroutine(CardUseCooldown(defaultCooldown));
        StartCoroutine(RemoveCardFromMagazine(CardMagazine[0]));        

    }

    //Method that is called on an animation event of a player animation which is used for a card effect
    void UseCardAnimationEvent()
    {
        //Checks if the first card actually has a player animation defined or that it actually uses an animation event,
        //otherwise will return. This is meant to allow player animations that are used for card effects to be able to be used for other
        //things without accidentally triggering this method.
        if(CardMagazine[0].cardSO.PlayerAnimation == null || !CardMagazine[0].cardSO.UsesAnimationEvent)
        {return;}

        CardEffect cardToUse = CardMagazine[0].effectPrefab.GetComponent<CardEffect>();

        cardToUse.gameObject.SetActive(true);
        cardToUse.ActivateCardEffect(); 

    }



    IEnumerator CardUseCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        CanUseCards = true;   
    }

    //Removes the given card from the magazine, can be given an optional duration to remove the card only after said duration
    IEnumerator RemoveCardFromMagazine(CardObjectReference card, float duration = 0)
    {
        yield return new WaitForSeconds(duration);
        CardMagazine.Remove(card);

        //Invoke event if it is not null
        OnRemoveCard?.Invoke();
    }

    public bool MagazineIsEmpty()
    {
        if(CardMagazine.Count == 0)
        {
            return true;
        }
        return false;
    }

}
