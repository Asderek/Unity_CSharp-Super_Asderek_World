using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverSwitch : EventObserver {

    private bool status = true;

    public override void SendNotification(bool status, Switch source)
    {
        if (this.status != status)
        {
            GetComponent<Animator>().SetTrigger("triggerSwitch");
            this.status = status;
        }
    }
}
