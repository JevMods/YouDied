using HarmonyLib;
using YouDied.Infrastructure;

namespace YouDied.Patches
{
    internal static class GamePatches
    {
        [HarmonyPatch(typeof(Game), nameof(Game.RequestRespawn))]
        private static class RequestRespawn
        {
            private static void Prefix(ref float delay, bool afterDeath)
            {
                GameEvents.Instance?.RaiseRespawnRequested(ref delay, afterDeath);
            }
        }
    }
}
