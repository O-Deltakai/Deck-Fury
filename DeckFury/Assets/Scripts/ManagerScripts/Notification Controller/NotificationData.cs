using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NotificationData
{
    public NotificationType notificationType;
    public string title;
    public string description;

    public NotificationData(NotificationType type, string title, string description)
    {
        notificationType = type;
        this.title = title;
        this.description = description;
    }

}
