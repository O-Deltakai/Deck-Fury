using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RewardMethodSelectorAttribute))]
public class RewardMethodSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get all methods from RewardConditions
        var methods = typeof(RewardConditionChecks).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                              .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(BonusScoreItemSO))
                                              .Select(m => m.Name)
                                              .ToArray();

        int currentIndex = System.Array.IndexOf(methods, property.stringValue);
        int selectedIndex = EditorGUI.Popup(position, "Reward Method", currentIndex, methods);

        if (selectedIndex >= 0)
        {
            property.stringValue = methods[selectedIndex];
        }
    }
}
