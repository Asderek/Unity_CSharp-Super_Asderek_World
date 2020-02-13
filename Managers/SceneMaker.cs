using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneMaker : MonoBehaviour
{
    private static SceneMaker sceneMaker;

    [System.Serializable]
    public enum GroundSize
    {
        _1x5,
        _5x5,
        _10x5,
        _50x5,
        _1x1,
        _5x1,
        _10x1,
        _50x1,
        _20x8
    }

    [System.Serializable]
    public enum GroundType
    {
        BasicFloor,
        KillingFloor,
        Special
    }

    [System.Serializable]
    public class Ground
    {
        public GroundSize size;
    }

    [System.Serializable]
    public class ContinuosRegion
    {
        public Vector2 relPosition;
        public float accelRatio = 1;
        public float speedRatio = 1;
        public bool inverted = false;
        public bool rotated = false;
        public Commandments.Element element = Commandments.Element.FIRE;
        public GroundType type;
        public List<Ground> grounds;
    }

    [System.Serializable]
    public enum WallSize
    {
        _01,
        _05,
        _10,
        _50
    }

    [System.Serializable]
    public enum WallType
    {
        BasicWall,
        ThickWall
    }

    [System.Serializable]
    public class Wall
    {
        public Vector2 relPosition;
        public WallType type;
        public Commandments.Element element = Commandments.Element.FIRE;
        public bool inverted = false;
        public bool rotated = false;
        public List<WallSize> elements;
    }

    [System.Serializable]
    public class ShrineController
    {
        public Commandments.Shrines type;
        public Vector2 pos;
    }

    [System.Serializable]
    public class ChestController
    {
        public Progress.GameItems type;
        public Vector2 pos;
    }

    [System.Serializable]
    public enum MiscType
    {
        Platform_1x1 = 0,
        Platform_3x1,
        Platform_5x1,
        Platform_15x1,

        TiltedPlatform_5x1_15 = 10,
        TiltedPlatform_15x1_15,
        TiltedPlatform_5x1_345,
        TiltedPlatform_15x1_345,

        FallingPlatform = 20,
        BreakableFloor,

        Ladder13 = 40,

        Pagoda = 49,
        AbilityGuy,
        ScriptGuy,

        ThickRisingDoor = 100,
        Secret,
        SimpleSwitch,
        Geyser,
        RouteArea,
        Fan,

        SpecialSpawner = 150,
        SimpleSpawner,

        Box25x25 = 160,
        Box35x50,
        Box50x50,

        LavaShower = 200,
        SwitchWall,
        BreakableArea
    }


    [System.Serializable]
    public class MiscController
    {
        public MiscType type;
        public Commandments.Element element = Commandments.Element.NEUTRAL;
        public Vector2 pos;
    }

    [System.Serializable]
    public class YokaiController
    {
        public Beastiary.YokaiID type;
        public Vector2 pos;
    }

    [System.Serializable]
    public class Region
    {
        public Vector2 relPosition;
        public List<ContinuosRegion> subRegions;
        public List<Wall> walls;
        public List<ShrineController> shrines;
        public List<ChestController> chest;
        public List<MiscController> misc;
        public List<YokaiController> yokai;
    }

    [System.Serializable]
    public class Limits
    {
        public Vector2 relPosition;
        public Vector2 point1;
        public Vector2 point2;
        public Commandments.LimitsType type;
    }

    [System.Serializable]
    public class Level
    {
        public Vector2 relPosition;
        public List<Region> regions;
        public List<Limits> cameraLimits;
    }

    [System.Serializable]
    public class Part
    {
        public Vector2 position;
        public List<Level> levels;
    }

    [System.Serializable]
    public enum BackgroundType
    {
        Vulcan,
        Nexus
    }

    [System.Serializable]
    public class Scene
    {
        public BackgroundType background;
        public Vector2 bgPosition;
        public List<Part> parts;
    }

    [System.Serializable]
    public class Boundaries
    {
        public List<Scene> scenes;

        //public void Save(string name)
        //{
        //    string json = JsonUtility.ToJson(this);
        //    PlayerPrefs.SetString(name, json);
        //}

        //public void Load(string name)
        //{
        //    string json = PlayerPrefs.GetString(name);
        //    JsonUtility.FromJsonOverwrite(json, this);
        //}

        public virtual void Load(TextAsset asset)
        {
            string json = asset.text;
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
    public class Floors : SerializableDictionary<GroundType, GameObject> { }

    [System.Serializable]
    public class Misc : SerializableDictionary<MiscType, GameObject> { }

    public Boundaries boundariesDescription;
    public GameObject boundaries;
    public TextAsset asset;
    public Boundaries save;
    public int asd = 0;

    private Dictionary<Commandments.Shrines, int> sceneDictionary;


    public static SceneMaker getInstance()
    {
        if (sceneMaker != null)
            return sceneMaker;
        throw new System.Exception("SceneMaker = null");

    }

    void Awake()
    {
        if (sceneMaker == null)
            sceneMaker = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Load();

        sceneDictionary = new Dictionary<Commandments.Shrines, int>();
        int i = 0;
        foreach (Scene scene in save.scenes)
        {
            foreach (Part part in scene.parts)
             {
                foreach (Level level in part.levels)
                {
                    foreach (Region region in level.regions)
                    {
                        if (region.shrines.Count != 0)
                        {
                            foreach (ShrineController shrine in region.shrines)
                            {
                                sceneDictionary.Add(shrine.type, i);
                            }
                        }
                    }

                }
            }
            i++;
        }

    }
    
    void Update()
    {

        if (ButtonManager.GetDown(ButtonManager.ButtonID.SAVE, this))
        {
            print("save");
            Save();
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.LOAD, this))
        {
            print("load");
            Load();
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.CREATE, this))
        {
            print("create boundaries");
            Create(true);
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.CREATE_SAVED, this))
        {
            print("create saved");
            Create(false);
        }

        if (ButtonManager.GetDown(ButtonManager.ButtonID.UPDATE_BOUNDARIES, this))
        {
            print("UPDATE_BOUNDARIES");
            boundariesDescription = save;
         
        }

    }

    private void Create(bool onBoundaries)
    {
        if (boundaries)
            Destroy(boundaries);

        boundaries = new GameObject("Boundaries");

        #region Background

        GameObject bgContainer = new GameObject("Background");
        bgContainer.transform.parent = boundaries.transform;
        bgContainer.transform.localPosition = Vector2.zero;

        String bgName = "Prefab/Background/" + ((onBoundaries) ? boundariesDescription.scenes[asd].background : save.scenes[asd].background).ToString();
        UnityEngine.Object bgPrefab = Resources.Load(bgName);
        if (bgPrefab)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(bgPrefab, Vector3.zero, Quaternion.identity);
            obj.transform.parent = bgContainer.transform;
            obj.transform.localPosition = ((onBoundaries) ? boundariesDescription.scenes[asd].bgPosition : save.scenes[asd].bgPosition);
            print(obj.name);
        }
        else
        {
            Debug.Log("Background not found: " + bgName);
        }
        #endregion

        int i = 0;
        foreach (Part part in ((onBoundaries) ? boundariesDescription.scenes[asd].parts : save.scenes[asd].parts))
        {
            GameObject partObj = new GameObject("Part_" + i);
            partObj.transform.parent = boundaries.transform;
            partObj.transform.localPosition = part.position;

            int j = 0;
            foreach (Level level in part.levels)
            {
                GameObject levelObj = new GameObject("Level_" + j);
                levelObj.transform.parent = partObj.transform;
                levelObj.transform.localPosition = level.relPosition;


                int k = 0;
                foreach (Region region in level.regions)
                {
                    if ((region.subRegions.Count == 0) && (region.walls.Count == 0) && (region.shrines.Count == 0) && (region.misc.Count == 0) && (region.yokai.Count == 0))
                        continue;

                    GameObject regionObj = new GameObject("Region_" + k);
                    regionObj.transform.parent = levelObj.transform;
                    regionObj.transform.localPosition = region.relPosition;

                    #region Ground

                    if (region.subRegions.Count != 0)
                    {
                        GameObject groundContainer = new GameObject("Region_" + k + "_grounds");
                        groundContainer.transform.parent = regionObj.transform;
                        groundContainer.transform.localPosition = Vector2.zero;

                        int l = 0;
                        foreach (ContinuosRegion subRegion in region.subRegions)
                        {
                            if (subRegion.grounds.Count == 0)
                                continue;

                            l++;
                            GameObject subRegionObj = new GameObject("Floor_" + l);
                            subRegionObj.transform.parent = groundContainer.transform;
                            subRegionObj.transform.localPosition = subRegion.relPosition;
                            subRegionObj.name = subRegionObj.name + "_" + subRegion.element;


                            GameObject subRegionCeil = new GameObject("Ceil_" + l);
                            subRegionCeil.transform.parent = groundContainer.transform;
                            subRegionCeil.transform.localPosition = subRegion.relPosition;
                            subRegionCeil.name = subRegionObj.name + "_" + subRegion.element;
                            

                            List<Vector2> coliderPos = new List<Vector2>();
                            List<Vector2> coliderCeil = new List<Vector2>();
                            Vector2 basePos = new Vector2(0, -0.2f);
                            Vector2 baseCeil = new Vector2(0, -4.8f);
                            coliderPos.Add(basePos);
                            coliderCeil.Add(baseCeil);

                            Vector2 relPos = Vector2.zero;
                            foreach (Ground ground in subRegion.grounds)
                            {

                                String fileName = "Prefab/Floor/" + subRegion.element + "/" + subRegion.type.ToString() + ground.size.ToString();
                                UnityEngine.Object pPrefab = Resources.Load(fileName);
                                if (pPrefab)
                                {
                                    GameObject obj = (GameObject)GameObject.Instantiate(pPrefab, Vector3.zero, Quaternion.identity);
                                    obj.transform.parent = subRegionObj.transform;
                                    obj.transform.localPosition = relPos;
                                    
                                    foreach (Vector2 pos in GetColiiderOffset(ground))
                                    {
                                        relPos = relPos + pos;
                                        coliderPos.Add(coliderPos[coliderPos.Count - 1] + pos);
                                        coliderCeil.Add(coliderCeil[coliderCeil.Count - 1] + pos);
                                    }
                                }
                                else
                                {
                                    Debug.Log("floor not found: " + fileName);
                                }

                            }

                            for (int index = 2; index < coliderPos.Count;)
                            {
                                if ((coliderPos[index - 2].x == coliderPos[index - 1].x) && (coliderPos[index - 1].x == coliderPos[index].x))
                                {
                                    coliderPos.RemoveAt(index - 1);
                                    continue;
                                }

                                if ((coliderPos[index - 2].y == coliderPos[index - 1].y) && (coliderPos[index - 1].y == coliderPos[index].y))
                                {
                                    coliderPos.RemoveAt(index - 1);
                                    continue;
                                }

                                index++;
                            }

                            for (int index = 2; index < coliderPos.Count;)
                            {
                                if ((coliderCeil[index - 2].x == coliderCeil[index - 1].x) && (coliderCeil[index - 1].x == coliderCeil[index].x))
                                {
                                    coliderCeil.RemoveAt(index - 1);
                                    continue;
                                }

                                if ((coliderCeil[index - 2].y == coliderCeil[index - 1].y) && (coliderCeil[index - 1].y == coliderCeil[index].y))
                                {
                                    coliderCeil.RemoveAt(index - 1);
                                    continue;
                                }

                                index++;
                            }

                            EdgeCollider2D col = subRegionObj.AddComponent<EdgeCollider2D>();
                            col.points = coliderPos.ToArray();
                            
                            EdgeCollider2D colCeil = subRegionCeil.AddComponent<EdgeCollider2D>();
                            colCeil.points = coliderCeil.ToArray();

                            if (subRegion.type == GroundType.KillingFloor)
                            {
                                subRegionObj.AddComponent<KillingFloor>();
                            } else
                            {
                                Floor floor = subRegionObj.AddComponent<Floor>();
                                floor.accelTimeRatio = subRegion.accelRatio;
                                floor.maxSpeedRatio = subRegion.speedRatio;
                            }

                            subRegionObj.transform.localScale = new Vector3(subRegionObj.transform.localScale.x, subRegionObj.transform.localScale.y * ((subRegion.inverted) ? -1 : 1), subRegionObj.transform.localScale.z);
                            subRegionCeil.transform.localScale = new Vector3(subRegionCeil.transform.localScale.x, subRegionCeil.transform.localScale.y * ((subRegion.inverted) ? -1 : 1), subRegionCeil.transform.localScale.z);

                            if (subRegion.rotated)
                                subRegionObj.transform.Rotate(new Vector3(0, 0, 180));
                            if (subRegion.rotated)
                                subRegionCeil.transform.Rotate(new Vector3(0, 0, 180));
                        }
                    }

                    #endregion

                    #region Wall

                    if (region.walls.Count != 0)
                    {
                        GameObject wallContainer = new GameObject("Region_" + k + "_walls");
                        wallContainer.transform.parent = regionObj.transform;
                        wallContainer.transform.localPosition = Vector2.zero;

                        int l = 0;
                        foreach (Wall wall in region.walls)
                        {
                            if (wall.elements.Count == 0)
                                continue;

                            l++;
                            GameObject subRegionObj = new GameObject("Wall_" + l + "_" + wall.element);
                            subRegionObj.transform.parent = wallContainer.transform;
                            subRegionObj.transform.localPosition = wall.relPosition;
                            int reflexion = ((wall.inverted) ? -1 : 1);

                            if (wall.rotated)
                                subRegionObj.transform.Rotate(new Vector3(0, 0, 180));

                            List<Vector2> coliderPos = new List<Vector2>();
                            List<Vector2> coliderPos2 = new List<Vector2>();
                            Vector2 basePos = new Vector2(reflexion * -0.2f, -0.2f);
                            Vector2 basePos2 = new Vector2(reflexion * -4.8f, -0.2f);
                            coliderPos.Add(basePos);
                            coliderPos2.Add(basePos2);

                            Vector2 relPos = Vector2.zero;
                            foreach (WallSize size in wall.elements)
                            {

                                String fileName = "Prefab/Wall/" + wall.element + "/" + wall.type.ToString() + size.ToString();
                                UnityEngine.Object pPrefab = Resources.Load(fileName);
                                if (pPrefab)
                                {
                                    GameObject obj = (GameObject)GameObject.Instantiate(pPrefab, Vector3.zero, Quaternion.identity);
                                    obj.transform.parent = subRegionObj.transform;
                                    obj.transform.localPosition = relPos;

                                    foreach (Vector2 pos in GetColiiderOffset(size))
                                    {
                                        relPos = relPos + pos;
                                        coliderPos.Add(coliderPos[coliderPos.Count - 1] + pos);
                                        coliderPos2.Add(coliderPos2[coliderPos2.Count - 1] + pos);
                                    }

                                }
                                else
                                {
                                    Debug.Log("Wall not found: " + fileName);
                                }

                            }

                            for (int index = 2; index < coliderPos.Count;)
                            {
                                if ((coliderPos[index - 2].x == coliderPos[index - 1].x) && (coliderPos[index - 1].x == coliderPos[index].x))
                                {
                                    coliderPos.RemoveAt(index - 1);
                                    continue;
                                }

                                if ((coliderPos[index - 2].y == coliderPos[index - 1].y) && (coliderPos[index - 1].y == coliderPos[index].y))
                                {
                                    coliderPos.RemoveAt(index - 1);
                                    continue;
                                }

                                index++;
                            }

                            for (int index = 2; index < coliderPos2.Count;)
                            {
                                if ((coliderPos2[index - 2].x == coliderPos2[index - 1].x) && (coliderPos2[index - 1].x == coliderPos2[index].x))
                                {
                                    coliderPos2.RemoveAt(index - 1);
                                    continue;
                                }

                                if ((coliderPos2[index - 2].y == coliderPos2[index - 1].y) && (coliderPos2[index - 1].y == coliderPos2[index].y))
                                {
                                    coliderPos2.RemoveAt(index - 1);
                                    continue;
                                }

                                index++;
                            }
                            coliderPos[coliderPos.Count - 1] = coliderPos[coliderPos.Count - 1] + new Vector2(0, 0.2f);
                            coliderPos2[coliderPos2.Count - 1] = coliderPos2[coliderPos2.Count - 1] + new Vector2(0, 0.2f);

                            EdgeCollider2D col = subRegionObj.AddComponent<EdgeCollider2D>();
                            col.points = coliderPos.ToArray();

                            if (wall.type == WallType.ThickWall)
                            {
                                EdgeCollider2D col2 = subRegionObj.AddComponent<EdgeCollider2D>();
                                col2.points = coliderPos2.ToArray();
                            }

                            subRegionObj.tag = "Wall";
                            subRegionObj.transform.localScale = new Vector3(subRegionObj.transform.localScale.x * reflexion, subRegionObj.transform.localScale.y, subRegionObj.transform.localScale.y);


                        }
                    }

                    #endregion

                    #region Shrines

                    if (region.shrines.Count != 0)
                    {

                        GameObject shrineContainer = new GameObject("Region_" + k + "_shrines");
                        shrineContainer.transform.parent = regionObj.transform;
                        shrineContainer.transform.localPosition = Vector2.zero;

                        foreach (ShrineController shrine in region.shrines)
                        {
                            String fileName = "Prefab/Others/Shrine";
                            UnityEngine.Object pPrefab = Resources.Load(fileName);
                            if (pPrefab)
                            {
                                GameObject obj = (GameObject)GameObject.Instantiate(pPrefab, Vector3.zero, Quaternion.identity);
                                obj.transform.parent = shrineContainer.transform;
                                obj.transform.localPosition = shrine.pos;

                                Shrine shrineScript = obj.GetComponentInChildren<Shrine>();
                                if (shrineScript)
                                {
                                    shrineScript.id = shrine.type;
                                }
                            }
                            else
                            {
                                Debug.Log("shrine not found: " + fileName);
                            }
                        }
                    }
                    #endregion

                    #region Chests

                    if (region.chest.Count != 0)
                    {

                        GameObject chestContainer = new GameObject("Region_" + k + "_chests");
                        chestContainer.transform.parent = regionObj.transform;
                        chestContainer.transform.localPosition = Vector2.zero;

                        foreach (ChestController controller in region.chest)
                        {
                            String fileName = "Prefab/Others/Chest";
                            UnityEngine.Object pPrefab = Resources.Load(fileName);
                            if (pPrefab)
                            {
                                GameObject obj = (GameObject)GameObject.Instantiate(pPrefab, Vector3.zero, Quaternion.identity);
                                obj.transform.parent = chestContainer.transform;
                                obj.transform.localPosition = controller.pos;

                                Chest chestScript = obj.GetComponentInChildren<Chest>();
                                if (chestScript)
                                {
                                    chestScript.type = controller.type;
                                }
                            }
                            else
                            {
                                Debug.Log("chest not found: " + fileName);
                            }
                        }
                    }
                    #endregion

                    #region Misc

                    if (region.misc.Count != 0)
                    {

                        GameObject miscContainer = new GameObject("Region_" + k + "_misc");
                        miscContainer.transform.parent = regionObj.transform;
                        miscContainer.transform.localPosition = Vector2.zero;

                        foreach (MiscController controller in region.misc)
                        {
                            String fileName = "Prefab/Misc/" + controller.element + "/" + controller.type;
                            UnityEngine.Object pPrefab = Resources.Load(fileName);
                            if (pPrefab)
                            {
                                //print("Creating Misc on: " + (miscContainer.transform.position + (Vector3)controller.pos));
                                GameObject obj = (GameObject)GameObject.Instantiate(pPrefab, miscContainer.transform.position + (Vector3)controller.pos, Quaternion.identity);
                                obj.name = obj.name + "-" + controller.element.ToString();
                                obj.transform.parent = miscContainer.transform;
                            }
                            else
                            {
                                Debug.Log("misc not found: " + fileName);
                            }

                        }

                    }
                    #endregion

                    #region Yokai

                    if (region.yokai.Count != 0)
                    {

                        GameObject yokaiContainer = new GameObject("Region_" + k + "_yokais");
                        yokaiContainer.transform.parent = regionObj.transform;
                        yokaiContainer.transform.localPosition = Vector2.zero;

                        foreach (YokaiController controller in region.yokai)
                        {
                            UnityEngine.Object pPrefab = Resources.Load("Prefab/Yokai/" + controller.type.ToString());
                            if (pPrefab)
                            {
                                GameObject obj = (GameObject)GameObject.Instantiate(pPrefab, Vector3.zero, Quaternion.identity);
                                obj.transform.parent = yokaiContainer.transform;
                                obj.transform.localPosition = controller.pos;
                            }
                            else
                            {
                                Debug.Log("yokai not found: " + "Prefab/Yokai/" + controller.type.ToString());
                            }
                        }
                    }
                    #endregion

                    k++;
                }


                foreach (Limits limit in level.cameraLimits)
                {
                    GameObject limitObj = new GameObject("Limit_" + k);
                    limitObj.transform.parent = levelObj.transform;
                    limitObj.transform.localPosition = limit.relPosition;

                    limitObj.tag = limit.type.ToString();
                    limitObj.layer = LayerMask.NameToLayer("CameraLimits");
                    EdgeCollider2D col = limitObj.AddComponent<EdgeCollider2D>();
                    col.isTrigger = false;
                    List<Vector2> ArrayList = (new List<Vector2>());
                    ArrayList.Add(limit.point1);
                    ArrayList.Add(limit.point2);
                    col.points = ArrayList.ToArray();
                    Rigidbody2D rb = limitObj.AddComponent<Rigidbody2D>();
                    rb.isKinematic = true;


                }
                j++;
            }


            i++;
        }


    }

    public void loadShrine(Commandments.Shrines destinyShrine)
    {
        asd = sceneDictionary[destinyShrine];
        Create(false);
    }

    private List<Vector2> GetColiiderOffset(Ground ground)
    {
        List<Vector2> points = new List<Vector2>();
        switch (ground.size)
        {
            case GroundSize._1x1:
            case GroundSize._1x5:
                points.Add(new Vector2(1, 0));
                break;

            case GroundSize._5x1:
            case GroundSize._5x5:
                points.Add(new Vector2(5, 0));
                break;

            case GroundSize._10x1:
            case GroundSize._10x5:
                points.Add(new Vector2(10, 0));
                break;

            case GroundSize._50x1:
            case GroundSize._50x5:
                points.Add(new Vector2(50, 0));
                break;

            case GroundSize._20x8:
                points.Add(new Vector2(5, 0));
                points.Add(new Vector2(11, 3));
                points.Add(new Vector2(4, 0));
                break;

        }

        return points;
    }

    private List<Vector2> GetColiiderOffset(WallSize size)
    {
        List<Vector2> points = new List<Vector2>();
        switch (size)
        {
            case WallSize._01:
                points.Add(new Vector2(0, -1));
                break;

            case WallSize._05:
                points.Add(new Vector2(0, -5));
                break;

            case WallSize._10:
                points.Add(new Vector2(0, -10));
                break;

            case WallSize._50:
                points.Add(new Vector2(0, -50));
                break;
        }

        return points;
    }

    private void Load()
    {
        save.Load(asset);
    }

    private void Save()
    {
        if (!boundaries)
            return;
        //save = new Boundaries();
        Scene scene = new Scene();
        scene.parts = new List<Part>();

        foreach (Transform part in boundaries.transform)
        {

            if (part.gameObject.name.Contains("Background"))
            {
                foreach (Transform background in part)
                {
                    foreach (BackgroundType type in Enum.GetValues(typeof(BackgroundType)))
                    {
                        if (background.gameObject.name.Contains(type.ToString()))
                        {
                            scene.background = type;
                            scene.bgPosition = background.localPosition;
                            break;
                        }
                    }
                }
            }

            else if (part.gameObject.name.Contains("Part"))
            {

                Part newPart = new Part();
                newPart.position = part.localPosition;
                newPart.levels = new List<Level>();

                foreach (Transform level in part)
                {
                    Level newLevel = new Level();
                    newLevel.relPosition = level.localPosition;
                    newLevel.regions = new List<Region>();
                    newLevel.cameraLimits = new List<Limits>();

                    foreach (Transform region in level)
                    {
                        if (region.gameObject.name.Contains("Limit"))
                        {
                            Limits newLimit = new Limits();
                            newLimit.type = (Commandments.LimitsType)region.localScale.z;
                            newLimit.relPosition = region.localPosition;
                            newLimit.point1 = region.GetComponent<EdgeCollider2D>().points[0];
                            newLimit.point2 = region.GetComponent<EdgeCollider2D>().points[1];

                            newLevel.cameraLimits.Add(newLimit);
                        }
                        if (region.gameObject.name.Contains("Region"))
                        {
                            Region newRegion = new Region();
                            newRegion.relPosition = region.localPosition;
                            newRegion.subRegions = new List<ContinuosRegion>();
                            newRegion.walls = new List<Wall>();
                            newRegion.shrines = new List<ShrineController>();
                            newRegion.chest = new List<ChestController>();
                            newRegion.misc = new List<MiscController>();
                            newRegion.yokai = new List<YokaiController>();

                            foreach (Transform subRegion in region)
                            {
                                if (subRegion.gameObject.name.Contains("grounds"))
                                {
                                    foreach (Transform grounds in subRegion)
                                    {
                                        ContinuosRegion newSubRegion = new ContinuosRegion();
                                        newSubRegion.relPosition = grounds.localPosition;
                                        newSubRegion.grounds = new List<Ground>();
                                        newSubRegion.inverted = grounds.localScale.y == -1;
                                        newSubRegion.rotated = grounds.eulerAngles.z == 180;

                                        Floor floor = region.GetComponent<Floor>();
                                        if (floor)
                                        {
                                            newSubRegion.accelRatio = floor.accelTimeRatio;
                                            newSubRegion.speedRatio = floor.maxSpeedRatio;
                                        }

                                        foreach (Commandments.Element element in Enum.GetValues(typeof(Commandments.Element)))
                                        {
                                            if (grounds.gameObject.name.Contains(element.ToString()))
                                            {
                                                newSubRegion.element = element;
                                                break;
                                            }
                                        }

                                        foreach (Transform ground in grounds)
                                        {
                                            foreach (GroundType id in Enum.GetValues(typeof(GroundType)))
                                            {
                                                foreach (GroundSize size in Enum.GetValues(typeof(GroundSize)))
                                                {
                                                    if ((ground.gameObject.name.StartsWith(id.ToString() + size.ToString())))
                                                    {
                                                        Ground newGround = new Ground();
                                                        newSubRegion.type = id;
                                                        newGround.size = size;
                                                        newSubRegion.grounds.Add(newGround);
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        newRegion.subRegions.Add(newSubRegion);
                                    }
                                }

                                else if (subRegion.gameObject.name.Contains("walls"))
                                {
                                    foreach (Transform walls in subRegion)
                                    {
                                        Wall wall = new Wall();
                                        wall.relPosition = walls.localPosition;
                                        wall.elements = new List<WallSize>();
                                        wall.inverted = walls.localScale.x == -1;
                                        wall.rotated = walls.eulerAngles.z == 180;

                                        foreach (Commandments.Element element in Enum.GetValues(typeof(Commandments.Element)))
                                        {
                                            if (walls.gameObject.name.Contains(element.ToString()))
                                            {
                                                wall.element = element;
                                                break;
                                            }
                                        }

                                        foreach (Transform wallsize in walls)
                                        {
                                            foreach (WallType id in Enum.GetValues(typeof(WallType)))
                                            {
                                                foreach (WallSize size in Enum.GetValues(typeof(WallSize)))
                                                {
                                                    if (wallsize.gameObject.name.StartsWith(id.ToString() + size.ToString()))
                                                    {
                                                        wall.elements.Add(size);
                                                        wall.type = id;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        newRegion.walls.Add(wall);
                                    }
                                }

                                else if (subRegion.gameObject.name.Contains("shrines"))
                                {
                                    foreach (Transform shrines in subRegion)
                                    {
                                        Shrine shrine = shrines.GetComponentInChildren<Shrine>();
                                        if (shrine)
                                        {
                                            ShrineController newController = new ShrineController();
                                            newController.type = shrine.id;
                                            newController.pos = shrines.localPosition;
                                            newRegion.shrines.Add(newController);
                                        }
                                    }
                                }

                                else if (subRegion.gameObject.name.Contains("chests"))
                                {
                                    foreach (Transform chest in subRegion)
                                    {
                                        Chest chestObj = chest.GetComponentInChildren<Chest>();
                                        if (chestObj)
                                        {
                                            ChestController newController = new ChestController();
                                            newController.type = chestObj.type;
                                            newController.pos = chest.localPosition;
                                            newRegion.chest.Add(newController);
                                        }
                                    }
                                }

                                else if (subRegion.gameObject.name.Contains("misc"))
                                {
                                    foreach (Transform obj in subRegion)
                                    {
                                        foreach (MiscType id in Enum.GetValues(typeof(MiscType)))
                                        {
                                            foreach (Commandments.Element element in Enum.GetValues(typeof(Commandments.Element)))
                                            {
                                                if ((obj.gameObject.name.StartsWith(id.ToString())) && (obj.gameObject.name.Contains(element.ToString())))
                                                {
                                                    MiscController newController = new MiscController();
                                                    newController.type = id;
                                                    newController.element = element;
                                                    newController.pos = obj.localPosition;
                                                    newRegion.misc.Add(newController);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                else if (subRegion.gameObject.name.Contains("yokais"))
                                {
                                    foreach (Transform obj in subRegion)
                                    {
                                        foreach (Beastiary.YokaiID id in Enum.GetValues(typeof(Beastiary.YokaiID)))
                                        {
                                            if (obj.gameObject.name.StartsWith(id.ToString()))
                                            {
                                                YokaiController newController = new YokaiController();
                                                newController.type = id;
                                                newController.pos = obj.localPosition;

                                                Enemy script = obj.gameObject.GetComponentInChildren<Enemy>();
                                                if (script)
                                                {
                                                    newController.pos = script.editorStartPosition - subRegion.position;
                                                }

                                                newRegion.yokai.Add(newController);
                                                break;
                                            }
                                        }
                                    }
                                }

                            }

                            newLevel.regions.Add(newRegion);
                        }
                    }
                    newPart.levels.Add(newLevel);
                }

                scene.parts.Add(newPart);

            }

        }

        if (scene.parts.Count < 1)
            return;

        save.scenes[asd] = scene;
        save.Save("Assets/Resources/JSON/fireBoundaries.json");

    }
}
