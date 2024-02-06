using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFactor : MonoBehaviour
{
    [SerializeField] StageEntity entity;
    [SerializeField, Min(0)] int healAmount = 1;
    [SerializeField, Min(0.0166f)] float healInterval = 1f;

    int entityStartHP;

    Coroutine CR_healOverTimeCoroutine;

    void Start()
    {
        if(!entity)
        {
            if(!TryGetComponent<StageEntity>(out entity))
            {
                Debug.LogWarning("HealingFactor.cs: No StageEntity component found on " + gameObject.name + ". Disabling HealingFactor.");
                enabled = false;
                return;
            }
        }

        if((NPC)entity)
        {
            entityStartHP = ((NPC)entity).EnemyData.MaxHP;
        }else
        {
            entityStartHP = entity.CurrentHP;
        }


        CR_healOverTimeCoroutine = StartCoroutine(HealOverTime());
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void StartHealOverTime()
    {
        if(CR_healOverTimeCoroutine != null)
        {
            StopCoroutine(CR_healOverTimeCoroutine);
        }
        CR_healOverTimeCoroutine = StartCoroutine(HealOverTime());
    }

    public void StopHealOverTime()
    {
        if(CR_healOverTimeCoroutine != null)
        {
            StopCoroutine(CR_healOverTimeCoroutine);
        }
    }

    IEnumerator HealOverTime()
    {
        while(!entity.IsDead)
        {
            yield return new WaitForSeconds(healInterval);
            if(entity.CurrentHP < entityStartHP)
            {
                entity.CurrentHP += healAmount;
            }
        }
    }


}
