using System;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelChecker {
    [BepInPlugin("com.github.Kaden5480.poy-level-checker", "Level Checker", PluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin {
        private static UI ui;
        internal static Plugin instance;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            Harmony.CreateAndPatchAll(typeof(Patches.SceneLoad));

            SceneManager.sceneUnloaded += UEOnSceneUnloaded;
        }

        /**
         * <summary>
         * Executes when the plugin is being destroyed.
         * </summary>
         */
        private void OnDestroy() {
            SceneManager.sceneUnloaded -= UEOnSceneUnloaded;
        }

        /**
         * <summary>
         * Executes each frame.
         * </summary>
         */
        private void Update() {
            if (ui != null) {
                ui.Update();
            }
        }

        /**
         * <summary>
         * Executes when a scene was unloaded.
         * </summary>
         * <param name="scene">The scene which unloaded</param>
         */
        private void UEOnSceneUnloaded(Scene scene) {
            LogDebug("Unity scene unload dispatched");
            DispatchSceneUnload(scene);
        }

        /**
         * <summary>
         * Dispatches scene loads.
         * </summary>
         * <param name="scene">The scene which loaded</param>
         */
        internal static void DispatchSceneLoad(Scene scene) {
            Cache.FindObjects();

            if (ui == null) {
                ui = new UI();
            }
        }

        /**
         * <summary>
         * Dispatches scene unloads.
         * </summary>
         * <param name="scene">The scene which unloaded</param>
         */
        internal static void DispatchSceneUnload(Scene scene) {
            Cache.Clear();
        }

        /**
         * <summary>
         * Logs a debug message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogDebug(string message) {
#if DEBUG
            if (instance == null) {
                Console.WriteLine($"[Debug] LevelChecker: {message}");
                return;
            }

            instance.Logger.LogInfo(message);
#else
            if (instance != null) {
                instance.Logger.LogDebug(message);
            }
#endif
        }

        /**
         * <summary>
         * Logs an informational message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogInfo(string message) {
            if (instance == null) {
                Console.WriteLine($"[Info] LevelChecker: {message}");
                return;
            }
            instance.Logger.LogInfo(message);
        }

        /**
         * <summary>
         * Logs an error message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogError(string message) {
            if (instance == null) {
                Console.WriteLine($"[Error] LevelChecker: {message}");
                return;
            }
            instance.Logger.LogError(message);
        }
    }
}
