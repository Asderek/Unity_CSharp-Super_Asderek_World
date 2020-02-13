using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class LoadManager : MonoBehaviour
{
    private static LoadManager load;

    private UIManager manager;
    private GameManager gameManager;
    private GameObject player;
    private Asderek playerScript;

    public Texture2D haloText;
    public Texture2D haloConstrictingText;
    private Commandments.Shrines destiny;
    private Rect haloRect;

    [System.Serializable]
    public struct WarpOptions
    {
        public string scene;
        public Commandments.Shrines[] shrines;
    }
    public WarpOptions[] options;

    protected Dictionary<Commandments.Shrines, GameObject> onScene;
    private Commandments.Shrines destinyShrine;

    private int startTime;
    public int transitionTime;
    private Texture2D warpImage;
    private bool startLoading = false;
    private Color[] colors;
    public Area.Position startPosition;
    private Vector2 center;

    //Circle Change Stage
    private List<List<int>> points;
    
    public bool gameLoaded;
    
    public static LoadManager getInstance()
    {
        if (load != null)
            return load;
        throw new System.Exception("LoadManager = null");

    }

    // Use this for initialization
    void Awake()
    {
        if (load == null)
            load = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        onScene = new Dictionary<Commandments.Shrines, GameObject>();

        destinyShrine = Commandments.Shrines.NEXUS;

    }

    void Start()
    {
        colors = null;
        manager = UIManager.GetInstance();
        gameManager = GameManager.GetInstance();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Asderek>();
        startTime = transitionTime;
        gameLoaded = true;
    }

    private void OnGUI()
    {
        if (startTime < transitionTime)
        {
            if (warpImage)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), warpImage);
                if (haloText != null && !ApplyOnce.alreadyApplied("Reload", gameObject))
                {
                    GUI.DrawTexture(haloRect, haloText);
                }
                else if(haloConstrictingText != null && ApplyOnce.alreadyApplied("Reload", gameObject))
                {
                    GUI.DrawTexture(haloRect, haloConstrictingText);
                }
                
            }
        }

    }

    #region Update Base

    void Update()
    {                
        verifyLoad();

        if (startTime < transitionTime)
        {
            if (ApplyOnce.alreadyApplied("Reload", gameObject))
            {
                UpdateCircleConstricting();
                UpdateHalo();
                startTime--;
                if (startTime < 0)
                {
                    startTime = 0;
                    ApplyOnce.remove("Reload", gameObject);
                }
                ApplyOnce.apply("ChangeMode", gameObject, () => {
                    UIManager.GetInstance().ChangeMode(UIManager.Mode.NORMAL);
                    return true;
                });
            }
            else
            {
                if (startTime == 0)
                    UpdateLoadPoints();
                UpdateCircleSimplified();
                startTime++;
                UpdateHalo();
            }

            if (startTime >= transitionTime)
            { 
                ApplyOnce.apply("notifyAsderek", gameObject, () =>
                {
                    playerScript.ReceiveNotification(Asderek.Notification.Return);
                    warpImage = null;
                    Time.timeScale = manager.chosenTime;
                    gameLoaded = true;
                    ApplyOnce.remove("ChangeMode", gameObject);
                    return true;
                });
            }

        }

    }

    public void verifyLoad()
    {
        if (startLoading)
        {
            if (playerScript.FinishedAnimation("sitting_neutral") || playerScript.FinishedAnimation("seated") || ApplyOnce.alreadyApplied("Reload", gameObject))
            {
                gameLoaded = false;
                //Interaction
                manager.ClearScreen();
                //string path = Application.dataPath + "/";
                string path = "D:/";
                string name = "temp.png";

                ApplyOnce.apply("ScreenShot", gameObject, () =>
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    File.Delete(path + name);
                    ScreenCapture.CaptureScreenshot(path + name);
                    UpdateLoadPoints();
                    if (!ApplyOnce.alreadyApplied("Reload", gameObject))
                        Time.timeScale = 0;
                    return true;
                });

                if (File.Exists(path + name))
                {
                    if (ApplyOnce.alreadyApplied("Reload", gameObject))
                        playerScript.ReceiveNotification(Asderek.Notification.Sit);


                    Texture2D auxImage = new Texture2D(100, 100);
                    auxImage.LoadImage(File.ReadAllBytes(path + name));
                    File.Delete(path + name);
                    colors = auxImage.GetPixels();

                    warpImage = new Texture2D(auxImage.width, auxImage.height);
                    warpImage.SetPixels(colors);
                    warpImage.Apply();


                    startLoading = false;
                    loadShrine();

                }

            }
        }
    }

    private void UpdateHalo()
    {
        int squareSide = Mathf.Max(Screen.width, Screen.height)*startTime/transitionTime*2;
        haloRect = new Rect(center.x-squareSide/2, (Screen.height - center.y) - squareSide / 2, squareSide, squareSide);
    }

    public void UpdateCircleSimplified()
    {
        if (startTime >= points.Count)
        {
            return;
        }

        foreach (int i in points[startTime])
        {
            if (colors.Length > i)
                colors[i] = new Color(0, 0, 0, 0);
        }
        warpImage.SetPixels(colors);
        warpImage.Apply();
    }

    public void UpdateCircleConstricting()
    {
        if (startTime < 0)
        {
            return;
        }

        foreach (int i in points[startTime])
        {
            if (colors.Length > i)
                colors[i] = new Color(0, 0, 0, 1);
        }

        //print("warpImage = " + warpImage);
        //print("colors = " + colors);
        warpImage.SetPixels(colors);
        warpImage.Apply();
    }

    #endregion

    public void Reload(Commandments.Shrines destiny)
    {
        destinyShrine = destiny;
        Reload();
    }

    public void Reload()
    {
        ApplyOnce.apply("Reload", gameObject, () =>
        {
            manager.ChangeHP(1);
            loadShrine(destinyShrine);
            return true;
        });
    }
    
    public void loadShrine(Commandments.Shrines shrine)
    {
        startLoading = true;
        destinyShrine = shrine;
    }

    private void loadShrine()
    {
        //if (onScene.ContainsKey(destinyShrine))
        //{
        //    SetActors();
        //    return;
        //}

        onScene.Clear();
        SceneMaker.getInstance().loadShrine(destinyShrine);


        //foreach (WarpOptions warp in options)
        //{
        //    foreach (Commandments.Shrines destiny in warp.shrines)
        //    {
        //        if (destiny == destinyShrine)
        //        {
        //            onScene.Clear();
        //            gameManager.PersistObjets();
        //            SceneManager.LoadScene(warp.scene);
        //            return;
        //        }
        //    }
        //}
    }
    
    private void SetActors()
    {
        foreach (KeyValuePair<Commandments.Shrines, GameObject> element in onScene)
        {
            //Debug.Log("Clearing shrine " + element.Key);
            Shrine shrine = element.Value.GetComponent<Shrine>();
            if (shrine)
            {
                shrine.Clear();
            }
        }

        Vector3 tempVect = onScene[destinyShrine].transform.position;
        tempVect.z = player.transform.position.z;
        player.transform.position = tempVect;
        playerScript.ResetLerp();
        playerScript.Reload();
        gameManager.getCurrentCamera().InstantChangeLocation(player);
        //msg = "New Player position: " + player.transform.position;

        if (warpImage)
        {
            if (ApplyOnce.alreadyApplied("Reload", gameObject))
            {
                startTime = transitionTime - 1;
            }
            else
            {
                //startTime = 0;
                //startTime = DEBUG_asd;
                for (startTime = 0; startTime < 2; startTime++)
                {
                    UpdateCircleSimplified();
                }
            }
        }

        ApplyOnce.remove("notifyAsderek", gameObject);
        ApplyOnce.remove("ScreenShot", gameObject);


    }

    public void RegisterToList(Commandments.Shrines shrine, GameObject me)
    {
        if (onScene.ContainsKey(shrine))
            onScene[shrine] = me;
        else
            onScene.Add(shrine, me);

        if (shrine == destinyShrine)
        {
            SetActors();
        }

    }

    public void UpdateLoadPoints()
    {
        Area.Position targetPosition = gameManager.getCurrentCamera().GetTargetPosition();

        if (( Mathf.Abs(targetPosition.x - startPosition.x) > 0.1f ) || (Mathf.Abs(targetPosition.y - (1- startPosition.y)) > 0.1f))
        {
            points = null;
            startPosition = targetPosition;
            startPosition.y = 1 - startPosition.y;
        }

        if (points == null)
        {
            center = new Vector2(startPosition.x * Screen.width, startPosition.y * Screen.height);
            float radius_factor = (float)Mathf.Max(Screen.width, Screen.height) / transitionTime;

            points = new List<List<int>>();
            for (int i = 0; i < transitionTime; i++)
            {
                points.Add(new List<int>());
            }
            
            Vector2 point = new Vector2(0, 0);
            float magnitude;

            for (int i = 0; i < Screen.height; i++)
            {
                for (int j = 0; j < Screen.width; j++)
                {

                    point.x = j;
                    point.y = i;
                    magnitude = (point - center).magnitude;

                    int index = Mathf.FloorToInt(magnitude / radius_factor);
                    if (index >= points.Count)
                        index = points.Count - 1;

                    int a = i * Screen.width + j;
                    points[index].Add(a);
                }
            }

            int cont = 0;
            for (int i = 0; i < points.Count; i++)
            {
                cont += points[i].Count;
            }
        }
    }

    public Commandments.Shrines GetCurrentShrine()
    {
        return destinyShrine;
    }
}
