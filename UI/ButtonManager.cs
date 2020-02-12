using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour {

    public bool DEBUG_changeControllerInputs;
    private static ButtonManager buttonManager;
    [System.Serializable]
    public enum ButtonID
    {
        TRIANGLE,
        CIRCLE,
        X,
        SQUARE,
        L1,
        L2,
        L3,
        R1,
        R2,
        R3,
        START,
        SELECT,
        R_UP,
        R_DOWN,
        R_LEFT,
        R_RIGHT,
        L_UP,
        L_DOWN,
        L_LEFT,
        L_RIGHT,
        DIRECT_UP,
        DIRECT_DOWN,
        DIRECT_LEFT,
        DIRECT_RIGHT,
        R,
        L,
        DIRECT,
        BUTTONS,
        CREATE,
        SAVE,
        LOAD,
        CREATE_SAVED,
        UPDATE_BOUNDARIES
    }

    [System.Serializable]
    public enum Controll
    {
        Ps4,
        Xbox,
        Steam,
        Keyboard
    };

    public Controll currentControll;
    [Serializable]
    public class ControllerTextures : SerializableDictionary<Controll, Texture2D> { }


    [System.Serializable]
    public struct ButtonTexture
    {
        public Texture2D ps4;
        public Texture2D xbox;
        public Texture2D steam;
        public Texture2D keyboard;
    }

    [System.Serializable]
    public struct ButtonConfig
    {
        public string inputName;
        public SingleButtonManager.Type type;

        [HideInInspector]
        public SingleButtonManager manager;
    }

    [Serializable]
    public class Config : SerializableDictionary<ButtonID, ButtonConfig> {}
    [Serializable]
    public class ButtonTextures : SerializableDictionary<ButtonID, ButtonTexture> { }


    public Config config;
    public ButtonTextures buttonTextures;
    public ControllerTextures controllerTextures;
    public CheatManager cheats;
    private List<ButtonID> history;

    private void Awake()
    {
        if (buttonManager == null)
            buttonManager = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        foreach (ButtonID id in Enum.GetValues(typeof(ButtonID)))
        {
            ButtonConfig button = config[id];
            button.manager = new SingleButtonManager(id, config[id]);
            config[id] = button;
        }

        history = new List<ButtonID>();
        cheats.Init();
    }

    public static ButtonManager GetInstance()
    {
        if (buttonManager != null)
            return buttonManager;
        throw new System.Exception("ButtonManager = null");
    }

    public void Update () {
        foreach (ButtonID id in Enum.GetValues(typeof(ButtonID)))
        {
            if (config[id].manager.Update())
            {
                print("Button <"+id+">: ");
                history.Add(id);
                if (history.Count > 50)
                {
                    history.RemoveAt(0);
                }
                CheckCheat();
            }
        }


    }

    private void CheckCheat()
    {
        if (cheats.CheckInput(history))
        {
            history.Clear();
        }
    }

    public static Texture GetButtonTexture(ButtonID id)
    {
        switch(buttonManager.currentControll)
        {
            case Controll.Xbox:
                return buttonManager.buttonTextures[id].xbox;

            case Controll.Steam:
                return buttonManager.buttonTextures[id].steam;

            case Controll.Keyboard:
                return buttonManager.buttonTextures[id].keyboard;

            case Controll.Ps4:
            default:
                return buttonManager.buttonTextures[id].ps4;
        }
    }
    
    public static bool GetUp(ButtonID id, System.Object requester)
    {
        id = CheckInvert(id);
        return buttonManager.config[id].manager.GetUp(requester);
    }

    public static bool GetDown(ButtonID id, System.Object requester)
    {
        id = CheckInvert(id);
        return buttonManager.config[id].manager.GetDown(requester);
    }

    public static bool Get(ButtonID id)
    {
        id = CheckInvert(id);
        return buttonManager.config[id].manager.Get();
    }
    private static ButtonID CheckInvert(ButtonID id)
    {
        if (!buttonManager.DEBUG_changeControllerInputs)
            return id;


        switch(id)
        {
            case ButtonID.DIRECT_DOWN:
            case ButtonID.DIRECT_UP:
            case ButtonID.DIRECT_LEFT:
            case ButtonID.DIRECT_RIGHT:
                return (ButtonID)((int)ButtonID.L_UP + (int)id - (int)ButtonID.DIRECT_UP);
            case ButtonID.L_DOWN:
            case ButtonID.L_UP:
            case ButtonID.L_LEFT:
            case ButtonID.L_RIGHT:
                return (ButtonID)((int)ButtonID.DIRECT_UP + (int)id - (int)ButtonID.L_UP);
        }
        return id;
    }
    public static float GetValue(ButtonID id)
    {
        id = CheckInvert(id);
        return buttonManager.config[id].manager.GetValue();
    }

    public static SingleButtonManager.State GetState(ButtonID id, System.Object requester)
    {
        return buttonManager.config[id].manager.GetState(requester);
    }

    public static float GetValue(ButtonID down, ButtonID up)
    {
        return buttonManager.config[up].manager.GetValue() - buttonManager.config[down].manager.GetValue();
    }

    public static Texture2D GetControllerTexture(Controll controll)
    {
        return buttonManager.controllerTextures[controll];
    }

    public static void SetControllerTexture(Controll controll)
    {
        buttonManager.currentControll = controll;
    }


}
