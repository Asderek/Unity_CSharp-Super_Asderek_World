using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWays : RouteFloor {

    protected Asderek player;
    
    protected override void FixedUpdate()
    {
        if (player!=null && player.gameObject.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("notifications_water_stay"))
        {
            base.FixedUpdate();
        }
    }

    protected override void HandleEdge()
    {


        if ((steps[steps.Count - 1].position.x - steps[steps.Count - 2].position.x) > 0)
        {
            player.gameObject.transform.eulerAngles = new Vector2(0, 0);
            //vira asderek pra direita
        }
        else
        {
            player.gameObject.transform.eulerAngles = new Vector2(0, 180);
        }

        player.ReceiveNotification(Asderek.Notification.Return);



       GameObject obj = player.gameObject;
       while ( obj.transform.parent != transform && (obj.transform.parent != null))
        {
            obj = obj.transform.parent.gameObject;
        }
        obj.transform.parent = null;

        player = null;


        if (currentStep <= 0)
        {
            currentStep = steps.Count - 1;
            transform.position = steps[steps.Count - 1].position;
        }
        else if (currentStep >= steps.Count)
        {
            currentStep = 0;
            transform.position = steps[0].position;
        }
    }

    public void setPlayer(Asderek script)
    {
        player = script;


        GameObject obj = script.gameObject;
        while (obj.transform.parent != null) {
            obj = obj.transform.parent.gameObject;
        }
        obj.transform.parent = transform;
    }
}
