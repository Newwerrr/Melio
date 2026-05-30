using MelonLoader;
using HarmonyLib;
using System;
using System.Reflection;

[assembly: MelonInfo(
    typeof(Melio.Patches.HarmonyHandler),
    "Melio Patches",
    "1.0.0",
    "Newwer"
)]

[assembly: MelonGame(null, null)]

namespace Melio.Patches
{
    public class HarmonyHandler : MelonMod
    {
        private static HarmonyLib.Harmony harmony;
        private const string HarmonyId = "com.melio.patches";

        public static void LoadHarmony()
        {
            try
            {
                MelonLogger.Msg("-[HarmonyHandler]- Initializing Harmony...");

                harmony = new HarmonyLib.Harmony(HarmonyId);

                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                foreach (Type type in currentAssembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(HarmonyPatch), false).Length > 0)
                    {
                        harmony.CreateClassProcessor(type).Patch();

                        MelonLogger.Msg(
                            $"-[HarmonyHandler]- Patched {type.Name}"
                        );
                    }
                }

                MelonLogger.Msg(
                    "-[HarmonyHandler]- Harmony patching complete"
                );
            }
            catch (Exception ex)
            {
                MelonLogger.Error(
                    $"-[HarmonyHandler]- Harmony failed:\n{ex}"
                );
            }
        }

        public override void OnDeinitializeMelon()
        {
            try
            {
                if (harmony != null)
                {
                    harmony.UnpatchSelf();
                    MelonLogger.Msg("-[HarmonyHandler]- Harmony unpatched");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(
                    $"-[HarmonyHandler]- Unpatch failed:\n{ex}"
                );
            }
        }
    }
}