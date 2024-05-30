using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used for pooling card effect prefabs and their object summons so they can be easily accessed by other systems
///like the card selection menu. Also handles pooling and hashing of card targeting reticles. Non-persistent Singleton object.
/// </summary>
public class CardPoolManager : MonoBehaviour
{
    //Singleton
    private static CardPoolManager _instance;
    public static CardPoolManager Instance{get {return _instance;} }

    public delegate void CompletePoolingEventHandler();
    public event CompletePoolingEventHandler OnCompletePooling;

    StageStateController stageController;


    //Maintain a reference to the player object in the scene at all times so that initialized effect prefabs or object summons can be given
    //a reference to the player quickly.
    [SerializeField] PlayerController player;


    [SerializeField] DeckSO defaultDeck;
    public DeckSO DefaultDeck => defaultDeck;

    GameDeck currentActiveDeck;

    //GameObject which all pooled objects will be instantiated under
    [SerializeField] GameObject CardPoolParent;

    /// <summary>
    /// GameObject which all pooled targeting reticles will be instantiated under, should normally be the TargetingReticleAnchor
    /// on the player object.
    /// </summary>
    [SerializeField] GameObject TargetingReticlePoolParent;

    /// <summary>
    /// Dictionary for reticles which are cached at the start of the game by checking the TargetingReticle prefab set for
    /// each card within the Card Pool
    /// </summary>
    Dictionary<GameObject, GameObject> ReticleDictionary = new Dictionary<GameObject, GameObject>();

    //A reference list to all CardObjectReferences that are defined during the pooling process
    //Primarily used by the card selection system in order to populate the menu with selectable cards
    [field:SerializeField] public List<CardObjectReference> CardObjectReferences {get; private set;}

    //A reference list of all currently pooled card objects (except card targeting reticles) for debugging purposes
    [SerializeField] List<GameObject> PooledObjects = new List<GameObject>();

    [SerializeField] bool useStageStateController = true;

    private void Awake()
    {
        _instance = this;
        

    }

    private void OnDestroy() 
    {
        _instance = null;
        GameManager.Instance.OnSetCriticalReferences -= InitializePooling;
    }

    private void Start() 
    {
        if(useStageStateController)
        {
            stageController = GameErrorHandler.NullCheck(StageStateController.Instance, "StageStateController");
            if(stageController.UsingPLC == true)
            {
                currentActiveDeck = StageStateController.Instance.PlayerData.CurrentDeck;
            }
        }

        GameManager.Instance.OnSetCriticalReferences += InitializePooling;
        //InitializePooling();

    }



    private void InitializePooling()
    {
        print("CardPoolManager intializing pooling");
        player = GameErrorHandler.NullCheck(GameManager.Instance.player, "Player from GameManager instance");
        TargetingReticlePoolParent = player.aimpoint.TargetingReticleAnchor;

        if(stageController != null && currentActiveDeck != null && currentActiveDeck.CardList != null && currentActiveDeck.CardList.Count > 0)
        {

            print("pooling objects from active game deck");
            PoolObjectsFromDeck(currentActiveDeck); 
            PoolReticlesFromDeck(currentActiveDeck);
        }else
        {
            print("pooling objects from default deck");
            PoolObjectsFromDeck(defaultDeck); 
            PoolReticlesFromDeck(defaultDeck);
        }
        
        OnCompletePooling?.Invoke();        
    }


    public void SetDefaultDeck(DeckSO deck)
    {
        defaultDeck = deck;
        RepoolObjectsFromDeck();

    }
    public void SetActiveGameDeck(GameDeck gameDeck)
    {
        currentActiveDeck = gameDeck;
        RepoolObjectsFromDeck();
     
    }


    //Clears the current pool of CardObjectReferences and ReticleDictionary and pools objects from the current active deck again
    public void RepoolObjectsFromDeck(bool forceClearPooledObjects = false)
    {
        CardObjectReferences.Clear();
        ReticleDictionary.Clear();

        //If forceClearPooledObjects is set to true, destroy all GameObjects within the PooledObjects list and clear it. This may take a long time
        //and could cause a significant hitch if used during active gameplay, so this is set to false by default. This section is mainly meant as a way to 
        //clear memory if for some reason we have too many pooled GameObjects in the scene (which at least at the moment isn't likely).
        if(forceClearPooledObjects)
        {
            CleanObjectPool();
        }

        if(currentActiveDeck != null && currentActiveDeck.CardList.Count > 0)
        {
            PoolObjectsFromDeck(currentActiveDeck); 
            PoolReticlesFromDeck(currentActiveDeck);
        }else
        {
            PoolObjectsFromDeck(defaultDeck); 
            PoolReticlesFromDeck(defaultDeck);
        }
        
        OnCompletePooling?.Invoke(); 
    }

    void CleanObjectPool()
    {
        foreach(GameObject obj in PooledObjects) 
        {
            Destroy(obj);    
        }
        PooledObjects.Clear();
    }

