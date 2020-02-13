using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


static class CoolDownManager
{

    [System.Serializable]
    public struct CDInfo
    {
        public float activateTime;
        public float cdTime;

        public CDInfo(float activateTime, float cdTime)
        {
            this.activateTime = activateTime;
            this.cdTime = cdTime;
        }

        public float RemainingTimePercent()
        {
            if (cdTime == float.PositiveInfinity)
                return 1;
            return 1- ((Time.time - activateTime)/cdTime);
        }

        public float RemainingTimeAbsolute()
        {
            if (cdTime == float.PositiveInfinity)
                return cdTime;
            return cdTime - (Time.time - activateTime);
        }
    }

    private static Dictionary<KeyValuePair<String, GameObject>, CDInfo> map = new Dictionary<KeyValuePair<String, GameObject>, CDInfo>();
    

    public static bool Apply(string id, GameObject caller, float cd, Func<bool> func)
    {
       id = id.ToLower();
        if (map.ContainsKey(new KeyValuePair<String, GameObject>(id, caller)))
        {
            return false;
        }

        if (func())
        {
            CDInfo info = new CDInfo(Time.time, cd);
            map[new KeyValuePair<String, GameObject>(id, caller)] = info;
            return true;
        }

        return false;
    }

    public static void Remove(string id, GameObject caller=null)
    {
        id = id.ToLower();

        if (!map.ContainsKey(new KeyValuePair<String, GameObject>(id, caller)))
        {
            return;
        }

        map.Remove(new KeyValuePair<String, GameObject>(id, caller));
    }

    public static void clear()
    {
        map.Clear();
    }

    public static void clean()
    {
        /*retorna os valores em que time - activate > cd*/
        Dictionary<KeyValuePair<string, GameObject>, CDInfo> clearMap = map.Where(pair => Time.time - pair.Value.activateTime > pair.Value.cdTime).ToDictionary(pair => pair.Key, pair => pair.Value);
        foreach (var value in clearMap)
        {
            map.Remove(value.Key);
        }
    }

    public static float RemainingTimePercent(string id, GameObject source=null)
    {
        id = id.ToLower();
      
        KeyValuePair<string, GameObject> entry = new KeyValuePair<String, GameObject>(id, source);
        if (map.ContainsKey(entry))
        {
            return map[entry].RemainingTimePercent();
        }
        return 0;
        
    }

    public static float RemainingTimeAbsolute(string id, GameObject source = null)
    {
        id = id.ToLower();

        KeyValuePair<string, GameObject> entry = new KeyValuePair<String, GameObject>(id, source);
        if (map.ContainsKey(entry))
        {
            return map[entry].RemainingTimeAbsolute();
        }
        return 0;
    }

}
