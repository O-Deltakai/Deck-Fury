using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    [Serializable]
    public class NoteIconType
    {
        public Sprite icon;
        public NotificationType type;
    }

    [SerializeField] List<NoteIconType> noteIconTypes;
    [SerializeField] NoteIconType deckUnlockNoteIcon;
    Dictionary<NotificationType, Sprite> noteTypeIconDict = new Dictionary<NotificationType, Sprite>();

    static NotificationController _instance;
    public static NotificationController Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }


    void InitializeNoteTypeIconDict()
    {
        foreach (var noteIconType in noteIconTypes)
        {
            noteTypeIconDict.Add(noteIconType.type, noteIconType.icon);
        }
    }



    void RecieveNotification(Notification notification)
    {

    }

}