    /// <summary>
    /// Sets the first found given CardSO in the CardObjectReference list to invisible, so that it will not be displayed in the card selection menu.
    /// </summary>
    /// <param name="cardSO"></param>
    public void SetCardReferenceInvisible(CardSO cardSO)
    {
        CardObjectReferences.Find(x => x.cardSO == cardSO).invisible = true;
    }

    /// <summary>
    /// Adds a single deck element to the object pool
    /// </summary>
    /// <param name="deckElement"></param>
    public void AddDeckElementToPool(DeckElement deckElement)
    {
        //If the deck element has less than or equal to 0 card count, issue a warning and continue to the next deck element.
        if(deckElement.cardCount <= 0)
        {
            Debug.LogWarning("Deck element of card: " + deckElement.card.CardName + "in player deck " 
            + "has a card count of less then or equal to 0, which should not happen. Remove the offending deck"
            + "element or increase its count past 0 to fix this warning.");
            return;
        }
        //Make sure that the card has an EffectPrefab defined in its cardSO, otherwise continue to the next deck element
        if(deckElement.card.EffectPrefab == null)
        {
            Debug.LogWarning("Card: " + deckElement.card.CardName + " does not have an EffectPrefab defined in its CardSO, card will not work.");
            return;
        }

        //Begin instantiating effect prefabs/object summons of the card and repeat card count many times.
        for(int i = 0; i < deckElement.cardCount; i++)
        {
            CardEffect effectPrefab = Instantiate(deckElement.card.EffectPrefab, CardPoolParent.transform).GetComponent<CardEffect>();
            effectPrefab.player = player;
            effectPrefab.cardSO = deckElement.card;
            effectPrefab.gameObject.SetActive(false);

            //Check if the card has the ObjectSummonsArePooled condition ticked and it has objects within its ObjectSummonList,
            //then pool objects within the list if conditions are met.
            if(deckElement.card.ObjectSummonsArePooled && deckElement.card.ObjectSummonList.Count > 0)
            {
                foreach(GameObject objectSummon in deckElement.card.ObjectSummonList)
                {
                    //Add references to the object summons to the effectPrefab so that it may access the objects.
                    GameObject concreteObject = Instantiate(objectSummon, CardPoolParent.transform);
                    effectPrefab.ObjectSummonList.Add(concreteObject);
                    PooledObjects.Add(objectSummon);
                    concreteObject.SetActive(false);
                }
            }
        
            PooledObjects.Add(effectPrefab.gameObject);

            //Create a new CardObjectReference that references the instantiated EffectPrefab and its ObjectSummons
            CardObjectReference cardObjectReference = new CardObjectReference
            {
                cardSO = deckElement.card,
                effectPrefab = effectPrefab.gameObject,
                objectSummonPrefabs = effectPrefab.ObjectSummonList
            };

            CardObjectReferences.Add(cardObjectReference);
        }


        CardSO card = deckElement.card;
        if(!card.UseTargetingReticle || card.TargetingReticle == null)
        {return;}

        if(!ReticleDictionary.ContainsKey(card.TargetingReticle))
        {
            GameObject instantiatedReticle = Instantiate(card.TargetingReticle, TargetingReticlePoolParent.transform);
            ReticleDictionary.Add(card.TargetingReticle, instantiatedReticle);
            instantiatedReticle.SetActive(false);
        }


    }

    //Primary method for pooling all effect prefabs/object summons from a given deck
    private void PoolObjectsFromDeck(DeckSO deck)
    {

        foreach(DeckElement deckElement in deck.CardList)
        {
            //If the deck element has less than or equal to 0 card count, issue a warning and continue to the next deck element.
            if(deckElement.cardCount <= 0)
            {
                Debug.LogWarning("Deck element of card: " + deckElement.card.CardName + "in Deck: " + deck.DeckName
                + "has a card count of less then or equal to 0, which should not happen. Remove the offending deck"
                + "element or increase its count past 0 to fix this warning.");
                continue;
            }
            //Make sure that the card has an EffectPrefab defined in its cardSO, otherwise continue to the next deck element
            if(deckElement.card.EffectPrefab == null)
            {
                Debug.LogWarning("Card: " + deckElement.card.CardName + " does not have an EffectPrefab defined in its CardSO, card will not work.");
                continue;
            }

            //Begin instantiating effect prefabs/object summons of the card and repeat card count many times.
            for(int i = 0; i < deckElement.cardCount; i++)
            {
                CardEffect effectPrefab = Instantiate(deckElement.card.EffectPrefab, CardPoolParent.transform).GetComponent<CardEffect>();
                effectPrefab.player = player;
                effectPrefab.cardSO = deckElement.card;

                //Check if the card has the ObjectSummonsArePooled condition ticked and it has objects within its ObjectSummonList,
                //then pool objects within the list if conditions are met.
                if(deckElement.card.ObjectSummonsArePooled && deckElement.card.ObjectSummonList.Count > 0)
                {
                    foreach(GameObject objectSummon in deckElement.card.ObjectSummonList)
                    {
                        //Add references to the object summons to the effectPrefab so that it may access the objects.
                        effectPrefab.ObjectSummonList.Add(Instantiate(objectSummon, CardPoolParent.transform));
                        PooledObjects.Add(objectSummon);
                    }
                }
            
                PooledObjects.Add(effectPrefab.gameObject);

                //Create a new CardObjectReference that references the instantiated EffectPrefab and its ObjectSummons
                CardObjectReference cardObjectReference = new CardObjectReference
                {
                    cardSO = deckElement.card,
                    effectPrefab = effectPrefab.gameObject,
                    objectSummonPrefabs = effectPrefab.ObjectSummonList

                };

                CardObjectReferences.Add(cardObjectReference);

            }
            

        }

        //All pooled effect prefabs and object summons start disabled by default, using a card will enable their corresponding effect prefab. 
        foreach(GameObject gameObject in PooledObjects)
        {
            gameObject.SetActive(false);
        }

    }

