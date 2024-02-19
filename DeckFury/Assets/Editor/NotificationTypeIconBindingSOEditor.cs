using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NotificationTypeIconBindingSO))]
public class NotificationTypeIconBindingSOEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NotificationTypeIconBindingSO iconTypeBinding = (NotificationTypeIconBindingSO)target;
        if (GUILayout.Button("Create Type Icon Bindings"))
        {
            iconTypeBinding.GenerateBindingsFromEnum();
        }

        foreach (var noteIconType in iconTypeBinding.IconTypeBindings)
        {
            if (noteIconType.icon != null)
            {
                GUILayout.Label(noteIconType.type.ToString());
                Texture2D iconTexture = noteIconType.icon.texture;
                GUILayout.Box(iconTexture, GUILayout.Width(50), GUILayout.Height(50));
            }
        }


    }

}
