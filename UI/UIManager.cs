using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System;
using System.Linq;

public class UIManager : MonoBehaviour
{

    public Asderek player;

    public BarDisplay hp;
    public Weapons weapon;
    public PauseMenu pause;
    public ButtonManager buttonManager;
    public Interaction interaction;
    public Ability ability;
    public Beastiary beastiary;
    public Progress progress;
    public Warp warp;
    public StarMenu start;

    public Texture2D GreenBall;
    public GameObject GreenBallObj;
    public static GameObject GlobalGreenBall;

    static UIManager manager;
    static LoadManager loadManager;

    int[] numImmunities;

    private bool isPause = false;
    public Font font;
    public float chosenTime = 1;
    
    public enum Mode
    {
        NORMAL,
        PAUSED,
        BESTIARY,
        ABILITY,
        NEW_WEAPON,
        START_GAME,
        WARP
    };

    public void SetWarp(SpriteRenderer displayImg)
    {
        ChangeMode(UIManager.Mode.WARP);
        warp.SetShrineImage(displayImg);
    }

    public Mode mode;
    private int lastChange;
    public BaseCamera currentCamera;


    void Awake()
    {
        if (manager == null)
            manager = this;

        GlobalGreenBall = Instantiate(GreenBallObj, Vector3.zero, Quaternion.identity);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Asderek>();
        pause = gameObject.GetComponentInChildren<PauseMenu>();
        hp = gameObject.GetComponentInChildren<BarDisplay>();
        weapon = gameObject.GetComponentInChildren<Weapons>();
        ability = gameObject.GetComponentInChildren<Ability>();
        interaction = gameObject.GetComponentInChildren<Interaction>();
        beastiary = gameObject.GetComponentInChildren<Beastiary>();
        progress = gameObject.GetComponentInChildren<Progress>();
        buttonManager = gameObject.GetComponentInChildren<ButtonManager>();
        warp = gameObject.GetComponentInChildren<Warp>();
        start = gameObject.GetComponentInChildren<StarMenu>();
    }

    void Start()
    {
        loadManager = LoadManager.getInstance();
        numImmunities = new int[Commandments.getNumberofAnimals()];
        for (int i = 0; i < numImmunities.Length; i++)
        {
            numImmunities[i] = 0;
        }

        lastChange = 100;

        if (mode != Mode.START_GAME)
            Time.timeScale = 0;
    }
    
    void Update()
    {
        lastChange++;

        //TODO
        //if (numImmunities[weapon.getSelectedWeapon().toInt()] > 0)
        //{
        //    weapon.ActiveWarning(true);
        //}
        //else
        //{
        //    weapon.ActiveWarning(false);
        //}


        switch (mode) {

            case Mode.NORMAL:
                weapon.UpdateOnPlay();
                hp.SetHPBarSize(BarDisplay.BarDisplaySize.Mini);
                interaction.UpdateOnPlay();
                break;

            case Mode.PAUSED:
                weapon.UpdateOnPause();
                hp.SetHPBarSize(BarDisplay.BarDisplaySize.Full);
                pause.UpdateOnPause();
                break;

            case Mode.ABILITY:
                ability.UpdateAbilityMenu();
                break;

            case Mode.NEW_WEAPON:
                //TODO weapon.UpdateOnNewWeapon();
                break;
                
            case Mode.START_GAME:
                start.UpdateOnStart();
                break;

            case Mode.WARP:
                warp.UpdateOnWarp();
                break;

        }

    }

    void OnGUI()
    {
        if (!loadManager.gameLoaded)
        {
            if (mode == Mode.START_GAME)
            {
                start.DisplayOnStart();
            }
            return;
        }

        switch (mode)
        {
            case Mode.NORMAL:
                interaction.Display();
                hp.Display();
                ability.DisplayOnPlay();
                weapon.DisplayOnPlay();
                break;

            case Mode.PAUSED:
                pause.Display();
                ability.DisplayOnPause();
                weapon.DisplayOnPause();
                break;

            case Mode.BESTIARY:
                beastiary.Display();
                break;
                
            case Mode.ABILITY:
                ability.DisplayAbilityMenu();
                break;
                
            case Mode.NEW_WEAPON:
                //TODO weapon.DisplayOnNewWeapon();
                break;
                
            case Mode.START_GAME:
                start.DisplayOnStart();
                break;
                
            case Mode.WARP:
                warp.Display();
                break;
        }

    }
    
