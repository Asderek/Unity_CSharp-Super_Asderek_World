using UnityEngine;
using System.Collections;

public abstract class NPC : Interactable{

    protected bool showFarewell = false;

    public enum NPCs
    {
        AbilityGuy,
        ScriptGuy
    }

    public NPCJson.NPC_Message npc;
    

    public Texture2D bar;
    public Area barArea;

    private float startTalkingTime;
    public NPCJson.Message[] currentMessage;
    protected int currentMessageIndex = -1;
    private Rect position = new Rect(Screen.width*0.1f, Screen.height*0.1f, Screen.width * 0.8f, Screen.height * 0.15f);
    private GUIStyle middleCenter;

    protected override void Start() {
        base.Start();
        button = ButtonManager.ButtonID.CIRCLE;
        middleCenter = new GUIStyle();
        middleCenter.alignment = TextAnchor.MiddleCenter;
        middleCenter.normal.textColor = Color.white;
        middleCenter.fontSize = 17;
        GameManager.GetInstance().UpdateNPC(ref npc);
    }
    
     void Update() {
        
         if (!inContact)
         {
             currentMessageIndex = -1;
         }

         if (showFarewell == true && manager.mode == UIManager.Mode.NORMAL)
         {
             showFarewell = false;
             if (npc.farewellMessage.Length == 0)
             {  
                 manager.InitDisplayOnScreen(gameObject, displayMsg, button);
             }
             else
                 showMessage(npc.farewellMessage);
             return;
         }

        if (currentMessageIndex < 0)
            return;

        if( (Time.time - startTalkingTime < currentMessage[currentMessageIndex].time))// && ((!(Input.GetButtonDown(button.ToString())))))
            return;


        startTalkingTime = Time.time;

        if (currentMessage.Length - 1 > currentMessageIndex)
            currentMessageIndex++;
        else
        {
            currentMessageIndex = -1;
            if (inContact )
            {
                if (currentMessage == npc.farewellMessage)
                    manager.InitDisplayOnScreen(gameObject, displayMsg, button);
                else
                {
                    showOptions();
                    return;
                }
            }
        }

    }

    public bool Equals(NPC other) {
        return other.npc.id == npc.id;
    }

    public override void ActivateInteraction() {

        Progress.GameItems item = manager.progress.CheckUpdate(npc.id);
        if (item == Progress.GameItems.NONE)
        {
            showMessage(npc.hiMessage);
        }
        else { 
            showMessage(item);
        }
        manager.StopDisplayOnScreen(gameObject);
    
    }
    
    public virtual void showMessage(NPCJson.Message[] message)
    {
        currentMessage = message;
        currentMessageIndex = 0;

        startTalkingTime = Time.time;
    }

    public virtual void showMessage(Progress.GameItems id) {

        foreach ( NPCJson.ItemMsg itemMsg in npc.itensMessage)
        {
            if (itemMsg.id == id)
            {
                showMessage(itemMsg.msgs);
                manager.progress.CommentedBy(npc.id, id);
                return;
            }
        }
        showMessage(npc.hiMessage);
    }

    public abstract void showOptions();

    protected virtual void OnGUI() {
        if (currentMessageIndex < 0)
            return;

        GUI.DrawTexture(barArea.GetRect(), bar, ScaleMode.StretchToFill, true, 0);
        GUI.Label(barArea.GetRect(), currentMessage[currentMessageIndex].text, GameManager.GetInstance().TextStyle());

    }
}
