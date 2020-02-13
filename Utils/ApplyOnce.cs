using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    static class ApplyOnce
    {

        private static Dictionary<KeyValuePair<String,System.Object>, Func<bool>> map = new Dictionary<KeyValuePair<String,System.Object>,Func<bool>>();

        public static void apply(string id, System.Object caller, Func<bool> func)
        {
            id.ToLower();

            if (map.ContainsKey(new KeyValuePair<String,System.Object>(id,caller)))
                return;

            if (func())
            {
                map[new KeyValuePair<String, System.Object>(id, caller)] = func;
            }
        }

        public static void remove(string id, System.Object caller)
        {
            id.ToLower();

            if (!map.ContainsKey(new KeyValuePair<String, System.Object>(id, caller)))
            {
                return;
            }

            map.Remove(new KeyValuePair<String, System.Object>(id, caller));
        }

        public static void clear()
        {
            map.Clear();
        }

        public static bool alreadyApplied(string id, System.Object caller)
        {
            id.ToLower();
        
            return map.ContainsKey(new KeyValuePair<String, System.Object>(id, caller));
        }
    }
