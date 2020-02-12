using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class Ability : MonoBehaviour
{

    #region Types

    public enum MenuState
    {
        Active,
        Passive,
        Scroll,
        ActiveSelection,
        PassiveSelection
    }
    
    public enum AbilityId
    {
        NONE,
        IncreaseDamage,
        GreaterIncreaseDamage,
        Thorns,
        Vampire,
        DamageConverter,
        IncreaseAttackSpeed,
        IncreaseCriticalChance,
        ReduceDamage,
        GreaterReduceDamage,
        Regen,
        ReduceFallingDamage,
        IncreaseRolling,
        IncreaseJumpHeight,
        IncreaseMoveSpeed,
        IncreaseDropRate,
        IncreaseScriptDropRate,
        IncreaseHealingRate,
        IncreaseUltimateRate,
        InfiniteMP,
        Invulnerability,
        Sonar,
        ManaShield,
        WallJump
    }

    [System.Serializable]
    public struct ActiveSettings
    {
        public GameObject instantiable;
        public float cooldown;
        public float upTime;
    }

    [System.Serializable]
    public class AbilitySettings
    {

        public Progress.GameItems itemID;
        public Texture2D texture;
        public Texture2D textureSelected;
        public Texture2D activated;
        public bool isActivatable;
        public ActiveSettings active;

        //[HideInInspector] TODO
        public AbilitiesDescriptionJson.Description description;
        //[HideInInspector] TODO
        public SaveJson.AbilitiesInfo saveInfo;

        public void Obtain()
        {
            saveInfo.obtained = true;
        }
    }

    [System.Serializable]
    public class Inputs : SerializableDictionary<MenuState, InputsDisplay> { }

    [System.Serializable]
    public class AbilityDictionary : SerializableDictionary<AbilityId, AbilitySettings> { }

    #endregion

    #region variables

    private static Ability ability;
    private UIManager manager;
    public CooldownHandler coolDown;

    public AbilityDictionary abilities;
    public AbilityDictionary test;
    public Inputs inputs;

    public Area menuArea;
    public Texture background;
    public Texture backgroundBorder;
    private float borderheight = 70;

    public Area activeMenuAreaOnPlay;
    public Area activeMenuAreaOnPause;
    public Area activeMenuAreaOnAbilityMenu;

    public Area passiveMenuAreaOnPause;
    public Area passiveMenuAreaOnAbilityMenu;

    public ActiveAbilitiesMenu activeMenu;
    public PassiveAbilitiesMenu passiveMenu;
    public SelectableAbilitiesMenu activeSelectMenu;
    public SelectableAbilitiesMenu passiveSelectMenu;

    public AbilityDetails details;

    public Area spacer;
    public Texture2D spacerTexture;

    private MenuState state;


    private Texture2D passiveMovingTexture = null;
    private Area passiveMovingArea = new Area();
    private bool isGoing = false;
    

    public static string cdIdentifier = "Ability";

    #endregion


    #region MonoBehavior


    void Awake()
    {
        if (ability == null)
            ability = this;
    }

        void Update()
    {
        foreach (KeyValuePair<MenuState, InputsDisplay> pair in inputs)
        {
            pair.Value.Init();
        }
    }

    void Start()
    {
        manager = UIManager.GetInstance();

        coolDown.init();

        activeMenu.menuArea.SetParent(menuArea);
        passiveMenu.menuArea.SetParent(menuArea);
        activeSelectMenu.menuArea.SetParent(menuArea);
        passiveSelectMenu.menuArea.SetParent(menuArea);
        spacer.SetParent(menuArea);

        foreach ( KeyValuePair< AbilityId, AbilitySettings> element in abilities )
        {
            element.Value.itemID = (Progress.GameItems) element.Key;
        }

    }

    public void Init()
    {
        //AbilitiesDescriptionJson json = new AbilitiesDescriptionJson();
        //List<AbilitiesDescriptionJson.Description> list = new List<AbilitiesDescriptionJson.Description>();
        //System.Globalization.TextInfo info = new System.Globalization.CultureInfo("en-US",false).TextInfo;
        foreach (KeyValuePair<AbilityId, AbilitySettings> element in abilities)
        {
            if (element.Value.texture != null)
                element.Value.saveInfo.obtained = true;

            //element.Value.description.id = element.Key;
            //element.Value.description.name = info.ToTitleCase(element.Key.ToString());
            //element.Value.description.description = element.Key.ToString().ToLower();
            //list.Add(element.Value.description);
        }
        //json.abilities = list.ToArray();

        //json.Save("Assets/Resources/JSON/ability_en.json");
        //json.Save("Assets/Resources/JSON/ability_pt.json");


        activeMenu.Init();
        passiveMenu.Init();
        activeSelectMenu.Init(ref abilities);
        passiveSelectMenu.isSelectable = (Ability.AbilitySettings ability) =>
        {
            return (!ability.isActivatable) && (ability.description.id != Ability.AbilityId.NONE);
        };
        passiveSelectMenu.Init(ref abilities);
        ChangeInternalState(MenuState.Active);
        details.Init();
        details.currentAbility = AbilityId.WallJump;
    }

    void FixedUpdate()
    {
        coolDown.update();
    }

    public void ObtainNewAbility(Progress.GameItems item)
    {
        ////print((("Obtain "+ item);
        foreach (AbilityId id in Enum.GetValues(typeof(AbilityId)))
        {
            if (id == AbilityId.NONE)
                continue;

            if (abilities[id].itemID == item)
            {
                AbilitySettings ability = abilities[id];
                ability.saveInfo.obtained = true;
                abilities[id] = ability;

                activeMenu.Init();
                passiveMenu.Init();
                activeSelectMenu.Init(ref abilities);
                passiveSelectMenu.isSelectable = (Ability.AbilitySettings abilities) =>
                {
                    return (!ability.isActivatable) && (ability.description.id != Ability.AbilityId.NONE);
                };
                passiveSelectMenu.Init(ref abilities);
                break;
            }
        }

    }

    #endregion

    #region OnGUI

    public void DisplayOnPlay()
    {

        /*GUI.DrawTexture(leftBorderMini, skillBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(leftSkillMini, abilitySettings[(int)ability1].texture, ScaleMode.StretchToFill, true, 0);

        float remainingTime = CoolDownManager.RemainingTime(cdIdentifier);
        remainingTime = 1 - remainingTime;
        if (remainingTime != 0)
        {
            Rect rectSpace = leftBorderMini;
            rectSpace.height *= 1 - remainingTime;
            Rect rectTex = new Rect(0, remainingTime, 1, 1 - remainingTime);
            //Rect rectText = new Rect(0, 0, 1, 1);

            GUI.DrawTextureWithTexCoords(rectSpace, skillBlackShader, rectTex, true);
        }

        GUI.DrawTexture(rightBorderMini, skillBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(rightSkillMini, abilitySettings[(int)ability2].texture, ScaleMode.StretchToFill, true, 0);*/
        activeMenu.Draw();


    }

    public void DisplayOnPause()
    {
        /*GUI.DrawTexture(leftBorder, skillBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(leftSkill, abilitySettings[(int)ability1].texture, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(rightBorder, skillBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(rightSkill, abilitySettings[(int)ability2].texture, ScaleMode.StretchToFill, true, 0);*/
        activeMenu.Draw();
        passiveMenu.Draw();

    }


    public void ChangeInternalState(MenuState state)
    {
        this.state = state;

        activeMenu.isActive = false;
        passiveMenu.isActive = false;
        activeSelectMenu.isActive = false;
        passiveSelectMenu.isActive = false;

        switch (state)
        {
            case MenuState.Active:
                activeMenu.isActive = true;
                break;

            case MenuState.Passive:
                passiveMenu.isActive = true;
                break;

            case MenuState.ActiveSelection:
                activeSelectMenu.isActive = true;
                break;

            case MenuState.PassiveSelection:
                passiveSelectMenu.isActive = true;
                break;
        }
    }

    public void UpdateAbilityMenu()
    {
        activeSelectMenu.CheckIndexes();
        passiveSelectMenu.CheckIndexes();
        passiveMovingArea.Update();

        if (isGoing)
            return;

        switch (state)
        {
            case MenuState.Active:
                activeMenu.Update();

                
                if (ButtonManager.GetValue(ButtonManager.ButtonID.R_DOWN) > 0.5f)
                {
                    if (passiveMenu.GetNumberOfSelectedAbilities() > 0)
                        ChangeInternalState(MenuState.Passive);
                }
                else if ((ButtonManager.GetValue(ButtonManager.ButtonID.R_RIGHT) > 0.5f) || (ButtonManager.GetDown(ButtonManager.ButtonID.X, this)))
                {
                    ChangeInternalState(MenuState.ActiveSelection);
                }

                if (ButtonManager.GetDown(ButtonManager.ButtonID.TRIANGLE,this))
                {
                    activeMenu.DeselectAbility();
                }

                details.currentAbility = activeMenu.GetHoveredAbility();
                break;

            case MenuState.Passive:
                passiveMenu.Update();
                activeMenu.menuArea.Update();

                if (ButtonManager.GetValue(ButtonManager.ButtonID.R_UP) > 0.5f)
                {
                    ChangeInternalState(MenuState.Active);
                }
                else if ((ButtonManager.GetValue(ButtonManager.ButtonID.R_RIGHT) > 0.5f) || (ButtonManager.GetDown(ButtonManager.ButtonID.X, this)))
                {
                    ChangeInternalState(MenuState.PassiveSelection);
                }

                if (ButtonManager.GetDown(ButtonManager.ButtonID.TRIANGLE, this))
                {
                    passiveMenu.DeselectAbility();
                }

                details.currentAbility = passiveMenu.GetHoveredAbility();
                break;

            case MenuState.ActiveSelection:
                activeSelectMenu.Update();

                if (ButtonManager.GetValue(ButtonManager.ButtonID.R_DOWN) > 0.5f)
                {
                    ChangeInternalState(MenuState.PassiveSelection);
                }
                else if (ButtonManager.GetValue(ButtonManager.ButtonID.R_LEFT) > 0.5f)
                {
                    ChangeInternalState(MenuState.Active);
                }

                bool newSelection = true;
                Area.Position dest = new Area.Position(0, 0);
                if (ButtonManager.GetDown(ButtonManager.ButtonID.X, this))
                {
                    newSelection = activeMenu.HasSlot(ActiveAbilitiesMenu.Options.DOWN);
                    activeMenu.ChangeSelected(ActiveAbilitiesMenu.Options.DOWN);
                    dest = activeMenu.GetSlot(ActiveAbilitiesMenu.Options.DOWN);
                }
                else if (ButtonManager.GetDown(ButtonManager.ButtonID.TRIANGLE, this))
                {
                    newSelection = activeMenu.HasSlot(ActiveAbilitiesMenu.Options.UP);
                    activeMenu.ChangeSelected(ActiveAbilitiesMenu.Options.UP);
                    dest = activeMenu.GetSlot(ActiveAbilitiesMenu.Options.UP);
                }
                else if (ButtonManager.GetDown(ButtonManager.ButtonID.CIRCLE, this))
                {
                    newSelection = activeMenu.HasSlot(ActiveAbilitiesMenu.Options.RIGHT);
                    activeMenu.ChangeSelected(ActiveAbilitiesMenu.Options.RIGHT);
                    dest = activeMenu.GetSlot(ActiveAbilitiesMenu.Options.RIGHT);
                }
                else if (ButtonManager.GetDown(ButtonManager.ButtonID.SQUARE, this))
                {
                    newSelection = activeMenu.HasSlot(ActiveAbilitiesMenu.Options.LEFT);
                    activeMenu.ChangeSelected(ActiveAbilitiesMenu.Options.LEFT);
                    dest = activeMenu.GetSlot(ActiveAbilitiesMenu.Options.LEFT);
                }
                else
                {
                    newSelection = false;
                }


                if (newSelection)
                {
                    if (activeSelectMenu.GetSelected().saveInfo.obtained)
                    {
                        if (activeMenu.TestAbility(activeSelectMenu.GetSelected()))
                        {
                            activeMenu.ChangeSelectedAbility(activeSelectMenu.GetSelected());
                        }
                        else
                        { 
                            isGoing = true;
                            passiveMovingTexture = activeSelectMenu.GetSelected().texture;
                            passiveMovingArea = activeSelectMenu.GetSelectedArea();
                            passiveMovingArea.GoTo(dest, () =>
                            {
                                isGoing = false;
                                passiveMovingTexture = null;
                                activeMenu.ChangeSelectedAbility(activeSelectMenu.GetSelected());
                            });
                        }
                    }
                }

                details.currentAbility = activeSelectMenu.GetHoveredAbility();

                break;

            case MenuState.PassiveSelection:
                passiveSelectMenu.Update();

                if (ButtonManager.GetValue(ButtonManager.ButtonID.R_UP) > 0.5f)
                {
                    ChangeInternalState(MenuState.ActiveSelection);
                }
                else if (ButtonManager.GetValue(ButtonManager.ButtonID.R_LEFT) > 0.5f)
                {
                    if (passiveMenu.GetNumberOfSelectedAbilities() > 0)
                        ChangeInternalState(MenuState.Passive);
                }

                if (ButtonManager.GetDown(ButtonManager.ButtonID.X, this))
                {
                    if (passiveSelectMenu.GetSelected().saveInfo.obtained)
                    {
                        if (!passiveMenu.IsSelectable(abilities[passiveSelectMenu.GetSelected().description.id]))
                        { 
                            passiveMenu.SelectAbility(passiveSelectMenu.GetSelected());
                        }
                        else
                        {
                            isGoing = true;
                            passiveMovingTexture = passiveSelectMenu.GetSelected().texture;
                            passiveMovingArea = passiveSelectMenu.GetSelectedArea();
                            //print("OIOI " +passiveMenu.GetNextSlot());
                            passiveMovingArea.GoTo(passiveMenu.GetNextSlot(), () =>
                            {
                                passiveMovingTexture = null;
                                passiveMenu.SelectAbility(passiveSelectMenu.GetSelected());
                                isGoing = false;
                            });
                        }
                    }
                }

                details.currentAbility = passiveSelectMenu.GetHoveredAbility();
                break;

        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.START, this))
        {
            manager.ChangeMode(UIManager.Mode.NORMAL);
        }


    }

    public void DisplayAbilityMenu()
    {
        DrawBackGround();
        if (spacerTexture != null)
            GUI.DrawTexture(spacer.GetRect(), spacerTexture, ScaleMode.ScaleToFit);

        activeMenu.Draw();
        passiveMenu.Draw();
        activeSelectMenu.Draw();
        passiveSelectMenu.Draw();

        if (passiveMovingTexture != null)
            GUI.DrawTexture(passiveMovingArea.GetRect(), passiveMovingTexture, ScaleMode.ScaleToFit);

        if (inputs.Contains(state))
        {
            inputs[state].Draw();
        }

        details.Draw();
    }

    #endregion
    

    public void ActivateAbility(AbilityId selectableAbility, Vector3 instantiatePosition)
    {
        if (abilities[selectableAbility].active.instantiable != null)
        {
            Instantiate(abilities[selectableAbility].active.instantiable, instantiatePosition, Quaternion.identity);
        }
    }

    public AbilitySettings AbilityInfo(AbilityId id)
    {
            return abilities[id];

    }


    protected void DrawBackGround()
    {
        /*BackGround = */
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill, true, 0);
        
        /*Background Border*/
        GUI.DrawTexture(new Rect(0, 0, Screen.width, borderheight), backgroundBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(new Rect(0, 0, borderheight, Screen.height), backgroundBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(new Rect(Screen.width - borderheight, 0, borderheight, Screen.height), backgroundBorder, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(new Rect(0, Screen.height - borderheight, Screen.width, borderheight), backgroundBorder, ScaleMode.StretchToFill, true, 0);
    }

    public void ChangeGameState(UIManager.Mode currentState, UIManager.Mode nextState)
    {
        //print("currentState = " + currentState + " ->> " + "nextState = " + nextState);
        switch (nextState)
        {
            case UIManager.Mode.PAUSED:
                passiveMenu.menuArea.CopyValues(passiveMenuAreaOnPause);
                activeMenu.menuArea.CopyValues(activeMenuAreaOnPause);
                break;
            case UIManager.Mode.NORMAL:
                activeMenu.menuArea.CopyValues(activeMenuAreaOnPlay);
                break;
            case UIManager.Mode.ABILITY:
                passiveMenu.menuArea.CopyValues(passiveMenuAreaOnAbilityMenu);
                activeMenu.menuArea.CopyValues(activeMenuAreaOnAbilityMenu);
                break;
        }
        activeMenu.menuArea.AdjustToSquare();
        switch (currentState)
        {
            case UIManager.Mode.ABILITY:
                UIManager.GetInstance().GetPlayer().VerifyAbilities();
                break;
            /*case UIManager.Mode.PAUSED:
                passiveMenu.menuArea.centerPosition = new Area.Position(1 - passiveMenu.menuArea.centerPosition.x, passiveMenu.menuArea.centerPosition.y);
                activeMenu.menuArea.centerPosition = new Area.Position(1 - activeMenu.menuArea.centerPosition.x, activeMenu.menuArea.centerPosition.y);
                break;*/
        }
        
    }

    public void ApplyJson(AbilitiesDescriptionJson json)
    {
        if (json == null)
        {
            return;
        }
        else
        {
            foreach (AbilitiesDescriptionJson.Description description in json.abilities)
            {
                if (!abilities.Contains(description.id))
                    continue;

                //print("Ability <" + description.id + ">: " + description.description);

                AbilitySettings ability = abilities[description.id];
                ability.description = description;
                abilities[description.id] = ability;
            }
        }


    }

    public void ApplyJson(SaveJson.AbilitiesInfo[] save)
    {
        if (save == null)
        {
            foreach (AbilityId id in Enum.GetValues(typeof(AbilityId)))
            {
                abilities[id].saveInfo = new SaveJson.AbilitiesInfo();
                abilities[id].saveInfo.id = id;
                abilities[id].saveInfo.isSelected = false;
                abilities[id].saveInfo.obtained = false;
            }
        }
        else
        {
            foreach (SaveJson.AbilitiesInfo info in save)
            {
                if (!abilities.Contains(info.id))
                    continue;

                AbilitySettings ability = abilities[info.id];
                ability.saveInfo = info;
                ability.saveInfo.id = info.id;
                abilities[info.id] = ability;
            }
        }

        Init();

    }

    public List<SaveJson.AbilitiesInfo> SaveJson()
    {
        List<SaveJson.AbilitiesInfo> saves = new List<SaveJson.AbilitiesInfo>();
        foreach (var pair in abilities)
        {
            saves.Add(pair.Value.saveInfo);
        }
        return saves;
    }

    public static bool IsSelected(AbilityId id)
    {
        return ability.abilities[id].saveInfo.isSelected;
    }

    public List<AbilitySettings> SelectedAbilities()
    {
        List<AbilitySettings> ret = new List<AbilitySettings>();

        foreach (KeyValuePair<AbilityId,AbilitySettings> element in abilities)
        {
            if (element.Value.saveInfo.isSelected)
                ret.Add(element.Value);
        }
        return ret;
    }

    public static AbilityId GetID(ActiveAbilitiesMenu.Options options)
    {
        return ability.activeMenu.GetID(options);
    }

    public static AbilitySettings GetAbilityInfo(AbilityId id)
    {
        return ability.AbilityInfo(id);

    }

    public static List<AbilitySettings> GetSelectedAbilities()
    {
        return ability.SelectedAbilities();
    }
    

}