    public void Pause()
    {
        if (mode == Mode.PAUSED)
            ChangeMode(Mode.NORMAL);
        else if (mode == Mode.NORMAL)
            ChangeMode(Mode.PAUSED);
    }
    
    public void ChangeHP(float percent)
    {
        hp.ChangeHP(percent);
    }

    public void ChangeMP(float percent)
    {
        hp.ChangeMP(percent);
    }

    public Commandments.Weapon getSelectedWeapon()
    {
        return weapon.getSelectedWeapon();
    }

    public Weapons.WeaponSettings getCurrentWeapon()
    {
        return weapon.getCurrentWeapon();
    }

    public void ActivateWeapon(Commandments.ZodiacAnimal zoadicAnimal)
    {
        //TODO weapon.ActiveNewWeapon(zoadicAnimal.toWeapon());
    }

    public bool AddItem(Item item)
    {
        return pause.AddItem(item);
    }

    public void ChangeUlt(float ratio)
    {
        weapon.ChangeUlt(ratio);
    }

    public void newImmunity(Commandments.Element newElement)
    {
        numImmunities[newElement.toInt()]++;
    }

    public void removeImmunity(Commandments.Element elementValue)
    {
        numImmunities[elementValue.toInt()]--;
        if (numImmunities[elementValue.toInt()] < 0)
        {
            numImmunities[elementValue.toInt()] = 0;
        }
    }

    public virtual void InitDisplayOnScreen(GameObject source, string text, ButtonManager.ButtonID button)
    {
        interaction.InitDisplayOnScreen(source, text, button);
    }

    public virtual void StopDisplayOnScreen(GameObject source)
    {
        interaction.StopDisplayOnScreen(source);
    }

    public virtual void ClearScreen()
    {
        interaction.ClearScreen();
    }

    public void ObtainNewYokai(int yokaiNumber)
    {
        beastiary.ObtainNewYokai(yokaiNumber);
    }
    
    public Beastiary.Yokai[] getYokais() {
        return beastiary.yokais;
    }

    public void ChangeMode(Mode mode)
    {
        if (this.mode == mode)
            return;


        lastChange = 0;
        if (mode == Mode.BESTIARY)
        {
            beastiary.ResetCount();
        }
        ability.ChangeGameState(this.mode, mode);
        this.mode = mode;
        weapon.ChangeState(mode);

        switch (mode)
        {
            case Mode.NORMAL:
                Time.timeScale = chosenTime;
                break;

            case Mode.START_GAME:
                start.ChangeState( StarMenu.MenuItemId.Start_Menu);
                Time.timeScale = 0;
                break;

            case Mode.ABILITY:
                ability.Init();
                Time.timeScale = 0;
                break;

            default:
                Time.timeScale = 0;
                break;
        }

    }

    public static UIManager GetInstance()
    {
        if (manager != null)
            return manager;
        throw new System.Exception("UIManager = null");

    }

    public void RegisterCamera(BaseCamera camera)
    {
        currentCamera = camera;
    }


    public void AddItem(Progress.GameItems type)
    {
        progress.AddItem(type);
    }

    internal void ActivateAbility(Ability.AbilityId selectableAbility, Vector3 instantiatePosition)
    {
        ability.ActivateAbility(selectableAbility, instantiatePosition);
    }

    public Asderek GetPlayer()
    {
        return player;
    }

    public void Slow(float gameSpeed)
    {
        if (gameSpeed > 1)
            gameSpeed = 1;
        if (gameSpeed < 0.1f)
            gameSpeed = 0.1f;

        chosenTime = gameSpeed;
        Time.timeScale = gameSpeed;
    }

}