    private void PoolObjectsFromDeck(GameDeck deck)
    {

        foreach(DeckElement deckElement in deck.CardList)
        {
            //If the deck element has less than or equal to 0 card count, issue a warning and continue to the next deck element.
            if(deckElement.cardCount <= 0)
            {
                Debug.LogWarning("Deck element of card: " + deckElement.card.CardName + "in player deck " 
                + "has a card count of less then or equal to 0, which should not happen. Remove the offending deck"
                + "element or increase its count past 0 to fix this warning.");
                continue;
            }
            //Make sure that the card has an EffectPrefab defined in its cardSO, otherwise continue to the next deck element
            if(deckElement.card.EffectPrefab == null)
            {
                Debug.LogWarning("Card: " + deckElement.card.CardName + " does not have an EffectPrefab defined in its CardSO, card will not work.");
                continue;
            }

            //Begin instantiating effect prefabs/object summons of the card and repeat card count many times.
            for(int i = 0; i < deckElement.cardCount; i++)
            {
                CardEffect effectPrefab = Instantiate(deckElement.card.EffectPrefab, CardPoolParent.transform).GetComponent<CardEffect>();
                effectPrefab.player = player;
                effectPrefab.cardSO = deckElement.card;

                //Check if the card has the ObjectSummonsArePooled condition ticked and it has objects within its ObjectSummonList,
                //then pool objects within the list if conditions are met.
                if(deckElement.card.ObjectSummonsArePooled && deckElement.card.ObjectSummonList.Count > 0)
                {
                    foreach(GameObject objectSummon in deckElement.card.ObjectSummonList)
                    {
                        //Add references to the object summons to the effectPrefab so that it may access the objects.
                        effectPrefab.ObjectSummonList.Add(Instantiate(objectSummon, CardPoolParent.transform));
                        PooledObjects.Add(objectSummon);
                    }
                }
            
                PooledObjects.Add(effectPrefab.gameObject);

                //Create a new CardObjectReference that references the instantiated EffectPrefab and its ObjectSummons
                CardObjectReference cardObjectReference = new CardObjectReference
                {
                    cardSO = deckElement.card,
                    effectPrefab = effectPrefab.gameObject,
                    objectSummonPrefabs = effectPrefab.ObjectSummonList

                };

                CardObjectReferences.Add(cardObjectReference);

            }
            

        }

        //All pooled effect prefabs and object summons start disabled by default, using a card will enable their corresponding effect prefab. 
        foreach(GameObject gameObject in PooledObjects)
        {
            gameObject.SetActive(false);
        }

    }


    /// <summary>
    /// Hashes and stores all necessary targeting reticles used by the deck in the ReticleDictionary. The original prefab assigned 
    /// in the CardSO is used as the key, and the value is the instantiated reticle within the scene.
    /// </summary>
    /// <param name="deck"></param>
    void PoolReticlesFromDeck(DeckSO deck)
    {
        foreach(DeckElement deckElement in deck.CardList)
        {
            CardSO card = deckElement.card;
            if(!card.UseTargetingReticle || card.TargetingReticle == null)
            {continue;}

            if(!ReticleDictionary.ContainsKey(card.TargetingReticle))
            {
                GameObject instantiatedReticle = Instantiate(card.TargetingReticle, TargetingReticlePoolParent.transform);
                ReticleDictionary.Add(card.TargetingReticle, instantiatedReticle);
                instantiatedReticle.SetActive(false);
            }

        }
    }

    void PoolReticlesFromDeck(GameDeck deck)
    {
        foreach(DeckElement deckElement in deck.CardList)
        {
            CardSO card = deckElement.card;
            if(!card.UseTargetingReticle || card.TargetingReticle == null)
            {continue;}

            if(!ReticleDictionary.ContainsKey(card.TargetingReticle))
            {
                GameObject instantiatedReticle = Instantiate(card.TargetingReticle, TargetingReticlePoolParent.transform);
                ReticleDictionary.Add(card.TargetingReticle, instantiatedReticle);
                instantiatedReticle.SetActive(false);
            }

        }
    }



    public GameObject GetReticleFromPool(GameObject reticleKey)
    {
       return ReticleDictionary[reticleKey];
    }



}
