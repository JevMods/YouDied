using System.IO;
using BepInEx;
using HarmonyLib;
using YouDied.Config;
using YouDied.Features;
using YouDied.Infrastructure;

namespace YouDied
{
    [BepInPlugin(PluginGuid, PluginName, BuildInfo.Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "JevMods.YouDied";
        public const string PluginName = "YouDied";

        private IFeature[] _features;
        private Harmony _harmony;

        private void Awake()
        {
            var events = new GameEvents();
            GameEvents.Instance = events;

            var screen = gameObject.AddComponent<DeathScreen>();
            screen.Initialize(Path.Combine(Path.GetDirectoryName(Info.Location), "assets"));

            var settings = new ModSettings(Config);
            _features = new IFeature[]
            {
                new DeathTimingFeature(events),
                new DeathScreenFeature(events, settings, screen)
            };
            foreach (var feature in _features)
            {
                feature.Enable();
            }

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();

            Logger.LogInfo("Loaded");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            if (_features != null)
            {
                foreach (var feature in _features)
                {
                    feature.Disable();
                }
            }

            GameEvents.Instance = null;
        }
    }
}
