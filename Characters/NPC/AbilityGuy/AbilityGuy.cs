using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGuy : NPC {

    protected override void Start()
    {
        base.Start();
    }

    public override void showOptions()
    {
        manager.ChangeMode(UIManager.Mode.ABILITY);
        showFarewell = true;
    }

    public override void showMessage(Progress.GameItems item)
    {
        base.showMessage(item);
        //print((("Progress Item = " + item.getMessage(gameObject)[0].text);
        manager.ability.ObtainNewAbility(item);

    }
    
}
