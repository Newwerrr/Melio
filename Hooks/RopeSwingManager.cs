using GorillaLocomotion.Gameplay;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Melio.Hooks
{
    public static class RopeSwingHooks
    {
        private static Type ropeSwingManagerType;
        private static object ropeSwingManagerInstance;
        private static FieldInfo ropesField;

        private static float nextAllowedCallTime;

        private static bool IsReady =>
            ropeSwingManagerType != null &&
            ropeSwingManagerInstance != null &&
            ropesField != null;

        private static void Init()
        {
            if (IsReady)
                return;

            if (ropeSwingManagerType == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type found = null;

                    try
                    {
                        found = asm.GetTypes()
                            .FirstOrDefault(t => t.FullName == "GorillaLocomotion.Gameplay.RopeSwingManager");
                    }
                    catch
                    {
                        continue;
                    }

                    if (found != null)
                    {
                        ropeSwingManagerType = found;
                        MelonLogger.Msg("[RopeHooks] Found RopeSwingManager");
                        break;
                    }
                }

                if (ropeSwingManagerType == null)
                    return;
            }

            if (ropeSwingManagerInstance == null)
            {
                var fields = ropeSwingManagerType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var f in fields)
                {
                    if (f.FieldType == ropeSwingManagerType)
                    {
                        ropeSwingManagerInstance = f.GetValue(null);
                        if (ropeSwingManagerInstance != null)
                            break;
                    }
                }

                if (ropeSwingManagerInstance == null)
                {
                    ropeSwingManagerInstance = UnityEngine.Object.FindObjectOfType(ropeSwingManagerType);
                }

                if (ropeSwingManagerInstance == null)
                    return;
            }

            if (ropesField == null)
            {
                ropesField = ropeSwingManagerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .FirstOrDefault(f => typeof(IDictionary).IsAssignableFrom(f.FieldType));

                if (ropesField == null)
                    return;

                MelonLogger.Msg("[RopeHooks] ropes field found: " + ropesField.Name);
            }
        }

        private static IDictionary GetRopeDict()
        {
            Init();

            if (!IsReady)
                return null;

            return ropesField.GetValue(ropeSwingManagerInstance) as IDictionary;
        }

        public static List<GorillaRopeSwing> GetAllRopes()
        {
            var dict = GetRopeDict();

            if (dict == null || dict.Count == 0)
                return new List<GorillaRopeSwing>();

            var list = new List<GorillaRopeSwing>(dict.Count);

            foreach (DictionaryEntry entry in dict)
            {
                if (entry.Value is GorillaRopeSwing rope)
                    list.Add(rope);
            }

            return list;
        }

        public static GorillaRopeSwing GetRandomRope()
        {
            if (Time.time < nextAllowedCallTime)
                return null;

            nextAllowedCallTime = Time.time + 0.1f;

            var dict = GetRopeDict();

            if (dict == null || dict.Count == 0)
                return null;

            int index = UnityEngine.Random.Range(0, dict.Count);

            foreach (DictionaryEntry entry in dict)
            {
                if (index-- == 0)
                    return entry.Value as GorillaRopeSwing;
            }

            return null;
        }

        public static int GetRandomRopeIndex()
        {
            var dict = GetRopeDict();

            if (dict == null || dict.Count == 0)
                return -1;

            int target = UnityEngine.Random.Range(0, dict.Count);

            int i = 0;

            foreach (DictionaryEntry entry in dict)
            {
                if (i == target)
                    return i;

                i++;
            }

            return -1;
        }
    }
}