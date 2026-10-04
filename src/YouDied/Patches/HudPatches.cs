using HarmonyLib;
using YouDied.Infrastructure;

namespace YouDied.Patches
{
    internal static class HudPatches
    {
        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateBlackScreen))]
        private static class UpdateBlackScreen
        {
            private static bool Prefix(Player player)
            {
                var skip = false;
                GameEvents.Instance?.RaiseBlackScreenUpdating(player, ref skip);
                return !skip;
            }
        }
    }
}
