using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMenu : MonoBehaviour {

    [System.Serializable]
    public enum MenuItemId
    {
        Continue,
        New_Game,
        Load_Game,
        Options,
        Exit,
        Start_Menu
    }

    [System.Serializable]
    public class Inputs : SerializableDictionary<StarMenu.MenuItemId, InputsDisplay> { }

    public StarMenu.MenuItemId state;
    public TextureBox logo;
    public TextMenu textMenu;

    public OptionsMenu options;
    public TextureBox loadZone;
    public LoadingMenu load;
    public Inputs inputs;


    private GameManager gameManager;
    private UIManager uIManager;

    private bool hasSomeSave;
    private bool hasSelectedSave;

    void Start ()
    {

        gameManager = GameManager.GetInstance();
        uIManager = UIManager.GetInstance();

        foreach (KeyValuePair< StarMenu.MenuItemId, InputsDisplay> pair in inputs )
        {
            pair.Value.Init();
        }

        Init();
    }

    public void Init()
    {        
        hasSomeSave = gameManager.saveJson.saves.Length != 0;
        hasSelectedSave = gameManager.saveIndex != -1 && hasSomeSave;

        List<string> items = new List<string>();
        foreach (MenuJson.MainMenuItem menuItem in gameManager.menuJson.mainMenu)
        {
            switch (menuItem.id)
            {
                case StarMenu.MenuItemId.Continue:
                    if (!hasSelectedSave)
                        continue;
                    break;

                case StarMenu.MenuItemId.Load_Game:
                    if (!hasSomeSave)
                        continue;
                    break;
            }
            items.Add(menuItem.name);
        }

        textMenu.items = items.ToArray();

        logo.Init();
        textMenu.Init(true, true);
        options.Init();
        loadZone.Init();
        load.menuArea.SetParent(loadZone.menuArea);
        load.Init();

    }

    public void UpdateOnStart ()
    {
        logo.Update();
        switch (state)
        {
            case StarMenu.MenuItemId.Start_Menu:
                textMenu.Update();

                if (ButtonManager.GetDown(ButtonManager.ButtonID.X,this))
                {
                    StarMenu.MenuItemId nextState = (StarMenu.MenuItemId)(textMenu.menuIndex.Get());
                    if (!hasSelectedSave)
                        nextState++;

                    if (!hasSomeSave)
                    {
                        if (nextState >= StarMenu.MenuItemId.Load_Game)
                        {
                            nextState++;
                        }

                    }

                    switch (nextState)
                    {

                        case StarMenu.MenuItemId.New_Game:
                            gameManager.NewGame();
                            break;

                        case StarMenu.MenuItemId.Continue:
                            gameManager.LoadGame();
                            break;

                        case StarMenu.MenuItemId.Exit:
                            Application.Quit();
                            break;

                        default:
                            ChangeState(nextState);
                            break;
                    }
                }

                break;

            case StarMenu.MenuItemId.Options:
                options.Update();

                if (ButtonManager.GetDown(ButtonManager.ButtonID.X, this))
                {
                    for (int i = 0; i < gameManager.menuJson.optionsMenu.Length; i++)
                    {
                        for (int j = 0; j < gameManager.menuJson.optionsMenu[i].items.Length; j++)
                        {
                            switch (gameManager.menuJson.optionsMenu[i].items[j].type)
                            {
                                case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Special:
                                    switch (gameManager.menuJson.optionsMenu[i].items[j].special)
                                    {
                                        case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.Language:
                                            break;

                                        case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.TitleFont:
                                            gameManager.saveJson.config.titleFontIndex = gameManager.menuJson.optionsMenu[i].items[j].comboSelected.Get();
                                            break;

                                        case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.TextFont:
                                            gameManager.saveJson.config.textFontIndex = gameManager.menuJson.optionsMenu[i].items[j].comboSelected.Get();
                                            break;

                                        case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.ComboTexture:
                                            ButtonManager.SetControllerTexture( (ButtonManager.Controll) gameManager.menuJson.optionsMenu[i].items[j].comboSelected.Get());
                                            break;

                                    }
                                    break;

                            }

                        }
                    }
                    gameManager.ChangeLanguage(gameManager.languagues[gameManager.menuJson.optionsMenu[0].items[0].comboSelected.Get()].name);
                    ChangeState(StarMenu.MenuItemId.Start_Menu);
                }
                
                break;

            case StarMenu.MenuItemId.Load_Game:
                loadZone.Update();
                load.Update();

                if (ButtonManager.GetDown(ButtonManager.ButtonID.TRIANGLE, this))
                {
                    gameManager.RemoveGame(load.menuIndex.Get());
                    if (gameManager.saveJson.saves.Length == 0)
                        ChangeState(StarMenu.MenuItemId.Start_Menu);
                    load.Init();

                } else if (ButtonManager.GetDown(ButtonManager.ButtonID.X, this))
                {
                    gameManager.LoadGame(load.menuIndex.Get());
                }
                break;

            default:
                ChangeState(StarMenu.MenuItemId.Start_Menu);
                break;

        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.CIRCLE, this))
        {
            ChangeState(StarMenu.MenuItemId.Start_Menu);
        }
    }

    public void ChangeState(StarMenu.MenuItemId nextState)
    {
        state = nextState;
        if (nextState == StarMenu.MenuItemId.Start_Menu)
        {
            Init();
        }
    }

    public void DisplayOnStart()
    {
        switch (state)
        {
            case StarMenu.MenuItemId.Start_Menu:
                logo.Draw();
                textMenu.Draw();
                break;

            case StarMenu.MenuItemId.Options:
                logo.DrawContext();
                options.Draw();
                break;

            case StarMenu.MenuItemId.Load_Game:
                logo.DrawContext();
                loadZone.Draw();
                load.Draw();
                break;
        }

        if (inputs.Contains(state))
        {
            inputs[state].Draw();
        }
    }
}
