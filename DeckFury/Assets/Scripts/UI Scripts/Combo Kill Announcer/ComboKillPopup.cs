using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(UIWaypointTraverser))]
public class ComboKillPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] TextMeshProUGUI comboNameText;

    [SerializeField] Color numberColor = Color.cyan;
    [SerializeField] Color comboNameColor = Color.cyan;

    public UIWaypointTraverser traverser { get; private set; }

    [SerializeField] TextShaker _numberTextShaker;
    public TextShaker NumberTextShaker { get => _numberTextShaker; }

    [SerializeField] TextShaker _comboNameTextShaker;
    public TextShaker ComboNameTextShaker { get => _comboNameTextShaker; }

    public RectTransform rectTransform { get; private set; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        traverser = GetComponent<UIWaypointTraverser>();
    }

    void Start()
    {
        
    }


    public void SetNumber(int number)
    {
        numberText.text = number.ToString() + "x";
    }
    public void SetComboName(string comboName)
    {
        comboNameText.text = "    " + comboName;
    }

    public void SetNumberColor(Color color)
    {
        numberText.color = color;
    }

    public void SetComboNameColor(Color color)
    {
        comboNameText.color = color;
    }



}
