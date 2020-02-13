using UnityEngine;
using System.Collections;

public class ScriptGuy : NPC
{

    private NPCJson.Message[] newScripts;

    protected override void Start() {
        base.Start();
        newScripts = new NPCJson.Message[1];
        newScripts[0].time = 2;
    }

    public override void showOptions() {
        manager.ChangeMode(UIManager.Mode.BESTIARY);
        //if (farewellMessage.Length > 0)
            showFarewell = true;
    }

    public override void ActivateInteraction()
    {
        int count = 0;
        Beastiary.Yokai[] yokais = manager.getYokais();
        for (int i = 0; i < yokais.Length; i++ )
        {
            if (yokais[i].status == Beastiary.Status.Found)
            {
                yokais[i].status = Beastiary.Status.Known;
                count++;
            }
        }

        if (count == 0)
        {
            showMessage(npc.hiMessage);
        }
        else
        {
            newScripts[0].text = "Wow... you have found " + count + " new scripts";
            showMessage(newScripts);
        }
        manager.StopDisplayOnScreen(gameObject);
    }

}
