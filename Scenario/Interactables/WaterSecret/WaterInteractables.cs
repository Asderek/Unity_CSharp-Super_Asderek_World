using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class WaterInteractables : NotifyingInteractables {

    public WaterWays path;

    public override void ActivateInteraction()
    {
        if (manager.getSelectedWeapon() == Commandments.Element.WATER.toWeapon())
        {
            base.ActivateInteraction();
            path.setPlayer(player);
        }
    }

    public override bool IsEnable(Asderek player)
    {
        if(manager.getSelectedWeapon() == Commandments.Element.WATER.toWeapon())
        {
            return true;
        }
        return false;
    }

}
