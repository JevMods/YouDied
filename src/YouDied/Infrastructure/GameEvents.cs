using System;

namespace YouDied.Infrastructure
{
    public sealed class GameEvents : IGameEvents
    {
        internal static GameEvents Instance { get; set; }

        public event Action LocalPlayerDied;
        public event RespawnRequestedHandler RespawnRequested;
        public event BlackScreenUpdatingHandler BlackScreenUpdating;
        public event MessageShowingHandler MessageShowing;

        internal void RaiseLocalPlayerDied()
        {
            LocalPlayerDied?.Invoke();
        }

        internal void RaiseRespawnRequested(ref float delay, bool afterDeath)
        {
            RespawnRequested?.Invoke(ref delay, afterDeath);
        }

        internal void RaiseBlackScreenUpdating(Player player, ref bool skip)
        {
            BlackScreenUpdating?.Invoke(player, ref skip);
        }

        internal void RaiseMessageShowing(string message, ref bool suppress)
        {
            MessageShowing?.Invoke(message, ref suppress);
        }
    }
}
