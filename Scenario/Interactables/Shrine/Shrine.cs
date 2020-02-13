using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Shrine : NotifyingInteractables {

    public Commandments.Shrines id;
    protected LoadManager load;
    public SpriteRenderer displayImg;

    protected override void Start()
    {
        base.Start();
        load = LoadManager.getInstance();
        load.RegisterToList(id, gameObject);
    }

    protected override void OnTriggerStay2D(Collider2D colisor)
    {
        base.OnTriggerStay2D(colisor);
    }

    public void Update()
    {
        if (ApplyOnce.alreadyApplied("Interact", gameObject))
        {
            if (player.FinishedAnimation("sitting_neutral") || player.FinishedAnimation("seated"))
            {
                manager.SetWarp(displayImg);
                ApplyOnce.remove("Interact", gameObject);
            }

        }
    }

    public override void ActivateInteraction()
    {
        base.ActivateInteraction();
        player.GetComponent<Asderek>().LerpToPosition(transform.position);

        ApplyOnce.apply("Interact", gameObject, () => { return true; });


        /*
        if (id == Commandments.Shrines.FIRE_STAGE_1)
            load.loadShrine(Commandments.Shrines.FIRE_STAGE_2);
        else if (id== Commandments.Shrines.FIRE_STAGE_2)
            load.loadShrine(Commandments.Shrines.NEXUS);
        else if (id == Commandments.Shrines.NEXUS)
            load.loadShrine(Commandments.Shrines.FIRE_STAGE_1);
         */

    }

    protected override void HandlePlayerInteraction()
    {
        if ((player.StateOfAnimation("sitting_neutral") != -1) || (player.StateOfAnimation("seated") != -1) || (player.StateOfAnimation("gettingup_neutral") != -1))
        {
            return;
        }
        base.HandlePlayerInteraction();
    }

    public void Clear()
    {
        displayImg.sprite = null;
    }
}
