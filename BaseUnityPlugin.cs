using BepInEx;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Elio
{
    [BepInPlugin("com.elio.plugin", "Elio", "1.0.0")]
    public class ElioPlugin : BaseUnityPlugin
    {
        public static List<AdminData> Admins = new List<AdminData>();

        private const string AdminsUrl =
            "https://raw.githubusercontent.com/Newwerrr/Elio/main/Classes/RPConsole/data.json";

        private void Awake()
        {
            Logger.LogInfo("Initializing Elio...");

            GameObject meniObject = new GameObject("Meni");

            DontDestroyOnLoad(meniObject);

            meniObject.AddComponent<Meni>();

            Logger.LogInfo("Meni initialized.");

            StartCoroutine(LoadAdmins());
        }

        private IEnumerator LoadAdmins()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(AdminsUrl))
            {
                yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
                if (request.isNetworkError || request.isHttpError)
#endif
                {
                    Logger.LogError($"Failed to download admin list: {request.error}");
                    yield break;
                }

                string json = request.downloadHandler.text;

                Logger.LogInfo(json);

                try
                {
                    AdminList adminList =
                        JsonConvert.DeserializeObject<AdminList>(json);

                    if (adminList != null && adminList.admins != null)
                    {
                        Admins = new List<AdminData>(adminList.admins);

                        Logger.LogInfo($"Loaded {Admins.Count} admin(s).");
                    }
                    else
                    {
                        Logger.LogWarning("Admin list was empty.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to parse admin list: {ex}");
                }
            }
        }
    }

    [Serializable]
    public class AdminList
    {
        public AdminData[] admins;
    }

    [Serializable]
    public class AdminData
    {
        public string name;

        public string playfab_id;
    }
}
