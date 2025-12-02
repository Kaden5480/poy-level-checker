using HarmonyLib;
using UnityEngine.SceneManagement;

namespace LevelChecker.Patches {
    internal static class SceneLoad {
        /**
         * <summary>
         * Dispatches scene load calls when a custom level (in normal play mode)
         * has been fully loaded.
         * </summary>
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomLevel_DistanceActivator), "InitializeObjects")]
        public static void PlayCustom() {
            Plugin.LogDebug("Custom level (normal play) dispatched");
            Plugin.DispatchSceneLoad(SceneManager.GetActiveScene());
        }

        /**
         * <summary>
         * Dispatches scene load/unload calls when quick playtest mode
         * is activated/deactivated.
         * </summary>
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorManager), "SetPlaymodeObjects")]
        public static void QuickPlayCustom(bool isPlaymode) {
            if (isPlaymode == true) {
                Plugin.LogDebug("Custom level (quick playtest) dispatched");
                Plugin.DispatchSceneLoad(SceneManager.GetActiveScene());
            }
            else {
                Plugin.LogDebug("Custom level (exit quick playtest) dispatched");
                Plugin.DispatchSceneUnload(SceneManager.GetActiveScene());
            }
        }
    }
}
