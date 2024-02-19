using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Notification
{
    public NotificationType notificationType;
    public string title;
    public string description;
}
