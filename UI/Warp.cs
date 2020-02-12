using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SceneInfo
{
    public Texture2D warpImg;
    public bool obtained;
}

[System.Serializable]
public struct ShrineInfo
{
    public Texture2D shrineTexture;
    public Sprite shrineSprite;
    public bool obtained;
    public string shrineName;
}

[Serializable]
public class ShrineToTexture : SerializableDictionary<Commandments.Shrines, ShrineInfo> { }
[Serializable]
public class SceneToTexture : SerializableDictionary<string, SceneInfo> { }

public class Warp : MonoBehaviour {


    public ShrineToTexture shrineToTexture;
    public SceneToTexture sceneToTexture;
    public WarpMenu warpMenu;

    private SpriteRenderer currentImg;
    private CircularIndex index;

    private LoadManager loadManager;
    private UIManager uiManager;
    public InputsDisplay inputsDisplay;

    // Use this for initialization
    void Start () {
        index = new CircularIndex(shrineToTexture.Count);
        loadManager = LoadManager.getInstance();
        uiManager = UIManager.GetInstance();
        UpdateScenes();
        inputsDisplay.Init();
    }
	
	// Update is called once per frame
	public void Update ()
    {
	}

    public void Display()
    {
        warpMenu.Draw();
        inputsDisplay.Draw();
    }

    public void UpdateOnWarp()
    {
        if (ApplyOnce.alreadyApplied("Exit", gameObject))
        {
            return;
        }

        int beforeTop = warpMenu.topMenu.menuIndex.Get();
        int beforeLeft = warpMenu.leftMenu.GetIndex();
        warpMenu.Update();
        if (beforeTop != warpMenu.topMenu.menuIndex.Get())
        {
            UpdateShrines();
        }
        if (beforeLeft != warpMenu.leftMenu.GetIndex())
        {
            if (currentImg)
                currentImg.sprite = warpMenu.leftMenu.list[warpMenu.leftMenu.GetIndex()].shrineSprite;
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.CIRCLE,this))
        {
            ApplyOnce.apply("Exit", gameObject, () =>
            {
                if (currentImg)
                    currentImg.sprite = null;

                uiManager.ChangeMode(UIManager.Mode.NORMAL);
                uiManager.GetPlayer().ReceiveNotification(Asderek.Notification.Return);
                return true;
            });
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.X, this))
        {
            //if (currentImg)
            //    currentImg.sprite = null;
            ApplyOnce.apply("Exit", gameObject, () =>
            {
                foreach (KeyValuePair<Commandments.Shrines, ShrineInfo> element in shrineToTexture)
                {
                    if (element.Value.Equals(warpMenu.leftMenu.list[warpMenu.leftMenu.GetIndex()]))
                    {
                        loadManager.loadShrine(element.Key);
                        UIManager.GetInstance().ChangeMode(UIManager.Mode.NORMAL);
                        break;
                    }
                }
                return true;
                }
            );
        }

    }

    public void SetShrineImage(SpriteRenderer displayImg)
    {
        ApplyOnce.remove("Exit", gameObject);
        currentImg = displayImg;
        UpdateScenes();
    }
    
    public Texture2D GetTexture(Commandments.Shrines shrine)
    {
        return shrineToTexture[shrine].shrineTexture;
    }

    public void UpdateScenes()
    {
        warpMenu.topMenu.list.Clear();
        int i = 0; 
        foreach (KeyValuePair<string,SceneInfo> element in sceneToTexture)
        {
            if (element.Value.obtained == true)
            {
                warpMenu.topMenu.list.Add(element.Value);
                if (element.Key == SceneManager.GetActiveScene().name)
                {
                    i = warpMenu.topMenu.list.Count - 1;
                }
            }
        }
        warpMenu.InitScene();
        warpMenu.topMenu.menuIndex.Set(i);
        UpdateShrines();

    }

    public void UpdateShrines()
    {
        warpMenu.leftMenu.list.Clear();
        string sceneName = null;
        foreach (KeyValuePair<string, SceneInfo> element in sceneToTexture)
        {
            if (element.Value.Equals(warpMenu.topMenu.list[warpMenu.topMenu.menuIndex.Get()]))
            {
                sceneName = element.Key;
                break;
            }
        }
        if (sceneName == null)
        {
            return;
        }
        foreach (LoadManager.WarpOptions opt in loadManager.options)
        {
            if (opt.scene == sceneName)
            {
                Commandments.Shrines[] sceneShrines = opt.shrines;
                foreach (KeyValuePair<Commandments.Shrines, ShrineInfo> element in shrineToTexture)
                {
                    foreach (Commandments.Shrines shrine in sceneShrines)
                    { 
                        if (element.Key == shrine)
                        { 
                            if (element.Value.obtained == true)
                            {
                                warpMenu.leftMenu.list.Add(element.Value);
                            }
                        }
                    }
                }
                break;
            }
        }

        warpMenu.InitShrines();
        if (currentImg)
            currentImg.sprite = warpMenu.leftMenu.list[warpMenu.leftMenu.GetIndex()].shrineSprite;
    }
}
