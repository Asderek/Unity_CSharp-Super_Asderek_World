using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyingInteractables : Interactable {

    public Asderek.Notification notificationType;

    public override void ActivateInteraction()
    {
        base.ActivateInteraction();
        player.GetComponent<Asderek>().ReceiveNotification(notificationType);
       
    }
}
