using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRangeChecker : MonoBehaviour
{
    [SerializeField] bool _playerInRange = false;
    public bool InRange => _playerInRange;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(TagNames.Player.ToString()))
        {
            _playerInRange = true;
            print("Player in range");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag(TagNames.Player.ToString()))
        {
            _playerInRange = false;
            print("Player out of range");
        }
    }

}
