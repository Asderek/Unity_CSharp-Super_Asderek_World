using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class OptionsMenu
{
    [System.Serializable]
    public class OptionComboTexture
    {
        public Area comboTextureLabel;
        public Area comboTextureTexture;
    }

    [System.Serializable]
    public class OptionMenuType : AbstractMenu
    {
        private AbstractMenu controller;
        private AbstractMenu parentMenu;
        private OptionComboTexture optionCombo;
        public Slider slider;
        private GameManager gameManager;
        private int lastHorizontal;
        private int shouldWait;
        public int waitTicks;


        public void Init(AbstractMenu controller, AbstractMenu parentMenu, OptionComboTexture optionCombo)
        {
            this.controller = controller;
            this.parentMenu = parentMenu;
            this.optionCombo = optionCombo;
            gameManager = GameManager.GetInstance();
            //print("OptionsMenuType.Init -> Controler.Get"+ controller.menuIndex.Get());
            base.Init(new LimitedIndex(parentMenu.menuIndex.Max()));
            
        }

        public override void DrawElement(Rect rect, int elementIndex, bool isActive)
        {
            if (elementIndex >= gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items.Length)
                return;

            MenuJson.OptionMenuTab.OptionsMenuItem opt = gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[elementIndex];

            GUIStyle style = gameManager.TextStyle();
            style.fontSize = (int)(style.fontSize * Mathf.Min(menuArea.GetPercentSize(), 1f));

            switch (opt.type)
            {
                case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Bool:
                    if (opt.isOn)
                    {
                        GUI.Label(rect, "ON", style);
                    }
                    else
                    {
                        GUI.Label(rect, "OFF", style);
                    }
                    break;

                case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Combo:
                    GUI.Label(rect, opt.combo[opt.comboSelected.Get()], style);
                    break;

                case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Float:
                    slider.Display(rect, opt.value, opt.valueMin, opt.valueMax);
                    break;

                case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Special:
                    switch (opt.special)
                    {
                        case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.TitleFont:
                        case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.TextFont:
                            style.font = gameManager.fonts[opt.comboSelected.Get()].font;
                            GUI.Label(rect, gameManager.fonts[opt.comboSelected.Get()].name, style);
                            break;

                        case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.Language:
                            GUI.Label(rect, gameManager.languagues[opt.comboSelected.Get()].name, style);
                            break;

                        case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.ComboTexture:
                            GUI.Label(optionCombo.comboTextureLabel.GetRect(rect), ((ButtonManager.Controll) opt.comboSelected.Get()).ToString(), style);
                            if (ButtonManager.GetControllerTexture((ButtonManager.Controll)opt.comboSelected.Get()))
                                GUI.DrawTexture(optionCombo.comboTextureTexture.GetRect(rect),ButtonManager.GetControllerTexture((ButtonManager.Controll)opt.comboSelected.Get()));
                            break;

                        default:
                            break;
                    }
                    break;
            }
        }

        public override void Update()
        {
            base.Update();
            if (shouldWait != 0)
            {
                shouldWait--;
                return;
            }
            if (ButtonManager.Get(ButtonManager.ButtonID.DIRECT_LEFT))
            {
                
                switch (gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].type)
                {
                    case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Float:
                        if (lastHorizontal < -10)
                        {
                            gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].Decrement(0.05f);
                        }
                        else
                        {
                            gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].Decrement();
                        }
                        shouldWait = waitTicks;
                        if (lastHorizontal > 0)
                        {
                            lastHorizontal = 0;
                        }
                        lastHorizontal--;
                        break;

                    case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Bool:
                        shouldWait = waitTicks;
                        gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].isOn =
                        !gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].isOn;
                        break;

                    case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Combo:
                    case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Special:
                        shouldWait = waitTicks;
                        gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].comboSelected--;
                        break;

                }
            }
            else if (ButtonManager.Get(ButtonManager.ButtonID.DIRECT_RIGHT))
            {
                switch (gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].type)
                {
                    case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Float:
                        if (lastHorizontal > 10)
                        {
                            gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].Increment(0.05f);
                        }
                        else
                        {
                            gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].Increment();
                        }
                        shouldWait = waitTicks;

                        if (lastHorizontal < 0)
                        {
                            lastHorizontal = 0;
                        }
                        lastHorizontal++;
                        break;

                    case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Bool:
                        shouldWait = waitTicks;
                        gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].isOn =
                        !gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].isOn;
                        break;

                    case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Combo:
                    case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Special:
                        shouldWait = waitTicks;
                        gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[parentMenu.menuIndex.Get()].comboSelected++;
                        break;

                }
            }
            else
            {
                lastHorizontal = 0;
            }
        }

        public override float GetAxis()
        {
            return 0;
        }
    }
    
    [System.Serializable]
    public class OptionMenuItem : TextMenu
    {
        private GameManager gameManager;
        private AbstractMenu controller;
        private OptionComboTexture optionCombo;

        public void Init(AbstractMenu controller, OptionComboTexture optionCombo)
        {
            base.Init(true,true);
            gameManager = GameManager.GetInstance();
            this.controller = controller;
            this.optionCombo = optionCombo;
        }

        public override void DrawElement(Rect rect, int elementIndex, bool isActive)
        {
            if (elementIndex >= gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items.Length)
                return;

            MenuJson.OptionMenuTab.OptionsMenuItem opt = gameManager.menuJson.optionsMenu[controller.menuIndex.Get()].items[elementIndex];

            switch (opt.type)
            {

                case MenuJson.OptionMenuTab.OptionsMenuItem.Type.Special:
                    switch (opt.special)
                    {
                         case MenuJson.OptionMenuTab.OptionsMenuItem.SpecialType.ComboTexture:
                            base.DrawElement( optionCombo.comboTextureLabel.GetRect(rect), elementIndex, isActive);
                            return;

                        default:
                            break;
                    }
                    break;
            }

            base.DrawElement(rect, elementIndex, isActive);
        }

    }

    public TextureBox topBar;
    public TextMenu top;

    public TextureBox bottomMenu;
    public OptionMenuItem bottomLeft;

    public OptionMenuType bottomRight;
    public OptionComboTexture optionComboTexture;

    public void Init()
    {
        List<string> items = new List<string>();
        foreach (MenuJson.OptionMenuTab option in GameManager.GetInstance().menuJson.optionsMenu)
        {
            items.Add(option.tabName);
        }
        top.items = items.ToArray();
        topBar.Init();
        top.Init(false, true);

        top.ChangeButtons(ButtonManager.ButtonID.L1, ButtonManager.ButtonID.R1);

        bottomLeft.menuArea.SetParent(bottomMenu.menuArea);
        bottomRight.menuArea.SetParent(bottomMenu.menuArea);
        BottomMenuChange(0, true);

    }
 
    public void Update()
    {

        topBar.Update();

        int before = top.menuIndex.Get();
        top.Update();

        if (top.menuIndex.Get() != before)
        {
            BottomMenuChange(top.menuIndex.Get());
            top.menuIndex.Set(before);
        }

        bottomMenu.Update();

        before = bottomLeft.menuIndex.Get();
        bottomLeft.Update();

        if (bottomLeft.menuIndex.Get() != before)
        {
        }
        bottomRight.Update();
    }
    
    public void Draw()
    {
        topBar.Draw();
        top.Draw();
        bottomMenu.Draw();
        bottomLeft.Draw();
        bottomRight.Draw();
    }

    public void BottomMenuChange(int index, bool start = false)
    {
        if (index >= GameManager.GetInstance().menuJson.optionsMenu.Length)
        {
            return;
        }

        if (start)
        {
            top.menuIndex.Set(index);
            List<string> items = new List<string>();
            foreach (MenuJson.OptionMenuTab.OptionsMenuItem option in GameManager.GetInstance().menuJson.optionsMenu[index].items)
            {
                items.Add(option.name);
            }

            bottomLeft.items = items.ToArray();
            bottomLeft.Init(top, optionComboTexture);
            bottomRight.Init(top, bottomLeft, optionComboTexture);

        }
        else
        {
            bottomMenu.menuArea.Reduce(() => {
                top.menuIndex.Set(index);
                List<string> items = new List<string>();
                foreach (MenuJson.OptionMenuTab.OptionsMenuItem option in GameManager.GetInstance().menuJson.optionsMenu[index].items)
                {
                    items.Add(option.name);
                }

                bottomLeft.items = items.ToArray();
                bottomLeft.Init(top, optionComboTexture);
                bottomRight.Init(top, bottomLeft, optionComboTexture);

                bottomMenu.menuArea.Grow();
            });

        }


    }
}