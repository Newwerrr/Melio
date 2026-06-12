using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;

// namespace Melio.Patches.Menu
// {
//     [HarmonyPatch(typeof(NetworkSystem), nameof(NetworkSystem.Instance.MasterClient))]
//     public class SetMasterPatch
//     {
//         public static bool LocalMasterOverride = false;
//         public static NetPlayer LocalMasterOverrideTarget = null;
// 
//         public static bool Prefix()
//         {
//             if (LocalMasterOverride == true)
//             {
//                 return LocalMasterOverrideTarget;
//             }
//             else
//             {
//                 return NetworkSystem.Instance.MasterClient; // Cannot implicitly convert type 'NetPlayer' to 'bool' | How the actual fuck is this a bool???
// 
//             }
//         }
//     }
// }