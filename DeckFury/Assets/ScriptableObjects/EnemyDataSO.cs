using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Enemy Data", menuName = "New Enemy Data", order = 0)]
public class EnemyDataSO : ScriptableObject
{

    [SerializeField] string _enemyName;
    public string EnemyName{get { return _enemyName; }}

[Header("Combat Stats")]
    [SerializeField, Min(0)] int _maxHP;
    public int MaxHP{get{ return _maxHP; }}

    [SerializeField, Min(0)] int _shieldHP;
    public int ShieldHP{get{ return _shieldHP; }}

    [SerializeField, Range(0, 100)] int _armor;
    public int Armor {get{ return _armor; }}

    [SerializeField, Range(0.1f, 10f)] double _defense = 1;
    public double Defense {get { return _defense; }}

[Tooltip("The list of attack elements this enemy will take bonus damage from.")]
    [SerializeField] List<AttackElement> _weaknesses;
    public List<AttackElement> Weaknesses{get{ return _weaknesses; }}

[Tooltip("The list of attack elements this enemy will take reduced damage from.")]
    [SerializeField] List<AttackElement> _resistances;
    public List<AttackElement> Resistances{get{ return _resistances; }}

[Header("Miscellaneous")]
    [SerializeField, Range(0, 100)] int _enemyTier;
    public int EnemyTier{get{return _enemyTier;}}


[Header("Information")]
    [SerializeField, TextArea(10, 20)] string _enemyDescription;
    
    [SerializeField, TextArea(5, 20)] string[] _notes;
    
    public string GetFormattedDescription()
    {
        return _enemyDescription;
    }

    public string[] GetFormattedNotes()
    {
        return _notes;
    }




}
