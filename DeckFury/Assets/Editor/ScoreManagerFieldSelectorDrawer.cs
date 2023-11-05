using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;



[CustomPropertyDrawer(typeof(ScoreManagerFieldSelectorAttribute))]
public class ScoreManagerFieldSelectorDrawer : PropertyDrawer
{
    private static readonly Type[] AllowedTypes = { typeof(int), typeof(float), typeof(double) }; // Add more types as needed

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // // Get all fields from ScoreManager that match the allowed types
        // var fields = typeof(ScoreManager).GetFields(BindingFlags.Public | BindingFlags.Instance)
        //                                  .Where(f => AllowedTypes.Contains(f.FieldType)).ToList();

        var fields = typeof(ScoreManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                        .Where(f => AllowedTypes.Contains(f.FieldType) && 
                                                    (f.Name.EndsWith("k__BackingField") || 
                                                    f.GetCustomAttribute<SerializeField>() != null))
                                        .ToList();


        // // Create a list of field names
        // string[] fieldNames = fields.Select(f => f.Name + " (" + f.FieldType.Name + ")").ToArray();

        string[] fieldNames = fields.Select(f => 
        {
            string name = f.Name;
            if (name.EndsWith(">k__BackingField"))
            {
                name = name.Substring(1, name.IndexOf(">") - 1); // Extract property name from backing field name
            }
            return name + " (" + f.FieldType.Name + ")";
        }).ToArray();        

        int currentIndex = Array.IndexOf(fieldNames, property.stringValue);

        int selectedIndex = EditorGUI.Popup(position, property.displayName, currentIndex, fieldNames);

        if (selectedIndex >= 0)
        {
            property.stringValue = fieldNames[selectedIndex];
        }
    }



}