using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using System;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct Language
    {
        public string name;
        public TextAsset yokaiJson;
        public TextAsset npcJson;
        public TextAsset abilityJson;
        public TextAsset menuJson;
    }

    [System.Serializable]
    public struct FontSetting
    {
        public string name;
        public Font font;
    }

    private static GameManager gameManager;
    public static float globalZero = 1E-5f;

    private UIManager uiManager;
    private Asderek player;
    private BaseCamera currentCamera;
    public GameObject yokaiEssence;

    public List<GameObject> persistsObjects;  

    [HideInInspector]
    public MenuJson menuJson = new MenuJson();
    private YokaisJson yokaiDescriptionsJson = new YokaisJson();
    public NPCJson npcsJson = new NPCJson();
    public AbilitiesDescriptionJson abilitiesJson = new AbilitiesDescriptionJson();

    public SaveJson saveJson;
    public int saveIndex;
    
    public Language[] languagues;
    public LimitedIndex currentLanguage;
    public FontSetting[] fonts;
    private GUIStyle titleStyle;
    private GUIStyle textStyle;
    

    #region Behavior

    public static GameManager GetInstance()
    {
        if (gameManager != null)
            return gameManager;
        throw new System.Exception("gameManager = null");
    }
    
    void Awake()
    {

        if (gameManager == null)
            gameManager = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        PersistObjets();

        saveJson.Load("save.json");

        currentLanguage = new LimitedIndex(languagues.Length);
        
        for (int i=0; i < languagues.Length; i++)
        {
            if (languagues[i].name == saveJson.config.languageName)
            {
                currentLanguage.Set(i);
            }
        }

        if (languagues.Length > 0)
            ChangeLanguage(languagues[currentLanguage.Get()].name,true);

        saveIndex = 0   ;
        
    }
    
    void Update()
    {
        ApplyOnce.apply("StartGame",gameObject,() => {
            uiManager = UIManager.GetInstance();
            UpdateLanguage();
            return true;
        });

        //if(Input.GetButtonUp("SELECT"))
        //{
        //    if (saveIndex >= 0) { 
        //        saveJson.saves[saveIndex].saveGame();
        //        saveJson.config.lastSave = saveIndex;
        //        saveJson.Save("save.json");
        //    }
        //}
    }

    void LateUpdate()
    {
        CoolDownManager.clean();
    }

    private void OnDestroy()
    {
        SaveGame();
    }
    
    public void PersistObjets()
    {

        foreach (GameObject obj in persistsObjects)
        {
            DontDestroyOnLoad(obj);
        }
        DontDestroyOnLoad(gameObject);

    }
    
    public void SpawnYokaiEssence(Vector3 startPosition, int yokaiRank, GameObject target)
    {
        GameObject newYokaiEssence = Instantiate(yokaiEssence, startPosition, Quaternion.identity);
        newYokaiEssence.GetComponent<ScrollMovement>().rank = yokaiRank;
        newYokaiEssence.GetComponent<ScrollMovement>().target = target;
    }

    #endregion

    #region TextAndLanguages

    public void ChangeLanguage(string name, bool force = false)
    {
        if ((languagues[currentLanguage.Get()].name == name) && (!force))
            return;

        for (int i = 0; i < languagues.Length; i++)
        {
            if (languagues[i].name == name)
            {
                currentLanguage.Set(i);
                yokaiDescriptionsJson.Load(languagues[i].yokaiJson);
                Beastiary beastiary = GetComponentInChildren<Beastiary>();
                foreach (YokaisJson.Description desc in yokaiDescriptionsJson.description)
                {
                    beastiary.yokais[desc.id].name = desc.name;
                    beastiary.yokais[desc.id].description = desc.formatedText;
                }

                npcsJson.Load(languagues[i].npcJson);

                abilitiesJson.Load(languagues[i].abilityJson);
            
                menuJson.Load(languagues[i].menuJson);
                //print("Update menuJson Json Description: " + languagues[i].menuJson.name);
                //print("abilitiesJson: " + menuJson.mainMenu[0].name);

                for (int j = 0; j < menuJson.optionsMenu.Length; j++)
                {
                    for (int k = 0; k < menuJson.optionsMenu[j].items.Length; k++)
                    {

                        switch (menuJson.optionsMenu[j].items[k].type)
                        {
                            case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Combo:
                                menuJson.optionsMenu[j].items[k].comboSelected = new CircularIndex(menuJson.optionsMenu[j].items[k].combo.Length);
                                break;

                            case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Special:
                                switch (menuJson.optionsMenu[j].items[k].special)
                                {
                                    case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.Language:
                                        menuJson.optionsMenu[j].items[k].comboSelected = new CircularIndex(languagues.Length);
                                        break;

                                    case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.TitleFont:
                                        menuJson.optionsMenu[j].items[k].comboSelected = new CircularIndex(fonts.Length);
                                        menuJson.optionsMenu[j].items[k].comboSelected.Set(saveJson.config.titleFontIndex);
                                        break;

                                    case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.TextFont:
                                        menuJson.optionsMenu[j].items[k].comboSelected = new CircularIndex(fonts.Length);
                                        menuJson.optionsMenu[j].items[k].comboSelected.Set(saveJson.config.textFontIndex);
                                        break;

                                }
                                break;
                        }

                    }

                }


                if (ApplyOnce.alreadyApplied("StartGame", gameObject))
                {
                    UpdateLanguage();
                }

                break;
            }
        }


    }

    private void UpdateLanguage()
    {
        menuJson.Init();
        uiManager.start.Init();
        uiManager.ability.ApplyJson(abilitiesJson);
    }

    public GUIStyle TitleStyle()
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 32;
        titleStyle.font = fonts[saveJson.config.titleFontIndex].font;
        titleStyle.wordWrap = true;
        titleStyle.normal.textColor = Color.white;
        return titleStyle;
    }

    public GUIStyle TextStyle()
    {
        GUIStyle textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontSize = 30;
        textStyle.font = fonts[saveJson.config.textFontIndex].font;
        textStyle.normal.textColor = Color.white;
        textStyle.wordWrap = true;
        return textStyle;
    }

    #endregion

    #region Save

    public Commandments.Shrines getCurrentRespawn()
    {
        return gameManager.saveJson.saves[saveIndex].respawPoint;
    }

    public SaveJson.SaveGame GetCurrentSave()
    {
        if ((saveIndex < 0) || (saveIndex > saveJson.saves.Length))
        {
            SaveJson.SaveGame save = new SaveJson.SaveGame();
            return save;
        }


        return saveJson.saves[saveIndex];
    }

    public void PrepareCurrentSave()
    {
        //TODO
        uiManager.weapon.ApplyJson(saveJson.saves[saveIndex].weapons);
        uiManager.ability.ApplyJson(saveJson.saves[saveIndex].abilities);
        uiManager.progress.Init();
        gameManager.saveJson.saves[saveIndex].StartGameTime();
        LoadManager.getInstance().Reload(gameManager.saveJson.saves[saveIndex].respawPoint);
    }

    public void NewGame()
    {
        List<SaveJson.SaveGame> saves = new List<SaveJson.SaveGame>(gameManager.saveJson.saves);
        SaveJson.SaveGame save = new SaveJson.SaveGame();
        save.percentComplete = 0;
        save.respawPoint = Commandments.Shrines.NEXUS;
        saves.Add(save);
        gameManager.saveJson.saves = saves.ToArray();
        saveIndex = gameManager.saveJson.saves.Length - 1;
        PrepareCurrentSave();
    }

    public void RemoveGame(int index)
    {
        if ((index < 0) || (index > saveJson.saves.Length))
        {
            return;
        }

        List<SaveJson.SaveGame> saves = new List<SaveJson.SaveGame>(gameManager.saveJson.saves);
        saves.Remove(saves[index]);
        gameManager.saveJson.saves = saves.ToArray();
        if (saveIndex == index)
        {
            saveIndex = -1;
        } else if (saveIndex > index)
        { 
            saveIndex--;
        }
    }

    public void LoadGame()
    {
        LoadGame(gameManager.saveJson.config.lastSave);
    }
    
    public void LoadGame(int index)
    {
        if ((index < 0) || (index > saveJson.saves.Length))
        {
            return;
        }

        saveIndex = index;
        PrepareCurrentSave();
    }

    public void SaveGame()
    {
        if (saveIndex >= 0)
        {
            saveJson.saves[saveIndex].UpdateGameTime();
            saveJson.config.lastSave = saveIndex;
            saveJson.config.languageName = languagues[currentLanguage.Get()].name;
            saveJson.saves[saveIndex].respawPoint = LoadManager.getInstance().GetCurrentShrine();
            saveJson.saves[saveIndex].weapons = uiManager.weapon.SaveJson().ToArray();
            saveJson.saves[saveIndex].abilities = uiManager.ability.SaveJson().ToArray();
            saveJson.saves[saveIndex].items = uiManager.progress.SaveJson().ToArray();
            saveJson.saves[saveIndex].items = uiManager.progress.SaveJson().ToArray();
            saveJson.saves[saveIndex].UpdateGameProgress();
            saveJson.Save("save.json");
        }
    }

    public void ExitGame()
    {
        SaveGame();
        saveJson.saves[saveIndex].StopGameTime();
    }

    #endregion

    #region Register

    public void RegisterPlayer(Asderek player)
    {
        this.player = player;
    }

    public Asderek getPlayer()
    {
        return player;
    }

    public void RegisterCamera(BaseCamera camera)
    {
        // if  (camera)
        //print("Register new camera on  scene " + SceneManager.GetActiveScene().name);
        //else
        //print("Unregister camera on  scene " + SceneManager.GetActiveScene().name);
        currentCamera = camera;
    }

    public BaseCamera getCurrentCamera()
    {
        //return GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<BaseCamera>();
        return currentCamera;
    }

    #endregion

    #region References

    public void UpdateNPC(ref NPCJson.NPC_Message npc)
    {
        foreach (NPCJson.NPC_Message npcTest in npcsJson.npcs)
        {
            if (npc.name == npcTest.name)
            {
                npc = npcTest; 
            }
        }
    }

    public void GetMenuReference(ref MenuJson json)
    {
        json = menuJson;
    }

    #endregion
}

//Yokais_Descriptions descriptions = new Yokais_Descriptions();
//descriptions.description = new Yokais_Descriptions.Description[2];

//descriptions.description[0].id = 0;
//descriptions.description[0].name = "My Name";
//descriptions.description[0].formatedText = "Here all the text\n\tWith this type of format.";

//descriptions.description[0].id = 1;
//descriptions.description[1].name = "My Name 2";
//descriptions.description[1].formatedText = "format.";

//string json = JsonUtility.ToJson(descriptions);
//print(((json);
//SaveItemInfo(json);



//public void SaveNPC(NPC npc) {
//    npcs.npc = new NPCs_Messages.NPC_Message[1];
//    npcs.npc[0] = npc.npc;

//    string json = JsonUtility.ToJson(npcs);
//    //print(((json);
//    SaveItemInfo(json,NPCs_Messages.assetName);
//}
