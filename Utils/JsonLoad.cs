using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class JsonLoad
{
    public virtual void Load(TextAsset asset)
    {
        string json = asset.text;
        UnityEngine.//Debug.Log(asset.name + ":\n "+json);
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public virtual void Save(string filePath)
    {
        string json = JsonUtility.ToJson(this);
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }

        UnityEditor.AssetDatabase.Refresh();

    }
}

[System.Serializable]
public class YokaisJson : JsonLoad
{
    [System.Serializable]
    public struct Description
    {
        public int id;
        public string name;
        public string formatedText;

        public override string ToString()
        {
            return "Yokai [" + id + "] \n"
                + "\tName: " + name
                + "\tDescription: " + formatedText;
        }
    }

    public Description[] description;

}

[System.Serializable]
public class NPCJson : JsonLoad
{
    [System.Serializable]
    public struct Message
    {
        public string text;
        public float time;
    }
    
    [System.Serializable]
    public struct ItemMsg
    {
        public Progress.GameItems id;
        public Message[] msgs;
    }

    [System.Serializable]
    public struct NPC_Message
    {
        public NPC.NPCs id;
        public string name;
        //[HideInInspector]
        public Message[] hiMessage;
        //[HideInInspector]
        public Message[] farewellMessage;

        public ItemMsg[] itensMessage;

        public override string ToString()
        {
            return "NPC: " + name;
        }
    }

    public NPC_Message[] npcs;
}

[System.Serializable]
public class AbilitiesDescriptionJson : JsonLoad
{
    [System.Serializable]
    public struct Description
    {
        public Ability.AbilityId id;
        public string name;
        public string description;


        public override string ToString()
        {
            return "Ability[" + id + "]\n\tName: " + name + "\n\tDescription: " + description;
        }
    }

    public Description[] abilities;
}

[System.Serializable]
public class MenuJson : JsonLoad
{ 
    [System.Serializable]
    public class OptionMenuTab 
    {
        [System.Serializable]
        public class OptionsMenuItem
        {
            [System.Serializable]
            public enum Type
            {
                Float,
                Bool,
                Combo,
                Special
            }

            [System.Serializable]
            public enum SpecialType
            {
                TitleFont,
                TextFont,
                Language,
                ComboTexture
            }
            
            public string name;

            public Type type;

            public float value;
            public float valueMax;
            public float valueMin;

            public bool isOn;

            public string[] combo;
            public CircularIndex comboSelected;

            public SpecialType special;

            public void Increment(float factor = 0.01f)
            {
                value += (valueMax - valueMin) * factor;
                if (value > valueMax)
                {
                    value = valueMax;
                }
            }

            public void Decrement(float factor = 0.01f)
            {
                value -= (valueMax - valueMin) * factor;
                if (value < valueMin)
                {
                    value = valueMin;
                }
            }
        }
                
        public OptionsMenuItem[] items;
        public string tabName;
    }
    
    [System.Serializable]
    public struct MainMenuItem
    {
        public StarMenu.MenuItemId id;
        public string name;
    }
        
    public OptionMenuTab[] optionsMenu;
    public MainMenuItem[] mainMenu;

    public void Init()
    {
        for (int i= 0; i < optionsMenu.Length; i++)
        {
            for( int j = 0; j < optionsMenu[i].items.Length; j++)
            {
                switch (optionsMenu[i].items[j].type)
                {
                    case OptionMenuTab.OptionsMenuItem.Type.Special:
                        switch(optionsMenu[i].items[j].special)
                        {
                            case OptionMenuTab.OptionsMenuItem.SpecialType.ComboTexture:
                                optionsMenu[i].items[j].comboSelected = new CircularIndex(Enum.GetValues(typeof(ButtonManager.Controll)).Length);
                                optionsMenu[i].items[j].comboSelected.Set((int)ButtonManager.GetInstance().currentControll);
                                break;

                            case OptionMenuTab.OptionsMenuItem.SpecialType.Language:
                                optionsMenu[i].items[j].comboSelected = new CircularIndex(GameManager.GetInstance().languagues.Length);
                                optionsMenu[i].items[j].comboSelected.Set(GameManager.GetInstance().currentLanguage.Get());
                                break;
                        }
                        break;
                }
            }
        }
    }


}

[System.Serializable]
public class SaveJson : JsonLoad
{
    public override void Save(string name)
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(name, json);
        base.Save("Assets/Resources/JSON/" + name);
    }

    public void Load(string name)
    {
        string json = PlayerPrefs.GetString(name);
        //UnityEngine.//Debug.Log(json);
        JsonUtility.FromJsonOverwrite(json, this);
    }
    
    [System.Serializable]
    public struct GameTime
    {
        public int hours;
        public int minutes;
        public int seconds;

        public void add(TimeSpan span)
        {
            seconds += span.Seconds;
            minutes += seconds / 60;
            seconds = seconds % 60;

            minutes += span.Minutes;
            hours += seconds / 60;
            minutes = minutes % 60;

            hours += span.Hours;
        }

        public override string ToString ()
        {
            return "" + ((hours < 10) ? ("0" + hours) : "" + hours) + ":"
                      + ((minutes < 10) ? ("0" + minutes) : "" + minutes) + ":"
                      + ((seconds < 10) ? ("0" + seconds) : "" + seconds);
        }
    }

    [System.Serializable]
    public struct WeaponsInfo
    {
        public Commandments.Weapon id;
        public bool obtained;
    }

    [System.Serializable]
    public struct AbilitiesInfo
    {
        [HideInInspector]
        public Ability.AbilityId id;
        public bool obtained;
        public bool isSelected;
        public ActiveAbilitiesMenu.Options selectLocation;
    }

    [System.Serializable]
    public struct ItemInfo
    {
        public Progress.GameItems id;
        public ProgressItem.NPCStatus[] npcSstatus;
        public bool obtained;
        public bool isActive;
    }

    [System.Serializable]
    public class SaveGame
    {
        public GameTime time;
        public float percentComplete;
        public Commandments.Shrines respawPoint;
        public WeaponsInfo[] weapons;
        public AbilitiesInfo[] abilities;
        public ItemInfo[] items;

        private Stopwatch watch;

        public void StartGameTime()
        {
            watch = System.Diagnostics.Stopwatch.StartNew();
        }

        public void StopGameTime()
        {
            watch.Stop();
        }

        public void UpdateGameTime()
        {
            if (watch != null)
            {
                watch.Stop();
                time.add(watch.Elapsed);
                watch.Reset();
                watch.Start();
            }
        }

        public void UpdateGameProgress()
        {
            float cont = 0;

            foreach (WeaponsInfo info in weapons)
            {
                if (info.obtained)
                    cont++;
            }

            foreach (AbilitiesInfo info in abilities)
            {
                if (info.obtained)
                    cont++;
            }

            foreach (ItemInfo info in items)
            {
                if (info.obtained)
                    cont++;
            }

            percentComplete = (float)((int) (10000 * cont / ((float)(items.Length + abilities.Length + weapons.Length))))/100f;


        }
    }

    [System.Serializable]
    public class GeralConfiguration
    {
        public int titleFontIndex;
        public int textFontIndex;
        public int lastSave;
        public string languageName;
    }
    
    public SaveGame[] saves;
    public GeralConfiguration config;
}