using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Color Palette", menuName = "New Item Color Palette", order = 0)]
public class ItemColorPaletteSO : ScriptableObject
{
    [Header("Rarity Colors")]
    [SerializeField] Color _commonColor = new Color(0.5372549f, 0.5372549f, 0.5372549f, 0.94f);
    public Color CommonColor => _commonColor;

    [SerializeField] Color _uncommonColor = new Color(0.2117647f, 0.5411765f, 0.5843138f, 0.94f);
    public Color UncommonColor => _uncommonColor;

    [SerializeField] Color _rareColor = new Color(0.8117647f, 0.6313726f, 0.07843138f, 0.94f );
    public Color RareColor => _rareColor;

    [SerializeField] Color _veryRareColor = new Color(0.6705883f, 0.2352941f, 0.6431373f, 0.94f );
    public Color VeryRareColor => _veryRareColor;


}
