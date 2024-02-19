using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPopup : MonoBehaviour
{
    [SerializeField] Image notificationIcon;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;


    public void Initialize(NotificationData notificationData, Sprite icon)
    {
        notificationIcon.sprite = icon;
        titleText.text = notificationData.title;
        descriptionText.text = notificationData.description;
    }


}
