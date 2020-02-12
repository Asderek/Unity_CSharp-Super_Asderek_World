using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Progress : MonoBehaviour {


    public enum GameItems
    {
        NONE,
        INCREASE_DAMAGE_ITEM,
        GREATER_INCREASE_DAMAGE_ITEM,
        THORNS_ITEM,
        VAMPIRE_ITEM,
        DAMAGE_CONVERTER_ITEM,
        INCREASE_ATTACK_SPEED_ITEM,
        INCREASE_CRITICAL_CHANCE_ITEM,
        REDUCE_DAMAGE_ITEM,
        GREATER_REDUCE_DAMAGE_ITEM,
        REGEN_ITEM,
        REDUCE_FALLING_DAMAGE_ITEM,
        INCREASE_ROLLING_ITEM,
        INCREASE_JUMP_HEIGHT_ITEM,
        INCREASE_MOVE_SPEED_ITEM,
        INCREASE_DROP_RATE_ITEM,
        INCREASE_SCRIPT_DROP_RATE_ITEM,
        INCREASE_HEALING_RATE_ITEM,
        INCREASE_ULTIMATE_RATE_ITEM,
        INFINITE_MP_ITEM,
        INVINCIBILITY_ITEM,
        SONAR_ITEM,
        MANA_SHIELD_ITEM,
        WALL_JUMP_ITEM,
        EARTH_ITEM,
        METAL_ITEM,
        ICE_ITEM,
        LIFE_ITEM,
        WATER_ITEM,
        DEATH_ITEM,
        WIND_ITEM,
        LIGHT_ITEM,
        LIGHTNING_ITEM,
        FIRE_ITEM,
        DARK_ITEM,
        WOOD_ITEM
    }

    [System.Serializable]
    public class ItensDictionary : SerializableDictionary<GameItems, ProgressItem> { }

    [System.Serializable]
    public class ItemSpriteDictionary : SerializableDictionary<GameItems, GameObject> { }


    public ItensDictionary items;
    public ItemSpriteDictionary sprites;
    public GameObject defaultSprite;
    
    private static Progress progress;

    public static Progress GetInstance()
    {
        if (progress != null)
            return progress;
        throw new System.Exception("gameManager = null");
    }

    void Awake()
    {
        if (progress == null)
            progress = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Init() {
        items.Clear();
        foreach (GameItems id in System.Enum.GetValues(typeof(GameItems)))
        {
            ProgressItem item = new ProgressItem();
            item.Init(id);
            items[id] = item;
        }

    }

    public void AddItem(GameItems type)
    {
        items[type].info.obtained = true;
    }

    public GameItems CheckUpdate(NPC.NPCs npc)
    {
        foreach (GameItems id in System.Enum.GetValues(typeof(GameItems)))
        {
            if (items[id].checkUpdate(npc))
                return id;
        }

        return GameItems.NONE;
    }
    
    public void CommentedBy(NPC.NPCs npc, Progress.GameItems id)
    {
        items[id].CommentedBy(npc);
    }

    public bool Active(Progress.GameItems id)
    {
        return items[id].info.obtained && items[id].info.isActive;
    }

    public List<SaveJson.ItemInfo> SaveJson()
    {
        List<SaveJson.ItemInfo> save = new List<SaveJson.ItemInfo>();
        foreach (GameItems id in System.Enum.GetValues(typeof(GameItems)))
        {
            save.Add(items[id].info);
        }
        return save;
    }

    public static GameObject GetSprite(GameItems id)
    {
        if (progress.sprites.ContainsKey(id))
            return progress.sprites[id];

        return progress.defaultSprite;
    }

}
