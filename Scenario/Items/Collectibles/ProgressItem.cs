using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProgressItem {

    public enum Status { Commented, NotCommented };

    [System.Serializable]
    public struct NPCStatus
    {
        public NPC.NPCs npc;
        public Status status;
    }

    public SaveJson.ItemInfo info;

    public void Init(Progress.GameItems itemID)
    {
        if (GameManager.GetInstance().GetCurrentSave().items == null)
        {
            info.id = itemID;
            info.obtained = false;
            info.isActive = false;

            List<NPCStatus> status = new List<NPCStatus>();
            foreach (NPCJson.NPC_Message npc in GameManager.GetInstance().npcsJson.npcs)
            {
                foreach (NPCJson.ItemMsg item in npc.itensMessage)
                {
                    if (item.id == itemID)
                    {
                        NPCStatus stat;
                        stat.npc = npc.id;
                        stat.status = Status.NotCommented;
                        status.Add(stat);
                        break;
                    }
                }
            }
            info.npcSstatus = status.ToArray();
            return;
        }

        foreach (SaveJson.ItemInfo i in GameManager.GetInstance().GetCurrentSave().items)
        {
            if (i.id == itemID)
            {
                info = i;
                return;
            }
        }
    }

    public bool checkUpdate(NPC.NPCs npc)
    {
        if (!info.obtained)
            return false;

        foreach (NPCStatus npcStatus in info.npcSstatus)
        {
            if ((npcStatus.npc == npc) && (npcStatus.status == Status.NotCommented))
            {
                return true;
            }
        }
        return false;
    }
    

    public void CommentedBy(NPC.NPCs npc)
    {
        SaveJson.ItemInfo[] infos = GameManager.GetInstance().GetCurrentSave().items;
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i].id != info.id)
                continue;

            for (int j = 0; j < infos[i].npcSstatus.Length; j++)
            {
                if (infos[i].npcSstatus[j].npc == npc)
                {
                    infos[i].npcSstatus[j].status = Status.Commented;
                    if (npc == NPC.NPCs.AbilityGuy)
                    {
                        infos[i].isActive = true;
                        UIManager.GetInstance().ability.ObtainNewAbility(info.id);
                    }
                    GameManager.GetInstance().saveJson.saves[GameManager.GetInstance().saveIndex].items = infos;
                    break;
                }
            }
        }
    }

}
