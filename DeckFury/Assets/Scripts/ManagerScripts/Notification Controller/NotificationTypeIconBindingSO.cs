using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "Notification Type Icon Binding", menuName = "New Notification Type Icon Binding", order = 0)]
public class NotificationTypeIconBindingSO : ScriptableObject
{
    [Serializable]
    public class NoteIconType
    {
        public Sprite icon;
        public NotificationType type;
    }


    [SerializeField] List<NoteIconType> _iconTypeBindings = new();
    public IReadOnlyList<NoteIconType> IconTypeBindings { get => _iconTypeBindings; }
    Dictionary<NotificationType, NoteIconType> noteIconTypeDictionary = new();

    void OnValidate()
    {
        InitializeDictionary();
    }


    void InitializeDictionary()
    {
        noteIconTypeDictionary.Clear();

        foreach (NoteIconType iconType in _iconTypeBindings)
        {
            noteIconTypeDictionary.Add(iconType.type, iconType);
        }
    }

    public Sprite GetIcon(NotificationType type)
    {
        if (noteIconTypeDictionary.TryGetValue(type, out NoteIconType iconType))
        {
            return iconType.icon;
        }
        return null;
    }

    public void GenerateBindingsFromEnum()
    {
        foreach (NotificationType type in Enum.GetValues(typeof(NotificationType)))
        {
            if (!HasBinding(type))
            {
                NoteIconType newBinding = new()
                {
                    type = type,
                    icon = null
                };

                _iconTypeBindings.Add(newBinding);
            }
        }
    }

    private bool HasBinding(NotificationType type)
    {
        foreach (NoteIconType iconType in _iconTypeBindings)
        {
            if (iconType.type == type)
            {
                return true;
            }
        }
        return false;
    }
}
