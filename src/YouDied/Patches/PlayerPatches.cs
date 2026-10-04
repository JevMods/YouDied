using HarmonyLib;
using YouDied.Infrastructure;

namespace YouDied.Patches
{
    internal static class PlayerPatches
    {
        [HarmonyPatch(typeof(Player), nameof(Player.OnDeath))]
        private static class OnDeath
        {
            private static void Postfix(Player __instance)
            {
                if (__instance == Player.m_localPlayer)
                {
                    GameEvents.Instance?.RaiseLocalPlayerDied();
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Message))]
        private static class Message
        {
            private static bool Prefix(string msg)
            {
                var suppress = false;
                GameEvents.Instance?.RaiseMessageShowing(msg, ref suppress);
                return !suppress;
            }
        }
    }
}
